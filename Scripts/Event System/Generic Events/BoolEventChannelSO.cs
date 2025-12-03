// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// ScriptableObject event channel specialized for boolean payloads.
/// Allows decoupled systems to raise and listen for boolean events through the inspector-created asset. // short
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/Bool Channel")]
public class BoolEventChannelSO : GenericEventChannelSO<bool>
{
    // No additional implementation required; base class provides publish/subscribe behaviour - short
}