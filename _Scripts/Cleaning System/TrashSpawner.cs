/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns trash items procedurally at predefined points.
/// Controlled by ActiveMissionManager to prevent race conditions.
/// </summary>
public class TrashSpawner : MonoBehaviour
{
    [Header("Spawn Points & Prefabs")]
    [Tooltip("Empty GameObjects representing potential trash locations.")]
    public List<Transform> spawnPoints;

    [Tooltip("Trash prefabs to spawn (e.g., Bottle, Paper, Box).")]
    public List<GameObject> trashPrefabs;

    /// <summary>
    /// Spawns a specified number of trash items at random locations chosen from the spawn points.
    /// Triggered externally (e.g., by ActiveMissionManager).
    /// </summary>
    /// <param name="count">The requested number of trash items to spawn.</param>
    public void SpawnTrash(int count)
    {
        if (spawnPoints == null || spawnPoints.Count == 0 || trashPrefabs == null || trashPrefabs.Count == 0) return;

        int actualSpawnCount = Mathf.Min(count, spawnPoints.Count);

        // Fisher-Yates Shuffle to randomize spawn points
        List<Transform> shuffledPoints = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            Transform temp = shuffledPoints[i];
            int randomIndex = Random.Range(i, shuffledPoints.Count);
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        for (int i = 0; i < actualSpawnCount; i++)
        {
            Transform point = shuffledPoints[i];
            GameObject randomPrefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];

            GameObject spawnedTrash = Instantiate(randomPrefab, point.position, point.rotation);
            spawnedTrash.name = $"Procedural_Trash_{i}";
        }

        Debug.Log($"[TrashSpawner] Spawned {actualSpawnCount} trash items.");
    }
}