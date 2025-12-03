// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Manages the visual and physical behavior of the equipped weapon.
/// Implements client-side cooldown gating to prevent visual/animation spam.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ToolDataSO currentWeaponData; // Weapon configuration and cooldown data

    [Header("Settings")]
    [SerializeField] private LayerMask hitLayers; // Layers considered for hit detection

    [Header("Dependencies")]
    [SerializeField] private Transform playerCamera; // Reference to player's camera transform
    [SerializeField] private ToolAnimator_DOTween toolAnimator; // Animator wrapper for tool visuals

    [Header("Event Listening")]
    [SerializeField] private VoidEventChannelSO attackChannel; // Event channel used to trigger attacks

    [Header("Debug")]
    [SerializeField] private bool showHitboxGizmos = true; // Toggle to draw hitbox gizmos in editor

    // State tracking
    private float lastAttackTime = -999f; // Tracks last attack time to enforce cooldown

    /// <summary>
    /// Ensure camera reference is valid on awake.
    /// </summary>
    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main.transform; // Fallback to main camera
    }

    /// <summary>
    /// Subscribe to the attack event when enabled.
    /// </summary>
    private void OnEnable()
    {
        if (attackChannel != null)
            attackChannel.OnEventRaised += TryPerformAttack; // Listen for attack invocations
    }

    /// <summary>
    /// Unsubscribe from the attack event when disabled to avoid leaks.
    /// </summary>
    private void OnDisable()
    {
        if (attackChannel != null)
            attackChannel.OnEventRaised -= TryPerformAttack; // Stop listening
    }

    // --- ATTACK FLOW ---

    /// <summary>
    /// Attempt to perform an attack. This method enforces client-side cooldown to
    /// prevent rapid repeated animation triggers and then executes visual and
    /// physical hit detection if allowed.
    /// </summary>
    private void TryPerformAttack()
    {
        //1. COOLDOWN CHECK (prevent visual/animation spam)
        // If current time is less than last attack time + cooldown, abort.
        if (Time.time < lastAttackTime + currentWeaponData.cooldown)
        {
            return; // Do nothing if still in cooldown
        }

        // Attack is allowed: record the timestamp.
        lastAttackTime = Time.time; // Update last attack timestamp

        //2. Play visual attack animation if available
        if (toolAnimator != null)
        {
            toolAnimator.TriggerAttack(); // Trigger tool animation
        }

        //3. Perform physical hit detection
        DetectHits(); // Execute overlap hit checks
    }

    /// <summary>
    /// Perform a box overlap in front of the player to detect hit targets.
    /// Detected IDamageable targets will be reported to the combat manager.
    /// </summary>
    private void DetectHits()
    {
        // Hitbox calculations
        float range = currentWeaponData.maxDistance; // Effective reach of the tool
        Vector3 hitboxCenter = playerCamera.position + (playerCamera.forward * (range * 0.5f)); // Center of overlap box
        Vector3 halfExtents = new Vector3(currentWeaponData.toolRadius, currentWeaponData.toolRadius, range * 0.5f); // Half size of the box

        // Performance optimization: use LayerMask in overlap check
        Collider[] hits = Physics.OverlapBox(hitboxCenter, halfExtents, playerCamera.rotation, hitLayers); // Query colliders in box

        foreach (var hit in hits)
        {
            // Prevent hitting own character (extra safety if LayerMask is insufficient)
            if (hit.transform == transform.root) continue; // Ignore self

            if (hit.TryGetComponent<IDamageable>(out var target))
            {
                Vector3 knockbackDir = (hit.transform.position - transform.position).normalized * currentWeaponData.knockbackPower; // Compute knockback vector
                Vector3 hitPoint = hit.ClosestPoint(transform.position); // Approximate contact point

                // Report hit to authoritative combat manager if available
                if (CombatManager.Instance != null)
                {
                    CombatManager.Instance.ProcessPlayerHit(target, currentWeaponData.damage, knockbackDir, hitPoint); // Forward hit data
                }
            }
        }
    }

    /// <summary>
    /// Draw the weapon hitbox in the editor when debugging is enabled.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!showHitboxGizmos || currentWeaponData == null || playerCamera == null) return; // Preconditions
        Gizmos.color = new Color(1, 0, 0, 0.3f); // Semi-transparent red
        float range = currentWeaponData.maxDistance; // Use configured range
        Vector3 center = playerCamera.position + (playerCamera.forward * (range * 0.5f)); // Hitbox center
        Vector3 size = new Vector3(currentWeaponData.toolRadius * 2, currentWeaponData.toolRadius * 2, range); // Full size for drawing
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, playerCamera.rotation, size); // Transform matrix for the cube
        Gizmos.matrix = rotationMatrix; // Apply transform
        Gizmos.DrawCube(Vector3.zero, Vector3.one); // Draw unit cube which is transformed by matrix
    }
}