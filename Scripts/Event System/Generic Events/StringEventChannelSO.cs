// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Specialized ScriptableObject event channel for broadcasting string payloads.
/// Inherits generic behavior from <see cref="GenericEventChannelSO{T}"/>.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/String Channel")]
public class StringEventChannelSO : GenericEventChannelSO<string>
{
    // Intentionally empty: functionality is provided by the generic base class.
    // Use the generic base to create additional typed channels (float, bool, ItemData, etc.).
}