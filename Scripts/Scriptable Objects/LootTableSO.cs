// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject defining lists of loot prefabs grouped by rarity tiers.
/// Used by the LootManager to sample and instantiate appropriate loot. // short
/// </summary>
[CreateAssetMenu(fileName = "LootTableSO", menuName = "Scriptable Objects/LootTableSO")]
public class LootTableSO : ScriptableObject
{
    [Header("Common Loot Prefab List")] // Prefab lists by rarity
    public GameObject[] commonLootList; // Common tier prefabs - serialized

    [Header("Rare Loot Prefab List")]
    public GameObject[] rareLootList; // Rare tier prefabs - serialized

    [Header("Epic Loot Prefab List")]
    public GameObject[] epicLootList; // Epic tier prefabs - serialized

    [Header("Legendary Loot Prefab List")]
    public GameObject[] legendaryLootList; // Legendary tier prefabs - serialized
}
