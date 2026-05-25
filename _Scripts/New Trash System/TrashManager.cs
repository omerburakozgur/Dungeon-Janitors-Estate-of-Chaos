/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Manages the collection of trash items in the dungeon.
/// Tracks collection count and broadcasts events when collection occurs.
/// </summary>
public class TrashManager : SingletonManager<TrashManager>
{
    private int currentTrashCount = 0;

    [Header("Broadcasting Events")]
    [Tooltip("Event channel raised when any trash item is successfully collected.")]
    [SerializeField] private VoidEventChannelSO onTrashCollectedEvent;

    [Tooltip("Event channel raised whenever the total trash count changes.")]
    [SerializeField] private IntEventChannelSO onTrashCountChangedEvent;

    /// <summary>
    /// Processes a collection request, increments the counter, and destroys the target item.
    /// </summary>
    /// <param name="itemToCollect">The GameObject representing the trash item.</param>
    /// <param name="data">Data definition of the collected item.</param>
    public void RequestCollect(GameObject itemToCollect, TrashDataSO data)
    {
        currentTrashCount += 1;
        Destroy(itemToCollect);

        if (onTrashCollectedEvent != null) onTrashCollectedEvent.Raise();
        if (onTrashCountChangedEvent != null) onTrashCountChangedEvent.Raise(currentTrashCount);
    }

    /// <summary>
    /// Resets the total trash count to zero and notifies subscribers.
    /// </summary>
    public void ResetTrash()
    {
        currentTrashCount = 0;
        if (onTrashCountChangedEvent != null) onTrashCountChangedEvent.Raise(currentTrashCount);
    }
}