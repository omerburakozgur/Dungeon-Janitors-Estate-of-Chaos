/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Defines lists of loot prefabs grouped by rarity, used by the LootManager for spawning.
/// </summary>
[CreateAssetMenu(fileName = "LootTableSO", menuName = "Scriptable Objects/LootTableSO")]
public class LootTableSO : ScriptableObject
{
    [Header("Common Loot Prefab List")]
    [Tooltip("Pool of common-tier prefabs.")]
    public GameObject[] commonLootList;

    [Header("Rare Loot Prefab List")]
    [Tooltip("Pool of rare-tier prefabs.")]
    public GameObject[] rareLootList;

    [Header("Epic Loot Prefab List")]
    [Tooltip("Pool of epic-tier prefabs.")]
    public GameObject[] epicLootList;

    [Header("Legendary Loot Prefab List")]
    [Tooltip("Pool of legendary-tier prefabs.")]
    public GameObject[] legendaryLootList;
}