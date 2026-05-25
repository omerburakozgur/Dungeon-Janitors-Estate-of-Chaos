/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Data for a purchaseable shop item, including economy costs and stat upgrade logic.
/// </summary>
[CreateAssetMenu(fileName = "ShopItemSO", menuName = "Scriptable Objects/ShopItemSO")]
public class ShopItemSO : ScriptableObject
{
    [Header("Display Info (UI)")]
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("3D Showcase")]
    [Tooltip("3D prefab displayed in the shop interface.")]
    public GameObject item3DModelPrefab;

    [Header("Economy")]
    public int baseCost;
    public int maxLevel = 5;

    [Header("Type & Logic")]
    public ShopItemType itemType;

    [Tooltip("Reference to tool configuration if this item is a Tool Unlock.")]
    public ToolDataSO toolReward;

    [Tooltip("Identifier for the statistic this item upgrades.")]
    public StatType statType;

    [Tooltip("Stat multiplier applied per level upgrade.")]
    public float statMultiplierPerLevel = 0.1f;

    /// <summary>
    /// Unique identifier for this item, using the ScriptableObject name as the key.
    /// </summary>
    public string ItemID => name;
}