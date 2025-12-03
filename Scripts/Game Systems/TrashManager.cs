// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Authoritative manager for handling trash collection and inventory state.
/// Centralizes validation and triggers events for UI and audio feedback.
/// </summary>
public class TrashManager : SingletonManager<TrashManager>
{
    [Header("Settings")]
    [SerializeField] private PlayerTrashController playerTrashController; // Read-only access to player capacity

    [Header("State (Authoritative)")]
    private int currentTrashCount =0; // Authoritative trash count

    [Header("Broadcasting Events (TDD1.3)")]
    [SerializeField] private VoidEventChannelSO onTrashCollectedEvent; // Sound or fx
    [SerializeField] private IntEventChannelSO onTrashCountChangedEvent; // UI updates
    [SerializeField] private VoidEventChannelSO onInventoryFullEvent; // Inventory full notification

    /// <summary>
    /// Request to collect an item. Validates inventory capacity and, if accepted,
    /// updates authoritative state and raises appropriate events.
    /// </summary>
    /// <param name="itemToCollect">GameObject to remove from the world.</param>
    /// <param name="data">Associated trash data (may be used for value/amount).</param>
    public void RequestCollect(GameObject itemToCollect, TrashDataSO data)
    {
        //1. Validation: Is inventory full?
        if (currentTrashCount >= playerTrashController.maxTrash)
        {
            // Reject the request and notify
            Debug.Log("Inventory Full!");
            if (onInventoryFullEvent != null) onInventoryFullEvent.Raise();
            return;
        }

        //2. Execute: Accept and update state
        currentTrashCount +=1; // For now assume each item counts as1

        // Update the world by destroying the object
        Destroy(itemToCollect);

        //3. Trigger events for UI and audio
        if (onTrashCollectedEvent != null) onTrashCollectedEvent.Raise();
        if (onTrashCountChangedEvent != null) onTrashCountChangedEvent.Raise(currentTrashCount);
    }

    /// <summary>
    /// Request to empty the player's trash inventory.
    /// Resets authoritative count and broadcasts updates.
    /// </summary>
    public void RequestEmptyTrash()
    {
        if (currentTrashCount <=0) return; // Nothing to do

        currentTrashCount =0; // Reset
        if (onTrashCountChangedEvent != null) onTrashCountChangedEvent.Raise(currentTrashCount);
        Debug.Log("Trash Emptied!");
    }

    /// <summary>
    /// Reset trash count for debugging or initialization purposes.
    /// </summary>
    public void ResetTrash()
    {
        currentTrashCount =0;
        if (onTrashCountChangedEvent != null) onTrashCountChangedEvent.Raise(currentTrashCount);
    }
}