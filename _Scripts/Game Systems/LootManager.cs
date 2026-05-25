/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Central authority responsible for spawning and distributing loot across the level.
/// Handles requests from the combat system when entities are destroyed.
/// </summary>
public class LootManager : SingletonManager<LootManager>
{
    [Header("Loot Configuration")]
    [Tooltip("The central loot table containing all droppable item prefabs grouped by rarity.")]
    [SerializeField] private LootTableSO globalLootTable;

    [Header("Spawn Settings")]
    [Tooltip("Vertical offset applied to the loot spawn position to prevent clipping with the floor.")]
    [SerializeField] private float spawnHeightOffset = 1.0f;
    [Tooltip("Amount of upward force applied to spawned loot for a satisfying 'pop' effect.")]
    [SerializeField] private float spawnPopForce = 4.0f;

    /// <summary>
    /// Requests the spawn of a random loot item at the specified position.
    /// Typically called by the combat manager upon enemy death.
    /// </summary>
    /// <param name="spawnPosition">World position where the loot should appear.</param>
    /// <param name="tier">The tier of the defeated enemy to determine loot rarity.</param>
    public void RequestSpawnLoot(Vector3 spawnPosition, EnemyTier tier)
    {
        if (globalLootTable == null)
        {
            Debug.LogWarning("[LootManager] Global Loot Table is missing. Cannot spawn loot.");
            return;
        }

        GameObject lootPrefab = DetermineLootPrefab(tier);

        if (lootPrefab != null)
        {
            Vector3 finalPos = spawnPosition + (Vector3.up * spawnHeightOffset);
            GameObject spawnedLoot = Instantiate(lootPrefab, finalPos, Quaternion.identity);

            if (spawnedLoot.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 randomDir = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)).normalized;
                rb.AddForce(randomDir * spawnPopForce, ForceMode.Impulse);
            }
        }
    }

    private GameObject DetermineLootPrefab(EnemyTier tier)
    {
        GameObject[] targetList;

        switch (tier)
        {
            case EnemyTier.Elite:
                targetList = globalLootTable.rareLootList;
                break;
            case EnemyTier.Boss:
                targetList = globalLootTable.epicLootList;
                break;
            case EnemyTier.Basic:
            default:
                targetList = globalLootTable.commonLootList;
                break;
        }

        if (targetList != null && targetList.Length > 0)
        {
            int randomIndex = Random.Range(0, targetList.Length);
            return targetList[randomIndex];
        }

        return null;
    }
}