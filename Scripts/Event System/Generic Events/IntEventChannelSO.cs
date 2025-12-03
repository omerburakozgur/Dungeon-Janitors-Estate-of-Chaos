// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// ScriptableObject channel for broadcasting integer payload events.
/// Inherits generic behaviour from GenericEventChannelSO&lt;int&gt; to provide a typed event channel.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/Int Channel")]
public class IntEventChannelSO : GenericEventChannelSO<int>
{
    // No additional logic required; base class handles event invocation and listener management.
}