// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic ScriptableObject event channel used to decouple producers and consumers.
/// Derived concrete SO types (e.g., FloatEventChannelSO) expose typed channels in the inspector. // short
/// </summary>
public abstract class GenericEventChannelSO<T> : ScriptableObject
{
    /// <summary>
    /// UnityAction invoked when the channel raises an event.
    /// Subscribers can add delegates to this action to receive payloads. // short
    /// </summary>
    public UnityAction<T> OnEventRaised;

    /// <summary>
    /// Raise the channel event with the provided payload, invoking all subscribers.
    /// Null-safe invocation is performed. // short
    /// </summary>
    public void Raise(T value)
    {
        OnEventRaised?.Invoke(value); // safe invoke - short
    }
}