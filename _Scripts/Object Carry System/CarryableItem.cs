/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Carryable physical item that supports pickup/drop via the PhysicsCarryManager.
/// Implements IInteractable to provide interaction prompts and behavior.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CarryableItem : MonoBehaviour, IInteractable
{
    [Header("Identity")]
    [SerializeField] private string itemName = "Object";

    [Header("Physics Settings")]
    [Tooltip("Indicates whether this object can be thrown by the player.")]
    [SerializeField] private bool canBeThrown = true;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    /// <summary>
    /// Queries whether the item supports being thrown by a player.
    /// </summary>
    /// <returns>True if the object can be thrown.</returns>
    public bool CanBeThrown() => canBeThrown;

    /// <summary>
    /// Retrieves the dynamic interaction prompt string based on the player's carry state.
    /// </summary>
    /// <returns>Interaction prompt text.</returns>
    public string GetInteractionPrompt()
    {
        if (PhysicsCarryManager.Instance != null && PhysicsCarryManager.Instance.IsObjectBeingCarried(rb))
        {
            return $"[E] Drop: {itemName}";
        }

        return $"[E] Pick Up: {itemName}";
    }

    /// <summary>
    /// Called when the player executes an interaction request on this item.
    /// Performs pickup or drop action depending on current carry state.
    /// </summary>
    public void RequestInteract()
    {
        if (PhysicsCarryManager.Instance == null) return;

        if (PhysicsCarryManager.Instance.IsObjectBeingCarried(rb))
        {
            PhysicsCarryManager.Instance.DropObject();
        }
        else
        {
            if (!PhysicsCarryManager.Instance.IsCarrying())
            {
                PhysicsCarryManager.Instance.RequestPickup(rb);
            }
        }
    }

    public void RequestHoldInteract() { }
    public void RequestReleaseInteract() { }
}