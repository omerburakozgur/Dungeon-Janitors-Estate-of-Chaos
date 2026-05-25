/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Health component interfacing with global damage systems while passing reaction hooks to local AI.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Configuration")]
    [SerializeField] private EnemyStatsSO stats;

    [Header("Visual Feedback")]
    [SerializeField] private ParticleSystem hitEffect;

    private float currentHealth;
    private bool isDead = false;

    private Animator animator;
    private Rigidbody rb;
    private RagdollController ragdoll;
    private EnemyBase enemyAI;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        ragdoll = GetComponent<RagdollController>();
        enemyAI = GetComponent<EnemyBase>();

        InitializeHealth();

        if (rb != null) rb.isKinematic = true;
    }

    private void InitializeHealth()
    {
        if (stats != null)
        {
            currentHealth = stats.maxHP;
        }
        else
        {
            currentHealth = 100f;
        }
    }

    /// <summary>
    /// Calculates sustained damage mapping structural data logic triggers validating stun logic calculations.
    /// </summary>
    public void TakeDamage(float amount, Vector3? knockbackForce = null, Vector3? hitPoint = null, float stunDuration = 0f, float knockbackDuration = 0.2f)
    {
        if (isDead) return;

        currentHealth -= amount;

        if (hitEffect != null && hitPoint.HasValue)
        {
            Instantiate(hitEffect, hitPoint.Value, Quaternion.LookRotation(hitPoint.Value - transform.position));
        }

        if (currentHealth <= 0)
        {
            Die(knockbackForce);
            return;
        }

        if (stunDuration > 0f && enemyAI != null)
        {
            enemyAI.ApplyStun(stunDuration);
        }

        if (knockbackForce.HasValue && enemyAI != null && stats != null)
        {
            float rawForce = knockbackForce.Value.magnitude;

            float finalForce = rawForce * (1f - stats.knockbackResistance);
            float finalStun = stunDuration * (1f - stats.stunResistance);

            if (finalForce > 0.1f || finalStun > 0.1f)
            {
                float distanceMultiplier = 0.5f;

                enemyAI.ApplyKnockback(
                    knockbackForce.Value.normalized,
                    finalForce * distanceMultiplier,
                    finalStun,
                    knockbackDuration
                );
            }
        }
    }

    /// <summary>
    /// Returns current health capacity.
    /// </summary>
    public float GetHealth() => currentHealth;

    /// <summary>
    /// Determines active death phase of component.
    /// </summary>
    public bool IsDead() => isDead;

    private void Die(Vector3? finalForce)
    {
        if (isDead) return;
        isDead = true;

        if (enemyAI != null) enemyAI.SwitchState(EnemyBase.AIState.Dead);

        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.ReportEnemyDeath(this.gameObject);
        }

        Collider mainCol = GetComponent<Collider>();
        if (mainCol != null) mainCol.enabled = false;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        if (ragdoll != null)
        {
            Vector3 pushForce = finalForce.HasValue ? finalForce.Value : Vector3.zero;
            ragdoll.ActivateRagdollWithForce(pushForce);
        }
        else
        {
            if (animator != null) animator.SetBool("Dead", true);
            Destroy(gameObject, 5f);
        }
    }
}