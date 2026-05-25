/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Central system responsible for procedurally spawning dirt patches across the dungeon.
/// MULTIPLAYER READY: In the future, this script should only execute if Runner.IsServer is true.
/// </summary>
public class DirtSpawner : MonoBehaviour
{
    [Header("Difficulty & Balance")]
    [Tooltip("Total number of dirts to spawn in this dungeon (can be modified externally based on difficulty).")]
    public int spawnCount = 50;

    [Header("Spawn Points & Prefabs")]
    [Tooltip("List of all potential spawn points placed in the scene (ProceduralDirtSpawnPoint instances).")]
    public List<ProceduralDirtSpawnPoint> spawnPoints;

    [Tooltip("Random dirt prefabs that can be spawned (e.g., Blood_Prefab, Slime_Prefab).")]
    public List<GameObject> dirtPrefabs;

    /// <summary>
    /// Forces procedural generation of dirt based on the provided target spawn count.
    /// Can be executed manually via Context Menu for testing.
    /// </summary>
    /// <param name="targetSpawnCount">Amount of dirt patches to attempt placing.</param>
    [ContextMenu("Force Spawn Dirts")]
    public void SpawnAllDirts(int targetSpawnCount)
    {
        if (spawnPoints == null || spawnPoints.Count == 0 || dirtPrefabs == null || dirtPrefabs.Count == 0)
        {
            Debug.LogWarning("[DirtSpawner] Missing spawn points or dirt prefabs in inspector!");
            return;
        }

        int actualSpawnCount = Mathf.Min(targetSpawnCount, spawnPoints.Count);

        List<ProceduralDirtSpawnPoint> shuffledPoints = new List<ProceduralDirtSpawnPoint>(spawnPoints);
        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            ProceduralDirtSpawnPoint temp = shuffledPoints[i];
            int randomIndex = Random.Range(i, shuffledPoints.Count);
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        for (int i = 0; i < actualSpawnCount; i++)
        {
            ProceduralDirtSpawnPoint point = shuffledPoints[i];
            GameObject randomPrefab = dirtPrefabs[Random.Range(0, dirtPrefabs.Count)];

            GameObject spawnedDirt = Instantiate(randomPrefab, point.transform.position, point.transform.rotation);
            spawnedDirt.name = $"Procedural_Dirt_{i}_{randomPrefab.name}";

            CleanableBase cleanable = spawnedDirt.GetComponent<CleanableBase>();
            if (cleanable != null && cleanable.dirtData != null)
            {
                cleanable.Initialize(cleanable.dirtData, cleanable.initialDirtiness);
            }
        }

        Debug.Log($"[DirtSpawner] Successfully generated {actualSpawnCount} random stains in the dungeon.");
    }
}