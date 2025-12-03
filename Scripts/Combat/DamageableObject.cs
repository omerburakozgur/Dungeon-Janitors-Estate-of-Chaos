// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Generic damageable component that implements health, optional knockback
/// behavior and death handling. Intended for props and AI characters alike. // short
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class DamageableObject : MonoBehaviour, IDamageable
{
    [Header("Health Stats")] // Inspector: health configuration
    [SerializeField] private float maxHealth = 50f; // Maximum HP - serialized
    private float currentHealth; // Current HP at runtime - short

    [Header("Knockback Settings")] // Inspector: knockback config
    [Tooltip("Whether this object should be pushed when hit. Disable for immovable objects like walls.")]
    [SerializeField] private bool canBeKnockedBack = true; // Toggle physical knockback - serialized

    [Tooltip("Resistance applied to knockback;0 = full knockback,1 = no movement.")]
    [Range(0f, 1f)][SerializeField] private float knockbackResistance = 0f; // Resistance fraction - serialized

    [Header("Feedback")] // Inspector: visual/audio feedback
    [Tooltip("Particle effect to spawn on hit (blood, dust, sparks, etc.).")]
    [SerializeField] private ParticleSystem hitEffect; // Optional hit VFX - serialized

    [Tooltip("Optional ragdoll controller to enable on death for characters.")]
    [SerializeField] private RagdollController ragdoll; // Optional ragdoll handler - serialized

    private Rigidbody rb; // Cached rigidbody reference - runtime

    private void Awake()
    {
        currentHealth = maxHealth; // initialize health - short
        rb = GetComponent<Rigidbody>(); // cache the rigidbody - short
    }

    // --- IDamageable implementation ---
    /// <summary>
    /// Apply damage, optional knockback and spawn hit effects. If health
    /// reaches zero, triggers death behavior. // short
    /// </summary>
    public void TakeDamage(float amount, Vector3? knockbackForce = null, Vector3? hitPoint = null)
    {
        if (currentHealth <= 0) return; // already dead - short

        // Subtract health
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took damage: -{amount}. Remaining: {currentHealth}"); // debug - short

        // Apply physical knockback when applicable
        if (canBeKnockedBack && knockbackForce.HasValue && rb != null)
        {
            ApplyKnockback(knockbackForce.Value); // apply impulse scaled by resistance - short
        }

        // Spawn impact VFX if provided and a hit point was specified
        if (hitEffect != null && hitPoint.HasValue)
        {
            Instantiate(hitEffect, hitPoint.Value, Quaternion.identity); // instantiate VFX - short
        }

        // Check for death
        if (currentHealth <= 0)
        {
            Die(knockbackForce); // execute death routine - short
        }
    }

    /// <summary>
    /// Apply impulse-based knockback scaled by the configured resistance.
    /// Skips applying the force directly when a ragdoll will handle physical reaction. // short
    /// </summary>
    private void ApplyKnockback(Vector3 force)
    {
        float finalForceMultiplier = 1f - knockbackResistance; // compute applied force scale

        if (finalForceMultiplier > 0)
        {
            // If a ragdoll controller is present, prefer activating that instead of applying a root-body impulse
            if (ragdoll == null)
            {
                rb.AddForce(force * finalForceMultiplier, ForceMode.Impulse); // apply impulse - short
            }
        }
    }

    /// <summary>
    /// Handle death: enable ragdoll if provided, otherwise destroy the GameObject.
    /// Also disables root collider and kinematicizes the root rigidbody when ragdolling. // short
    /// </summary>
    private void Die(Vector3? finalForce)
    {
        Debug.Log($"{gameObject.name} died!"); // debug - short

        if (ragdoll != null)
        {
            Vector3 pushForce = finalForce.HasValue ? finalForce.Value : Vector3.zero; // optional impulse to pass to ragdoll

            // Disable main collider and root physics to hand control to the ragdoll
            Collider mainCol = GetComponent<Collider>();
            if (mainCol != null) mainCol.enabled = false; // disable root collider - short

            rb.isKinematic = true; // disable root rigidbody physics - short
            ragdoll.ActivateRagdollWithForce(pushForce); // activate ragdoll with optional force - short
        }
        else
        {
            // No ragdoll: simply remove the object from the scene
            Destroy(gameObject); // cleanup - short
        }
    }

    // --- Simple accessors ---
    public float GetHealth() => currentHealth; // access current health - short
    public bool IsDead() => currentHealth <= 0; // convenience check - short
}