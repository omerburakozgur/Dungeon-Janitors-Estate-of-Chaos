// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Authoritative manager that tracks and updates dirtiness across registered cleanables.
/// Exposes registration API, processes cleaning requests, supports passive regeneration
/// and raises events for UI/VFX systems.
/// </summary>
public class CleaningManager : SingletonManager<CleaningManager>
{
    [Header("Debug")] // Inspector: debug options
    [SerializeField] private bool debugMode = true; // Toggle debug logging

    [Header("State")] // Runtime state containers
    // Maps instance ID -> normalized dirtiness value (0..1)
    private Dictionary<int, float> dirtinessDatabase = new Dictionary<int, float>(); // authoritative state
    // Tracks last value sent to visuals to avoid frequent updates
    private Dictionary<int, float> lastVisualUpdateValue = new Dictionary<int, float>(); // last visual push
    // Lookup of instance ID -> CleanableBase for visual updates & collider control
    private Dictionary<int, CleanableBase> localDecalLookup = new Dictionary<int, CleanableBase>(); // reference map

    [Header("Events")] // Event channels
    [SerializeField] private FloatEventChannelSO onDirtCleanedAmountEvent; // Raised with delta when dirt changes
    [SerializeField] private VoidEventChannelSO onDecalFullyCleanedEvent; // Raised when a decal reaches zero dirt

    public event System.Action<int> OnDirtCleanedWithID; // Listeners receive ID when an item is fully cleaned

    private const float VISUAL_UPDATE_THRESHOLD = 0.05f; // Threshold to reduce visual update frequency

    protected override void Awake()
    {
        base.Awake();
        // Recreate dictionaries on awake to ensure clean state after domain reloads
        dirtinessDatabase = new Dictionary<int, float>();
        lastVisualUpdateValue = new Dictionary<int, float>();
        localDecalLookup = new Dictionary<int, CleanableBase>();
    }

    // --- Registration API ---
    /// <summary>
    /// Register a CleanableBase instance with an initial dirt value.
    /// Uses Unity instance ID as the key.
    /// </summary>
    public void RegisterCleanable(CleanableBase decalScript, float initialDirtiness)
    {
        if (decalScript == null) return; // Guard
        int id = decalScript.GetInstanceID(); // Unique key per instance

        if (!dirtinessDatabase.ContainsKey(id))
        {
            dirtinessDatabase.Add(id, initialDirtiness); // add authoritative value
            localDecalLookup.Add(id, decalScript); // track reference
            lastVisualUpdateValue.Add(id, initialDirtiness); // initialize visual tracker
            if (debugMode) Debug.Log($"[CleaningManager] REGISTER: {decalScript.name} (ID: {id}) registered.");
        }
        else
        {
            // Re-register: update stored values & references
            dirtinessDatabase[id] = initialDirtiness;
            localDecalLookup[id] = decalScript;
            if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue[id] = initialDirtiness;
            if (debugMode) Debug.Log($"[CleaningManager] RE-REGISTER: {decalScript.name} updated.");
        }
    }

    /// <summary>
    /// Unregister a cleanable and remove all associated tracking state.
    /// </summary>
    public void UnregisterCleanable(CleanableBase cleanable)
    {
        if (cleanable == null) return; // Guard
        int id = cleanable.GetInstanceID();
        if (dirtinessDatabase.ContainsKey(id)) dirtinessDatabase.Remove(id);
        if (localDecalLookup.ContainsKey(id)) localDecalLookup.Remove(id);
        if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue.Remove(id);
    }

    // --- Passive regeneration and visual synchronization ---
    private void Update()
    {
        if (dirtinessDatabase.Count == 0) return; // nothing to process

        foreach (var kvp in localDecalLookup)
        {
            int id = kvp.Key;
            CleanableBase stain = kvp.Value;

            // Skip invalid stains or those without a buildup rate
            if (stain == null || stain.dirtData == null || stain.dirtData.stainBuildupRate <= 0) continue;
            if (!dirtinessDatabase.ContainsKey(id)) continue; // ensure authoritative record exists

            float currentDirt = dirtinessDatabase[id];
            if (currentDirt <= 0 || currentDirt >= 1.0f) continue; // only regenerate when strictly between bounds

            // Increment dirt by the configured buildup rate (per second)
            float regenAmount = stain.dirtData.stainBuildupRate * Time.deltaTime; // amount this frame
            float newDirt = Mathf.Min(1.0f, currentDirt + regenAmount); // clamp to1.0

            float progressLoss = currentDirt - newDirt; // negative when increasing dirt
            if (Mathf.Abs(progressLoss) > 0.0001f && onDirtCleanedAmountEvent != null)
                onDirtCleanedAmountEvent.Raise(progressLoss); // notify listeners of delta

            dirtinessDatabase[id] = newDirt; // persist updated dirtiness

            float lastVisual = lastVisualUpdateValue.ContainsKey(id) ? lastVisualUpdateValue[id] : currentDirt;
            if (Mathf.Abs(newDirt - lastVisual) >= VISUAL_UPDATE_THRESHOLD)
            {
                // Push visual update to the CleanableBase instance
                stain.UpdateVisuals(newDirt);
                if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue[id] = newDirt;
                else lastVisualUpdateValue.Add(id, newDirt);
            }
        }
    }

    /// <summary>
    /// Restore dirt on a previously registered instance to the specified amount.
    /// Updates authoritative storage, visuals and re-enables colliders where appropriate.
    /// </summary>
    public void RestoreDirt(int id, float amount)
    {
        if (!dirtinessDatabase.ContainsKey(id)) return; // unknown id
        float previousDirt = dirtinessDatabase[id];
        float dirtRestored = amount - previousDirt; // positive when dirt added
        if (onDirtCleanedAmountEvent != null) onDirtCleanedAmountEvent.Raise(-dirtRestored); // negative raise to indicate added dirt
        dirtinessDatabase[id] = amount; // persist authoritative value

        if (localDecalLookup.ContainsKey(id))
        {
            localDecalLookup[id].UpdateVisuals(amount); // update world visuals to match authoritative state
            if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue[id] = amount;
            Collider col = localDecalLookup[id].GetComponent<Collider>();
            if (col != null) col.enabled = true; // re-enable collider when dirt restored
        }
    }

    // --- Processing cleaning requests (authoritative) ---
    /// <summary>
    /// Processes a cleaning request made against a target using a specified tool.
    /// Validates tool compatibility, computes the cleaning amount and updates state.
    /// Triggers events and VFX when an object reaches fully-cleaned state.
    /// </summary>
    public void RequestClean(CleanableBase target, ToolDataSO tool)
    {
        //1. Null Check
        if (target == null || tool == null)
        {
            if (debugMode) Debug.LogError("[CleaningManager] ERROR: Target or Tool is NULL!");
            return;
        }

        int id = target.GetInstanceID(); // resolve instance id

        //2. Ensure entry exists in the database (lazy registration)
        if (!dirtinessDatabase.ContainsKey(id))
        {
            if (debugMode) Debug.LogWarning($"[CleaningManager] WARNING: {target.name} not in database. Registering with initial value...");
            RegisterCleanable(target, target.initialDirtiness);
        }

        //3. Tool type compatibility check
        if (target.dirtData.requiredToolType != tool.specificCleaningType)
        {
            if (debugMode) Debug.LogError($"[CleaningManager] REJECTED (Type Mismatch): Stain requires '{target.dirtData.requiredToolType}', tool is '{tool.specificCleaningType}'.");
            return; // reject incompatible tool
        }

        //4. Current state check
        float currentDirt = dirtinessDatabase[id];
        if (currentDirt <= 0)
        {
            if (debugMode) Debug.Log($"[CleaningManager] CANCELLED: {target.name} already fully clean.");
            return; // nothing to do
        }

        //5. Compute cleaning applied this frame and persist
        float resistance = target.dirtData.resistance > 0 ? target.dirtData.resistance : 1f; // avoid division by zero
        float cleanPower = (tool.cleanSpeed / resistance) * Time.deltaTime; // cleaning amount this frame
        float newDirt = Mathf.Max(0f, currentDirt - cleanPower); // ensure floor at zero

        dirtinessDatabase[id] = newDirt; // persist change

        if (localDecalLookup.ContainsKey(id))
        {
            localDecalLookup[id].UpdateVisuals(newDirt); // update visuals immediately
            if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue[id] = newDirt;
        }

        if (onDirtCleanedAmountEvent != null) onDirtCleanedAmountEvent.Raise(cleanPower); // broadcast cleaned amount

        if (currentDirt > 0 && newDirt <= 0)
        {
            // Fully cleaned: raise events, spawn VFX and notify subscribers
            if (debugMode) Debug.Log($"[CleaningManager] COMPLETED: {target.name} cleaned!");
            if (onDecalFullyCleanedEvent != null) onDecalFullyCleanedEvent.Raise();

            if (target.dirtData.cleanVFX != null)
                Instantiate(target.dirtData.cleanVFX, target.transform.position, Quaternion.identity); // spawn VFX

            OnDirtCleanedWithID?.Invoke(id); // notify listeners with instance id
        }
    }
}