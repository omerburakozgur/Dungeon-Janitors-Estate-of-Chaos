/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Core abstract class governing base AI state machines, movement interactions, and standard detection behaviors.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(EnemyHealth))]
public abstract class EnemyBase : MonoBehaviour
{
    [Header("AI Configuration")]
    [SerializeField] protected EnemyStatsSO stats;

    [Header("Patrol Settings")]
    [SerializeField] protected Transform[] waypoints;
    [SerializeField] protected float patrolWaitTime = 2.0f;

    public enum AIState { Idle, Patrol, Chase, Attack, Stunned, Dead }

    [Header("Debug State")]
    [SerializeField] protected AIState currentState = AIState.Idle;

    [Header("Events")]
    [SerializeField] protected VoidEventChannelSO onPlayerDeath;

    protected NavMeshAgent navAgent;
    protected EnemyHealth health;
    protected Transform targetPlayer;
    protected Animator animator;

    protected float stateTimer;
    protected int currentWaypointIndex = 0;
    protected float stunImmunityTimer = 0f;
    protected float stuckTimer = 0f;

    protected virtual void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        health = GetComponent<EnemyHealth>();
        animator = GetComponentInChildren<Animator>();

        if (stats != null)
        {
            navAgent.speed = stats.patrolSpeed;
            navAgent.stoppingDistance = stats.stoppingDistance;
        }
    }

    protected virtual void Start()
    {
        EnemyAIManager.Instance.RegisterEnemy(this);
        SwitchState(AIState.Patrol);
    }

    protected virtual void OnEnable()
    {
        if (onPlayerDeath != null) onPlayerDeath.OnEventRaised += HandlePlayerDeath;
    }

    protected virtual void OnDisable()
    {
        if (onPlayerDeath != null) onPlayerDeath.OnEventRaised -= HandlePlayerDeath;
    }

    protected virtual void OnDestroy()
    {
        if (EnemyAIManager.Instance != null)
            EnemyAIManager.Instance.UnregisterEnemy(this);
    }

    /// <summary>
    /// Evaluates internal state transitions, countdowns, and active targeting logic.
    /// </summary>
    public virtual void Tick()
    {
        if (stunImmunityTimer > 0) stunImmunityTimer -= Time.deltaTime;
        if (currentState == AIState.Dead || currentState == AIState.Stunned) return;

        stateTimer += Time.deltaTime;

        if (targetPlayer == null && CheckForPlayer())
        {
            SwitchState(AIState.Chase);
        }

        switch (currentState)
        {
            case AIState.Idle:
                if (stateTimer >= patrolWaitTime)
                {
                    SwitchState(AIState.Patrol);
                }
                break;
            case AIState.Patrol:
                HandlePatrol();
                break;
            case AIState.Chase:
                HandleChase();
                break;
            case AIState.Attack:
                HandleAttack();
                break;
        }
    }

    /// <summary>
    /// Safely checks if the NavMeshAgent has reached its destination.
    /// Includes a stuck-detection mechanism to prevent infinite walking in place 
    /// caused by NavMesh polygon seams or pause/unpause state interruptions.
    /// </summary>
    protected bool IsDestinationReached()
    {
        if (navAgent.pathPending)
        {
            stuckTimer = 0f;
            return false;
        }

        if (navAgent.remainingDistance <= (navAgent.stoppingDistance + 0.1f))
        {
            stuckTimer = 0f;
            return true;
        }

        if (navAgent.hasPath && navAgent.velocity.sqrMagnitude < 0.05f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 0.5f)
            {
                stuckTimer = 0f;
                return true;
            }
        }
        else
        {
            stuckTimer = 0f;
        }

        return false;
    }

    /// <summary>
    /// Applies calculated damage force evaluating stun validity matching resistance constraints.
    /// </summary>
    public void ApplyKnockback(Vector3 knockbackDir, float force, float stunDuration, float slideDuration)
    {
        if (currentState == AIState.Dead) return;
        if (stunImmunityTimer > 0) stunDuration = 0f;
        else if (stats != null && stunDuration > 0.5f) stunImmunityTimer = stats.stunImmunityWindow;

        SwitchState(AIState.Stunned);
        StartCoroutine(KnockbackRoutine(knockbackDir, force, slideDuration, stunDuration));
    }

    private IEnumerator KnockbackRoutine(Vector3 dir, float force, float slideDuration, float stunWaitTime)
    {
        dir.y = 0;
        dir.Normalize();

        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;
        navAgent.ResetPath();

        if (animator != null)
        {
            animator.SetTrigger("Hit");
            animator.SetBool("IsStunned", true);
            animator.SetBool("IsIdle", false);
        }

        Vector3 startPos = transform.position;
        Vector3 proposedTargetPos = startPos + (dir * force);
        Vector3 finalTargetPos = proposedTargetPos;

        if (navAgent.Raycast(proposedTargetPos, out UnityEngine.AI.NavMeshHit hit))
        {
            finalTargetPos = hit.position;
        }

        finalTargetPos.y = startPos.y;

        float elapsed = 0f;
        if (slideDuration < 0.05f) slideDuration = 0.05f;

        while (elapsed < slideDuration)
        {
            if (this == null) yield break;
            elapsed += Time.deltaTime;

            float t = Mathf.Sin((elapsed / slideDuration) * Mathf.PI * 0.5f);
            transform.position = Vector3.Lerp(startPos, finalTargetPos, t);

            yield return null;
        }

        transform.position = finalTargetPos;

        float remainingWait = Mathf.Max(stunWaitTime, slideDuration) - slideDuration;
        if (remainingWait > 0) yield return new WaitForSeconds(remainingWait);

        if (currentState != AIState.Dead)
        {
            if (animator != null) animator.SetBool("IsStunned", false);

            navAgent.isStopped = false;
            SwitchState(AIState.Chase);
        }
    }

    /// <summary>
    /// Transitions the AI state machine while ensuring NavMeshAgent parameters adapt smoothly.
    /// </summary>
    public void SwitchState(AIState newState)
    {
        if (currentState == newState || currentState == AIState.Dead) return;

        currentState = newState;
        stateTimer = 0f;

        if (newState == AIState.Patrol)
        {
            navAgent.isStopped = false;
            navAgent.speed = stats.patrolSpeed;
            if (waypoints != null && waypoints.Length > 0)
                navAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
        else if (newState == AIState.Chase)
        {
            navAgent.isStopped = false;
            navAgent.speed = stats.chaseSpeed;
        }
        else if (newState == AIState.Idle)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
            navAgent.ResetPath();
        }
    }

    /// <summary>
    /// Applies a stun effect, stopping movement and interrupting current AI actions.
    /// </summary>
    /// <param name="duration">How long the stun lasts in seconds.</param>
    public virtual void ApplyStun(float duration)
    {
        if (currentState == AIState.Dead) return;

        SwitchState(AIState.Stunned);
        stateTimer = duration;

        if (navAgent != null && navAgent.isActiveAndEnabled)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
            navAgent.ResetPath();
        }

        if (animator != null)
        {
            animator.SetBool("IsStunned", true);
            animator.SetTrigger("Hit");
        }

        InterruptLocalActions();
    }

    /// <summary>
    /// Hook for child classes to stop their specific routines (e.g., Attack Coroutines).
    /// </summary>
    protected virtual void InterruptLocalActions() { }

    protected abstract void HandlePatrol();
    protected abstract void HandleChase();
    protected abstract void HandleAttack();

    /// <summary>
    /// Implements spherical detection queries acquiring target layer intersections.
    /// </summary>
    protected bool CheckForPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, stats.detectionRange);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                IDamageable target = hit.GetComponent<IDamageable>();
                if (target != null && target.IsDead()) continue;
                targetPlayer = hit.transform;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Forces standard idle recovery when target entity logic identifies dead state validation parameters.
    /// </summary>
    protected virtual void HandlePlayerDeath()
    {
        targetPlayer = null;
        SwitchState(AIState.Idle);

        if (navAgent != null)
        {
            navAgent.isStopped = true;
            navAgent.velocity = Vector3.zero;
            navAgent.ResetPath();
        }
    }
}