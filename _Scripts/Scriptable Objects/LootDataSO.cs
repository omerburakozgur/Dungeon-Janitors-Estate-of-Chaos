/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Describes a single loot item used by the economy and loot systems.
/// </summary>
[CreateAssetMenu(fileName = "LootDataSO", menuName = "Scriptable Objects/LootDataSO")]
public class LootDataSO : ScriptableObject
{
    [Header("Name")]
    [Tooltip("Display name for the loot item.")]
    public string itemName;

    [Header("Loot Tier")]
    [Tooltip("Rarity tier of this item.")]
    public LootTier LootTier;

    [Header("Loot Value")]
    [Tooltip("Currency value associated with this item.")]
    public int lootValue;

    [Header("Inventory Size")]
    [Tooltip("2D grid dimensions used for inventory UI.")]
    public Vector2 inventorySize;
}