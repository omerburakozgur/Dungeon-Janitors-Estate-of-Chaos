// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using UnityEngine.Events; // Required for UnityEvent

/// <summary>
/// Editor-friendly listener component that subscribes to a <see cref="VoidEventChannelSO"/> and
/// forwards the notification to a UnityEvent configured in the inspector.
/// Useful for hooking audio or particle systems to SO events without custom scripts.
/// </summary>
public class VoidEventListener : MonoBehaviour
{
    [SerializeField] private VoidEventChannelSO eventChannel; // Channel to subscribe to
    [SerializeField] private UnityEvent onEventRaised; // Event invoked when the channel raises

    private void OnEnable()
    {
        if (eventChannel != null)
            eventChannel.OnEventRaised += Respond; // Subscribe
    }

    private void OnDisable()
    {
        if (eventChannel != null)
            eventChannel.OnEventRaised -= Respond; // Unsubscribe
    }

    /// <summary>
    /// Called when the bound channel is raised; forwards to inspector UnityEvent.
    /// </summary>
    private void Respond()
    {
        onEventRaised.Invoke(); // Invoke inspector-configured responses
    }
}