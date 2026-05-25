/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Generic damageable component that implements health, optional knockback
/// behavior and death handling. Intended for props and AI characters alike.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DamageableObject : MonoBehaviour, IDamageable
{
    [Header("Health Stats")]
    [SerializeField] private float maxHealth = 50f;

    private float currentHealth;

    [Header("Knockback Settings")]
    [Tooltip("Whether this object should be pushed when hit. Disable for immovable objects like walls.")]
    [SerializeField] private bool canBeKnockedBack = true;

    [Tooltip("Resistance applied to knockback; 0 = full knockback, 1 = no movement.")]
    [Range(0f, 1f)][SerializeField] private float knockbackResistance = 0f;

    [Header("Feedback")]
    [Tooltip("Particle effect to spawn on hit (blood, dust, sparks, etc.).")]
    [SerializeField] private ParticleSystem hitEffect;

    [Tooltip("Optional ragdoll controller to enable on death for characters.")]
    [SerializeField] private RagdollController ragdoll;

    private Rigidbody rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Apply damage, optional knockback and spawn hit effects. If health
    /// reaches zero, triggers death behavior.
    /// </summary>
    public void TakeDamage(float amount, Vector3? knockbackForce = null, Vector3? hitPoint = null, float stunDuration = 0, float knockbackDuration = 0.2f)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took damage: -{amount}. Remaining: {currentHealth}");

        if (canBeKnockedBack && knockbackForce.HasValue && rb != null)
        {
            ApplyKnockback(knockbackForce.Value);
        }

        if (hitEffect != null && hitPoint.HasValue)
        {
            Instantiate(hitEffect, hitPoint.Value, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            Die(knockbackForce);
        }
    }

    private void ApplyKnockback(Vector3 force)
    {
        float finalForceMultiplier = 1f - knockbackResistance;

        if (finalForceMultiplier > 0)
        {
            // If a ragdoll controller is present, prefer activating that instead of applying a root-body impulse
            if (ragdoll == null)
            {
                rb.AddForce(force * finalForceMultiplier, ForceMode.Impulse);
            }
        }
    }

    private void Die(Vector3? finalForce)
    {
        Debug.Log($"{gameObject.name} died!");

        if (ragdoll != null)
        {
            Vector3 pushForce = finalForce.HasValue ? finalForce.Value : Vector3.zero;

            Collider mainCol = GetComponent<Collider>();
            if (mainCol != null) mainCol.enabled = false;

            rb.isKinematic = true;
            ragdoll.ActivateRagdollWithForce(pushForce);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Accessor fetching current available runtime health pool.
    /// </summary>
    /// <returns>Current health total.</returns>
    public float GetHealth() => currentHealth;

    /// <summary>
    /// Evaluates if internal health tracker dictates the target is destroyed.
    /// </summary>
    /// <returns>Is dead state flag.</returns>
    public bool IsDead() => currentHealth <= 0;
}