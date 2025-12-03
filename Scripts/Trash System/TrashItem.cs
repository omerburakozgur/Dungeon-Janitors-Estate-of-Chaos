// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Represents a trash object in the world that the player can collect.
/// Implements IInteractable and forwards collection requests to TrashManager.
/// </summary>
public class TrashItem : MonoBehaviour, IInteractable
{
    [Header("Data")]
    [SerializeField] private TrashDataSO trashData; // Metadata for this trash item

    /// <summary>
    /// Interaction prompt text used by PlayerInteraction/UI.
    /// </summary>
    public string GetInteractionPrompt()
    {
        if (trashData != null) return $"[E] Pick Up: {trashData.itemName}";
        return "[E] Pick Up: Trash";
    }

    /// <summary>
    /// Request to collect this item. Forwards the request to the authoritative TrashManager.
    /// </summary>
    public void RequestInteract()
    {
        TrashManager.Instance.RequestCollect(this.gameObject, trashData); // Forward request
    }

    public void RequestHoldInteract() { }
    public void RequestReleaseInteract() { }
}