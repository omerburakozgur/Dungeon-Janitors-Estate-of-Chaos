// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using TMPro;

/// <summary>
/// Listener UI component that displays the player's current trash count and capacity.
/// This component does not reference TrashManager directly and only reacts to events.
/// </summary>
public class UI_TrashCounter : MonoBehaviour
{
    [Header("Listening To")]
    [SerializeField] private IntEventChannelSO onTrashCountChanged; // Event channel to listen for count changes

    [Header("References")]
    [SerializeField] private TextMeshProUGUI counterText; // UI text element to update
    [SerializeField] private PlayerTrashController playerRef; // Reference to read max capacity

    /// <summary>
    /// Subscribe to the trash count event when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        if (onTrashCountChanged != null)
            onTrashCountChanged.OnEventRaised += UpdateCounter; // Subscribe to updates
    }

    /// <summary>
    /// Unsubscribe from the trash count event when the object is disabled to avoid leaks.
    /// </summary>
    private void OnDisable()
    {
        if (onTrashCountChanged != null)
            onTrashCountChanged.OnEventRaised -= UpdateCounter; // Unsubscribe
    }

    /// <summary>
    /// Update the displayed counter when an event is received.
    /// </summary>
    /// <param name="currentCount">Current trash count provided by the event.</param>
    private void UpdateCounter(int currentCount)
    {
        if (playerRef != null)
        {
            counterText.text = $"{currentCount} / {playerRef.maxTrash}"; // e.g. "3 /10"
        }
    }
}