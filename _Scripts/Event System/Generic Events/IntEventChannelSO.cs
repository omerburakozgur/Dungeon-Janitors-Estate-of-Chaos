/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// ScriptableObject channel for broadcasting integer payload events.
/// Inherits generic behaviour from GenericEventChannelSO&lt;int&gt; to provide a typed event channel.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/Int Channel")]
public class IntEventChannelSO : GenericEventChannelSO<int>
{
}