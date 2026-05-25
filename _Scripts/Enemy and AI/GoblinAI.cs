/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using System.Collections;
using UnityEngine;

/// <summary>
/// Specific AI controller mapping combat, chase, and patrol implementations uniquely tailored for Goblin entities.
/// </summary>
public class GoblinAI : EnemyBase
{
    [Header("Goblin Specifics")]
    [SerializeField] private Animator goblinAnimator;

    private float lastAttackTime;
    private bool isAttackRoutineRunning = false;
    private bool canRotateDuringAttack = false;

    protected override void Awake()
    {
        base.Awake();
        if (base.animator == null) base.animator = GetComponentInChildren<Animator>();
        if (goblinAnimator == null) goblinAnimator = base.animator;
        navAgent.updateRotation = true;
    }

    protected override void Start()
    {
        base.Start();
        if (stats != null) lastAttackTime = -stats.attackCooldown;
    }

    /// <summary>
    /// Interrupts the Goblin's specific attack routine when stunned.
    /// </summary>
    protected override void InterruptLocalActions()
    {
        if (isAttackRoutineRunning)
        {
            StopAllCoroutines();
            isAttackRoutineRunning = false;
            canRotateDuringAttack = false;

            if (goblinAnimator != null)
            {
                goblinAnimator.SetBool("IsAttacking", false);
            }
        }
    }

    /// <summary>
    /// Evaluates dynamic state machine rules specifically accounting for attack animation lockouts.
    /// </summary>
    public override void Tick()
    {
        if (currentState == AIState.Stunned)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0f)
            {
                if (goblinAnimator != null) goblinAnimator.SetBool("IsStunned", false);
                SwitchState(AIState.Idle);
            }
            return;
        }

        if (isAttackRoutineRunning)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
            if (canRotateDuringAttack) RotateTowardsTarget();

            UpdateAnimationState();
            return;
        }

        base.Tick();
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        if (goblinAnimator == null || navAgent == null) return;

        if (isAttackRoutineRunning)
        {
            goblinAnimator.SetBool("IsIdle", false);
            goblinAnimator.SetFloat("Speed", 0f);
            return;
        }

        float currentSpeed = navAgent.velocity.magnitude;
        goblinAnimator.SetFloat("Speed", currentSpeed);

        bool shouldBeIdle = (currentSpeed < 0.1f);
        goblinAnimator.SetBool("IsIdle", shouldBeIdle);
    }

    /// <summary>
    /// Handles the patrol logic. Sets the destination safely and uses the 
    /// stuck-proof IsDestinationReached method to prevent walking in place.
    /// </summary>
    protected override void HandlePatrol()
    {
        if (CheckForPlayer())
        {
            SwitchState(AIState.Chase);
            return;
        }

        if (waypoints == null || waypoints.Length == 0) return;

        navAgent.speed = stats.patrolSpeed;
        navAgent.isStopped = false;

        if (!navAgent.hasPath && !navAgent.pathPending)
        {
            navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }

        if (IsDestinationReached())
        {
            SwitchState(AIState.Idle);
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    /// <summary>
    /// Upgraded state transition explicitly mapping initial logic targets on state entries.
    /// </summary>
    public new void SwitchState(AIState newState)
    {
        base.SwitchState(newState);

        if (newState == AIState.Patrol)
        {
            navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    /// <summary>
    /// Drives logic evaluating spatial separation triggering combat states dynamically.
    /// </summary>
    protected override void HandleChase()
    {
        if (targetPlayer == null) { SwitchState(AIState.Idle); return; }

        float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.position);
        if (distanceToPlayer <= stats.attackRange)
        {
            if (Time.time >= lastAttackTime + stats.attackCooldown)
            {
                StartCoroutine(PerformHostAuthoritativeAttack());
            }
            else
            {
                navAgent.isStopped = true;
                navAgent.velocity = Vector3.zero;
                RotateTowardsTarget();
            }
        }
        else
        {
            navAgent.isStopped = false;
            navAgent.speed = stats.chaseSpeed;
            navAgent.SetDestination(targetPlayer.position);
            if (distanceToPlayer > stats.detectionRange * 1.5f) SwitchState(AIState.Patrol);
        }
    }

    /// <summary>
    /// Unused override; Combat logic executes within PerformHostAuthoritativeAttack instead.
    /// </summary>
    protected override void HandleAttack() { }

    private void RotateTowardsTarget()
    {
        if (targetPlayer == null) return;
        Vector3 direction = (targetPlayer.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * stats.rotationSpeed);
        }
    }

    private void SetBool(string name, bool value)
    {
        if (goblinAnimator != null && goblinAnimator.GetBool(name) != value) goblinAnimator.SetBool(name, value);
    }

    private IEnumerator PerformHostAuthoritativeAttack()
    {
        isAttackRoutineRunning = true;
        lastAttackTime = Time.time;
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
        navAgent.ResetPath();
        canRotateDuringAttack = true;

        SetBool("IsAttacking", true);
        SetBool("IsIdle", false);

        yield return new WaitForSeconds(stats.attackDamageDelay);

        canRotateDuringAttack = false;
        if (targetPlayer != null)
        {
            IDamageable target = targetPlayer.GetComponent<IDamageable>();
            if (target != null && !target.IsDead())
            {
                float dist = Vector3.Distance(transform.position, targetPlayer.position);
                if (dist <= stats.attackRange + 1.0f)
                {
                    Vector3 dir = (targetPlayer.position - transform.position).normalized; dir.y = 0;
                    target.TakeDamage(stats.attackDamage, dir * 2f, targetPlayer.position);
                }
            }
        }

        float recovery = stats.attackCooldown - stats.attackDamageDelay;
        if (recovery > 0) yield return new WaitForSeconds(recovery);

        SetBool("IsAttacking", false);
        isAttackRoutineRunning = false;
        canRotateDuringAttack = false;

        if (targetPlayer != null) navAgent.isStopped = false;
        else SwitchState(AIState.Idle);
    }

    /// <summary>
    /// Secures entity returning properly to static idle phases abandoning active tasks immediately.
    /// </summary>
    protected override void HandlePlayerDeath()
    {
        base.HandlePlayerDeath();

        if (isAttackRoutineRunning)
        {
            StopAllCoroutines();
            isAttackRoutineRunning = false;
            canRotateDuringAttack = false;
            SetBool("IsAttacking", false);
        }

        if (goblinAnimator != null)
        {
            goblinAnimator.SetBool("IsIdle", true);
            goblinAnimator.SetFloat("Speed", 0f);
            goblinAnimator.Play("Idle");
        }
    }
}