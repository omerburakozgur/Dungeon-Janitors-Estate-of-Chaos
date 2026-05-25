/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawns and manages cleanable objects configured in the inspector.
/// Responsible for instantiating dirt prefabs, registering them with the
/// CleaningManager and optionally handling auto-regeneration.
/// </summary>
public class CleanableObjectManager : SingletonManager<CleanableObjectManager>
{
    [System.Serializable]
    public struct DirtSpawnSettings
    {
        public string name;
        public GameObject prefab;
        public DirtDataSO dirtData;
        [Range(0, 1)] public float startDirtiness;
        public Transform spawnPoint;

        [Header("Regeneration")]
        public bool autoRegenerate;
        public float regenerationDelay;
    }

    [Header("Manual Configuration (Specific/Mission Dirts)")]
    public List<DirtSpawnSettings> spawnList;

    [Header("Procedural Spawning (Random Dirts)")]
    public bool enableProceduralSpawning = true;

    [Tooltip("Total amount of procedural dirt to spawn in this dungeon, usually based on difficulty.")]
    public int proceduralSpawnCount = 50;

    [Tooltip("All potential spawn points placed in the dungeon (Empty GameObjects).")]
    public List<Transform> proceduralSpawnPoints;

    [Tooltip("Randomly selected dirt prefabs (Blood, Slime, Soot, etc.).")]
    public List<GameObject> proceduralDirtPrefabs;

    private Dictionary<int, DirtSpawnSettings> activeSpawns = new Dictionary<int, DirtSpawnSettings>();

    private void Start()
    {
        SpawnAll();

        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.OnDirtCleanedWithID += HandleDirtCleaned;
        }
    }

    private void OnDestroy()
    {
        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.OnDirtCleanedWithID -= HandleDirtCleaned;
        }
    }

    /// <summary>
    /// Triggers both manual and procedural spawning routines.
    /// Can be invoked directly or via Context Menu.
    /// </summary>
    [ContextMenu("Spawn All")]
    public void SpawnAll()
    {
        foreach (var setting in spawnList)
        {
            SpawnDirt(setting);
        }

        if (enableProceduralSpawning)
        {
            SpawnProceduralDirts();
        }
    }

    private void SpawnProceduralDirts()
    {
        if (proceduralSpawnPoints == null || proceduralSpawnPoints.Count == 0 ||
            proceduralDirtPrefabs == null || proceduralDirtPrefabs.Count == 0)
        {
            Debug.LogWarning("[CleanableObjectManager] Procedural spawn points or prefab list is empty!");
            return;
        }

        int countToSpawn = Mathf.Min(proceduralSpawnCount, proceduralSpawnPoints.Count);

        List<Transform> shuffledPoints = new List<Transform>(proceduralSpawnPoints);
        for (int i = 0; i < shuffledPoints.Count; i++)
        {
            Transform temp = shuffledPoints[i];
            int randomIndex = Random.Range(i, shuffledPoints.Count);
            shuffledPoints[i] = shuffledPoints[randomIndex];
            shuffledPoints[randomIndex] = temp;
        }

        for (int i = 0; i < countToSpawn; i++)
        {
            Transform spawnPoint = shuffledPoints[i];
            GameObject randomPrefab = proceduralDirtPrefabs[Random.Range(0, proceduralDirtPrefabs.Count)];

            GameObject obj = Instantiate(randomPrefab, spawnPoint.position, spawnPoint.rotation);
            obj.name = $"Procedural_{randomPrefab.name}_{i}";

            CleanableBase cleanable = obj.GetComponent<CleanableBase>();
            if (cleanable != null)
            {
                float randomDirtiness = Random.Range(0.6f, 1.0f);
                cleanable.Initialize(cleanable.dirtData, randomDirtiness);
            }
        }
    }

    private void SpawnDirt(DirtSpawnSettings setting)
    {
        if (setting.prefab == null) return;

        Vector3 pos = setting.spawnPoint != null ? setting.spawnPoint.position : Vector3.zero;
        Quaternion rot = setting.spawnPoint != null ? setting.spawnPoint.rotation : Quaternion.identity;

        GameObject obj = Instantiate(setting.prefab, pos, rot);
        obj.name = setting.name;

        CleanableBase cleanable = obj.GetComponent<CleanableBase>();
        if (cleanable != null)
        {
            cleanable.Initialize(setting.dirtData, setting.startDirtiness);

            int id = cleanable.GetInstanceID();
            if (!activeSpawns.ContainsKey(id))
            {
                activeSpawns.Add(id, setting);
            }
        }
    }

    private void HandleDirtCleaned(int id)
    {
        if (activeSpawns.ContainsKey(id))
        {
            DirtSpawnSettings setting = activeSpawns[id];

            if (setting.autoRegenerate)
            {
                StartCoroutine(RegenerateRoutine(id, setting.regenerationDelay, setting.startDirtiness));
            }
        }
    }

    private IEnumerator RegenerateRoutine(int id, float delay, float targetValue)
    {
        yield return new WaitForSeconds(delay);
        CleaningManager.Instance.RestoreDirt(id, targetValue);
    }
}