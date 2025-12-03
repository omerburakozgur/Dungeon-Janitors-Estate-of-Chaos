// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject describing a single loot item used by the loot systems.
/// Stores identifying data, tier and inventory display size. // short
/// </summary>
[CreateAssetMenu(fileName = "LootDataSO", menuName = "Scriptable Objects/LootDataSO")]
public class LootDataSO : ScriptableObject
{
    [Header("Name")] // Display name in inspector
    public string itemName; // Friendly loot name - serialized

    [Header("Loot Tier")] // Rarity classification
    public LootTier LootTier; // Loot rarity - serialized

    [Header("Loot Value")]
    public int lootValue; // Currency or value associated with this item - serialized

    [Header("Inventory Size")]
    public Vector2 inventorySize; //2D size used by inventory UI grid - serialized
}
