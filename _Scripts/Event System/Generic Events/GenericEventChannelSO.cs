/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Generic ScriptableObject event channel used to decouple producers and consumers.
/// Derived concrete SO types expose typed channels in the inspector.
/// </summary>
public abstract class GenericEventChannelSO<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    /// <summary>
    /// Raise the channel event with the provided payload, invoking all subscribers.
    /// </summary>
    public void Raise(T value)
    {
        OnEventRaised?.Invoke(value);
    }
}