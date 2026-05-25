/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Specialized ScriptableObject event channel for broadcasting string payloads.
/// Inherits generic behavior from GenericEventChannelSO&lt;string&gt;.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/String Channel")]
public class StringEventChannelSO : GenericEventChannelSO<string>
{
}