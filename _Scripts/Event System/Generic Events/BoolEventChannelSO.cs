/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// ScriptableObject event channel specialized for boolean payloads.
/// Allows decoupled systems to raise and listen for boolean events through the inspector-created asset.
/// </summary>
[CreateAssetMenu(menuName = "SO Event Channels/Generic Event Channels/Bool Channel")]
public class BoolEventChannelSO : GenericEventChannelSO<bool>
{
    // No additional implementation required; base class provides publish/subscribe behaviour
}