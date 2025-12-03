// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Controls enabling and disabling of ragdoll physics for a character.
/// Provides methods to toggle ragdoll state and activate ragdoll with impulse force.
/// </summary>
public class RagdollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator; // Animator that controls animated state
    [SerializeField] private Transform ragdollRoot; // Root transform for ragdoll bones (pelvis/hips)

    // Cached ragdoll components collected from the ragdoll root
    private Rigidbody[] ragdollBodies; // All rigidbodies in ragdoll hierarchy
    private Collider[] ragdollColliders; // All colliders in ragdoll hierarchy

    /// <summary>
    /// Gather referenced physics components and ensure character starts animated (ragdoll off).
    /// </summary>
    private void Awake()
    {
        // Find all Rigidbody and Collider components under the ragdoll root
        ragdollBodies = ragdollRoot.GetComponentsInChildren<Rigidbody>(); // Cache rigidbodies
        ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>(); // Cache colliders

        // Ensure ragdoll is disabled at start (character controlled by animator)
        ToggleRagdoll(false); // Start in animated state
    }

    /// <summary>
    /// Enable or disable ragdoll physics. When enabled, animator is disabled and physics
    /// bodies are activated. When disabled, animator is enabled and ragdoll physics are turned off.
    /// </summary>
    /// <param name="state">True to enable ragdoll; false to return to animated state.</param>
    public void ToggleRagdoll(bool state)
    {
        // Animator should be disabled while ragdoll is active
        if (animator != null) animator.enabled = !state; // Toggle animator

        // When ragdoll is active, rigidbodies become non-kinematic to respond to physics
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !state; // Kinematic when animated, dynamic when ragdoll
        }

        // Enable or disable the ragdoll colliders to allow interaction with the world
        foreach (var col in ragdollColliders)
        {
            col.enabled = state; // Colliders enabled only during ragdoll
        }
    }

    /// <summary>
    /// Activate ragdoll and apply an immediate impulse force to all ragdoll parts.
    /// Useful to simulate the final hit force when the character dies.
    /// </summary>
    /// <param name="force">Impulse force vector to apply to ragdoll parts.</param>
    public void ActivateRagdollWithForce(Vector3 force)
    {
        ToggleRagdoll(true); // Enable ragdoll physics

        // Distribute the impulse across all ragdoll rigidbodies for a visually pleasing effect
        foreach (var rb in ragdollBodies)
        {
            rb.AddForce(force, ForceMode.Impulse); // Apply impulse to each part
        }
    }
}