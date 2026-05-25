/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 100f;

    [Header("Invulnerability")]
    [Tooltip("Duration in seconds for invulnerability frames after taking damage.")]
    [SerializeField] private float damageCooldown = 1.0f;
    private bool isInvulnerable = false;

    [Header("Events")]
    [Tooltip("Event to trigger Game Over UI in UIManager.")]
    [SerializeField] private VoidEventChannelSO onPlayerDeath;
    [Header("Feedback")]
    [SerializeField] private VoidEventChannelSO cameraShakeChannel;

    [Header("Death Settings")]
    [Tooltip("Layer name to switch to upon death (Must match Physics Matrix!).")]
    [SerializeField] private string ragdollLayerName = "Ragdoll";
    [Tooltip("How fast the player falls over when dying.")]
    [SerializeField] private float fallSpeed = 120f;

    [Header("Physics References")]
    [Tooltip("Assign the Capsule Collider here. It prevents falling through ground when CharacterController is disabled.")]
    [SerializeField] private CapsuleCollider deadBodyCollider;

    [Header("Broadcasting")]
    [Tooltip("Event channel broadcasting health percentage change [0..1].")]
    [SerializeField] private FloatEventChannelSO onHealthChanged;

    private float currentHealth;
    private bool isDead = false;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Applies damage to the player, handling death and invulnerability frames.
    /// </summary>
    public void TakeDamage(float amount, Vector3? knockbackForce = null, Vector3? hitPoint = null, float stunDuration = 0f, float knockbackDuration = 0.2f)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= amount;
        PublishHealth();

        if (cameraShakeChannel != null) cameraShakeChannel.Raise();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(DamageCooldownRoutine());
        }
    }

    public float GetHealth() => currentHealth;
    public bool IsDead() => isDead;

    /// <summary>
    /// Restores health up to the maximum limit.
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        PublishHealth();
    }

    private void PublishHealth()
    {
        if (onHealthChanged != null) onHealthChanged.Raise(currentHealth / maxHealth);
    }

    private IEnumerator DamageCooldownRoutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(damageCooldown);
        isInvulnerable = false;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        if (deadBodyCollider != null)
        {
            deadBodyCollider.enabled = true;
        }
        else
        {
            Debug.LogError("Error: Capsule Collider missing on Player!");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        int layerID = LayerMask.NameToLayer(ragdollLayerName);
        if (layerID != -1)
        {
            gameObject.layer = layerID;
        }

        if (onPlayerDeath != null)
        {
            onPlayerDeath.Raise();
        }

        StartCoroutine(DeathFallRoutine());
    }

    private IEnumerator DeathFallRoutine()
    {
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = startRot * Quaternion.Euler(0, 0, 90f);

        float elapsed = 0f;
        float duration = 1.0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.rotation = Quaternion.Lerp(startRot, targetRot, t);
            yield return null;
        }
    }
}