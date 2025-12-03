// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// ScriptableObject channel specialized for float payload events.
/// Use these assets to decouple systems by raising float-typed events in the editor.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/Float Channel")]
public class FloatEventChannelSO : GenericEventChannelSO<float>
{
    // Implementation is provided by the generic base class
}