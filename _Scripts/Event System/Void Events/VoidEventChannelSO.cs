// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Lightweight ScriptableObject event channel that broadcasts a void (no-data) event.
/// Intended for simple notifications such as playing a sound or triggering a VFX.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Void Events/Void Channel")]
public class VoidEventChannelSO : ScriptableObject
{
 /// <summary>Subscriber callback invoked when the event is raised.</summary>
 public UnityAction OnEventRaised; // Event subscribers

 /// <summary>Invoke the channel and notify all subscribers.</summary>
 public void Raise()
 {
 OnEventRaised?.Invoke(); // Safe-invoke subscribers
 }
}