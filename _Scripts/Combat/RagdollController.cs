/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Controls enabling and disabling of ragdoll physics for a character.
/// Handles cleanup (Layer change) to prevent player tripping over corpses.
/// </summary>
public class RagdollController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [SerializeField] private Transform ragdollRoot;

    [Header("Weapon Handling")]
    [Tooltip("Drag the Sword/Weapon Collider here to disable it on death.")]
    [SerializeField] private Collider weaponCollider;

    [Header("Settings")]
    [Tooltip("Layer to assign to body parts on death (Must be setup in Physics Matrix!).")]
    [SerializeField] private string deadBodyLayer = "DeadBody";

    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;

    private void Awake()
    {
        ragdollBodies = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>();

        ToggleRagdoll(false);
    }

    /// <summary>
    /// Toggles the physical state of the character between animated and physics-driven ragdoll.
    /// </summary>
    /// <param name="state">True to activate ragdoll, false to return to normal animation.</param>
    public void ToggleRagdoll(bool state)
    {
        if (animator != null) animator.enabled = !state;

        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = !state;
        }

        foreach (var col in ragdollColliders)
        {
            col.enabled = state;
        }

        // normally active, MUST BE DISABLED in Ragdoll to prevent tripping
        if (weaponCollider != null)
        {
            weaponCollider.enabled = !state;
        }
    }

    /// <summary>
    /// Immediately activates the ragdoll state, assigns the corpse physics layer, and applies a localized impulse.
    /// </summary>
    /// <param name="force">The impulse vector applied to all ragdoll bodies.</param>
    public void ActivateRagdollWithForce(Vector3 force)
    {
        ToggleRagdoll(true);

        // Change Layer to prevent player from tripping
        int layerID = LayerMask.NameToLayer(deadBodyLayer);
        if (layerID != -1)
        {
            ChangeLayersRecursively(this.transform, layerID);
        }
        else
        {
            Debug.LogWarning($"[RagdollController] Layer '{deadBodyLayer}' not found! Check Project Settings.");
        }

        foreach (var rb in ragdollBodies)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    private void ChangeLayersRecursively(Transform trans, int layerID)
    {
        trans.gameObject.layer = layerID;
        foreach (Transform child in trans)
        {
            ChangeLayersRecursively(child, layerID);
        }
    }
}