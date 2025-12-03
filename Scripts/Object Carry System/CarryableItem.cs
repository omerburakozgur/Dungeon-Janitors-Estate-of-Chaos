// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

// Carryable physical item that supports pickup/drop via the PhysicsCarryManager
// Implements IInteractable to provide interaction prompts and behavior. // short
[RequireComponent(typeof(Rigidbody))]
public class CarryableItem : MonoBehaviour, IInteractable
{
    [Header("Identity")] // Inspector: display name
    [SerializeField] private string itemName = "Object"; // Human-readable name - serialized

    [Header("Physics Settings")] // Inspector: physics configuration
    /// <summary>
    /// Indicates whether this object can be thrown by the player.
    /// Some items (e.g. generators) may be non-throwable. // short
    /// </summary>
    [SerializeField] private bool canBeThrown = true; // Throw capability - serialized

    private Rigidbody rb; // Cached rigidbody reference - runtime

    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); // cache rb

        // Ensure continuous collision detection for robust physics interactions
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // prevent tunneling - short
    }

    // --- API consumed by PhysicsCarryManager ---
    public bool CanBeThrown() => canBeThrown; // Query if this item supports throwing - short

    // --- IInteractable Implementation ---
    public string GetInteractionPrompt()
    {
        // If this object is currently being carried show the 'drop' prompt
        if (PhysicsCarryManager.Instance != null && PhysicsCarryManager.Instance.IsObjectBeingCarried(rb))
        {
            return $"[E] Drop: {itemName}"; // show drop prompt - short
        }

        return $"[E] Pick Up: {itemName}"; // default pick up prompt - short
    }

    public void RequestInteract()
    {
        if (PhysicsCarryManager.Instance == null) return; // guard

        // If currently carried, request drop. Otherwise request pickup if player is free-handed
        if (PhysicsCarryManager.Instance.IsObjectBeingCarried(rb))
        {
            PhysicsCarryManager.Instance.DropObject(); // drop current object - short
        }
        else
        {
            if (!PhysicsCarryManager.Instance.IsCarrying())
            {
                PhysicsCarryManager.Instance.RequestPickup(rb); // attempt pickup - short
            }
        }
    }

    // Hold interactions not used for this item; methods are intentionally empty
    public void RequestHoldInteract() { }
    public void RequestReleaseInteract() { }
}