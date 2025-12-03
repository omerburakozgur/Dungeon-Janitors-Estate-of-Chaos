// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawns and manages cleanable objects configured in the inspector.
/// Responsible for instantiating dirt prefabs, registering them with the
/// CleaningManager and optionally handling auto-regeneration. // short
/// </summary>
public class CleanableObjectManager : SingletonManager<CleanableObjectManager>
{
    [System.Serializable]
    public struct DirtSpawnSettings
    {
        public string name; // Friendly name used for spawned GameObjects - short
        public GameObject prefab; // Prefab that contains a CleanableBase component - short
        public DirtDataSO dirtData; // Dirt type configuration to apply - short
        [Range(0, 1)] public float startDirtiness; // Initial dirtiness (0..1) - short
        public Transform spawnPoint; // Optional transform used as the spawn location - short

        [Header("Regeneration")] // Regeneration grouping in inspector
        public bool autoRegenerate; // If true the spawn will respawn after being cleaned - short
        public float regenerationDelay; // Delay (seconds) before regeneration occurs - short
    }

    [Header("Configuration")] // Inspector list header
    public List<DirtSpawnSettings> spawnList; // Configured spawns to create at Start - serialized

    // Runtime lookup of settings by instance id so we can regenerate or reference metadata
    private Dictionary<int, DirtSpawnSettings> activeSpawns = new Dictionary<int, DirtSpawnSettings>(); // runtime map - short

    /// <summary>
    /// Spawn all configured entries and subscribe to cleaning events.
    /// </summary>
    private void Start()
    {
        SpawnAll(); // Create instances for every spawn entry

        // Subscribe to the authoritative CleaningManager to react to fully cleaned notifications
        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.OnDirtCleanedWithID += HandleDirtCleaned; // register handler - short
        }
    }

    private void OnDestroy()
    {
        if (CleaningManager.Instance != null)
        {
            CleaningManager.Instance.OnDirtCleanedWithID -= HandleDirtCleaned; // unregister handler - short
        }
    }

    /// <summary>
    /// Spawn all entries from the inspector list. Can be invoked from the context menu.
    /// </summary>
    [ContextMenu("Spawn All")]
    public void SpawnAll()
    {
        foreach (var setting in spawnList)
        {
            SpawnDirt(setting); // spawn each configured entry - short
        }
    }

    /// <summary>
    /// Instantiates a dirt prefab according to the given settings and registers it for potential regeneration.
    /// </summary>
    private void SpawnDirt(DirtSpawnSettings setting)
    {
        if (setting.prefab == null) return; // skip empties - short

        Vector3 pos = setting.spawnPoint != null ? setting.spawnPoint.position : Vector3.zero; // determine position
        Quaternion rot = setting.spawnPoint != null ? setting.spawnPoint.rotation : Quaternion.identity; // determine rotation

        GameObject obj = Instantiate(setting.prefab, pos, rot); // create instance
        obj.name = setting.name; // set readable name for hierarchy

        CleanableBase cleanable = obj.GetComponent<CleanableBase>();
        if (cleanable != null)
        {
            // Initialize the cleanable before the manager's logic runs to ensure visuals & data are set
            cleanable.Initialize(setting.dirtData, setting.startDirtiness);

            // Track the spawn settings keyed by the instance id so regeneration can look them up later
            int id = cleanable.GetInstanceID();
            if (!activeSpawns.ContainsKey(id))
            {
                activeSpawns.Add(id, setting); // store mapping for future regeneration - short
            }
        }
    }

    /// <summary>
    /// Callback invoked when a dirt object is fully cleaned. If the original spawn
    /// settings request auto-regeneration, schedule the regeneration coroutine. // short
    /// </summary>
    private void HandleDirtCleaned(int id)
    {
        if (activeSpawns.ContainsKey(id))
        {
            DirtSpawnSettings setting = activeSpawns[id];

            if (setting.autoRegenerate)
            {
                StartCoroutine(RegenerateRoutine(id, setting.regenerationDelay, setting.startDirtiness)); // schedule regeneration - short
            }
        }
    }

    /// <summary>
    /// Waits for the configured delay and requests the CleaningManager to restore the dirt value.
    /// This triggers the usual manager flow that will update visuals and internal state. // short
    /// </summary>
    private IEnumerator RegenerateRoutine(int id, float delay, float targetValue)
    {
        yield return new WaitForSeconds(delay); // wait before regeneration - short

        // Request the authoritative system to restore dirt for the given instance id
        CleaningManager.Instance.RestoreDirt(id, targetValue);

        Debug.Log($"Dirt {id} regenerated by Spawner."); // debug log for visibility - short
    }
}