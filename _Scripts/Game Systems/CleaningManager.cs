/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Authoritative manager that tracks and updates dirtiness across registered cleanables.
/// </summary>
public class CleaningManager : SingletonManager<CleaningManager>
{
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    [Header("State")]
    private Dictionary<int, float> dirtinessDatabase = new Dictionary<int, float>();
    private Dictionary<int, float> lastVisualUpdateValue = new Dictionary<int, float>();
    private Dictionary<int, CleanableBase> localDecalLookup = new Dictionary<int, CleanableBase>();
    private List<int> activeRegenerationList = new List<int>();

    [Header("Events")]
    [SerializeField] private FloatEventChannelSO onDirtCleanedAmountEvent;
    [SerializeField] private VoidEventChannelSO onDecalFullyCleanedEvent;

    public event System.Action<int> OnDirtCleanedWithID;

    private const float VISUAL_UPDATE_THRESHOLD = 0.05f;

    /// <summary>
    /// Initializes all database dictionaries and lists tracking dirt parameters.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        dirtinessDatabase = new Dictionary<int, float>();
        lastVisualUpdateValue = new Dictionary<int, float>();
        localDecalLookup = new Dictionary<int, CleanableBase>();
        activeRegenerationList = new List<int>();
    }

    /// <summary>
    /// Register a CleanableBase instance with an initial dirt value.
    /// </summary>
    /// <param name="decalScript">The cleanable base script to register.</param>
    /// <param name="initialDirtiness">Starting dirtiness percentage (0..1).</param>
    public void RegisterCleanable(CleanableBase decalScript, float initialDirtiness)
    {
        if (decalScript == null) return;
        int id = decalScript.GetInstanceID();

        if (!dirtinessDatabase.ContainsKey(id))
        {
            dirtinessDatabase.Add(id, initialDirtiness);
            localDecalLookup.Add(id, decalScript);
            lastVisualUpdateValue.Add(id, initialDirtiness);

            if (decalScript.dirtData != null && decalScript.dirtData.stainBuildupRate > 0)
            {
                if (!activeRegenerationList.Contains(id))
                    activeRegenerationList.Add(id);
            }
        }
        else
        {
            dirtinessDatabase[id] = initialDirtiness;
            localDecalLookup[id] = decalScript;

            if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue[id] = initialDirtiness;
            else lastVisualUpdateValue.Add(id, initialDirtiness);

            if (decalScript.dirtData != null && decalScript.dirtData.stainBuildupRate > 0)
            {
                if (!activeRegenerationList.Contains(id))
                    activeRegenerationList.Add(id);
            }
        }
    }

    /// <summary>
    /// Unregister a cleanable and remove all associated tracking state.
    /// </summary>
    /// <param name="cleanable">The cleanable instance to unregister.</param>
    public void UnregisterCleanable(CleanableBase cleanable)
    {
        if (cleanable == null) return;
        int id = cleanable.GetInstanceID();

        if (dirtinessDatabase.ContainsKey(id)) dirtinessDatabase.Remove(id);
        if (localDecalLookup.ContainsKey(id)) localDecalLookup.Remove(id);
        if (lastVisualUpdateValue.ContainsKey(id)) lastVisualUpdateValue.Remove(id);

        if (activeRegenerationList.Contains(id)) activeRegenerationList.Remove(id);
    }

    private void Update()
    {
        if (activeRegenerationList.Count == 0) return;

        for (int i = activeRegenerationList.Count - 1; i >= 0; i--)
        {
            int id = activeRegenerationList[i];

            if (!localDecalLookup.ContainsKey(id) || !dirtinessDatabase.ContainsKey(id))
            {
                activeRegenerationList.RemoveAt(i);
                continue;
            }

            CleanableBase stain = localDecalLookup[id];
            float currentDirt = dirtinessDatabase[id];

            if (currentDirt <= 0 || currentDirt >= 1.0f) continue;

            // Frame-Rate Independent Calculation:
            // Amount = Rate (per second) * Time.deltaTime (time passed this frame)
            float regenAmount = stain.dirtData.stainBuildupRate * Time.deltaTime;
            float newDirt = Mathf.Min(1.0f, currentDirt + regenAmount);

            float progressLoss = currentDirt - newDirt;

            if (Mathf.Abs(progressLoss) > 0.0001f && onDirtCleanedAmountEvent != null)
                onDirtCleanedAmountEvent.Raise(progressLoss);

            dirtinessDatabase[id] = newDirt;

            float lastVisual = lastVisualUpdateValue[id];
            if (Mathf.Abs(newDirt - lastVisual) >= VISUAL_UPDATE_THRESHOLD)
            {
                stain.UpdateVisuals(newDirt);
                lastVisualUpdateValue[id] = newDirt;
            }
        }
    }

    /// <summary>
    /// Restores a specific amount of dirtiness to the tracked instance.
    /// </summary>
    /// <param name="id">Instance ID of the dirt.</param>
    /// <param name="amount">The restored value.</param>
    public void RestoreDirt(int id, float amount)
    {
        if (!dirtinessDatabase.ContainsKey(id)) return;

        float previousDirt = dirtinessDatabase[id];
        float dirtRestored = amount - previousDirt;

        if (onDirtCleanedAmountEvent != null) onDirtCleanedAmountEvent.Raise(-dirtRestored);

        dirtinessDatabase[id] = amount;

        if (localDecalLookup.ContainsKey(id))
        {
            CleanableBase stain = localDecalLookup[id];
            stain.UpdateVisuals(amount);
            lastVisualUpdateValue[id] = amount;

            Collider col = stain.GetComponent<Collider>();
            if (col != null) col.enabled = true;

            if (stain.dirtData.stainBuildupRate > 0 && !activeRegenerationList.Contains(id))
            {
                activeRegenerationList.Add(id);
            }
        }
    }

    /// <summary>
    /// Processes a cleaning request. 
    /// Math is frame-rate independent: CleanAmount = (Speed / Resistance) * DeltaTime.
    /// </summary>
    /// <param name="target">The cleanable block.</param>
    /// <param name="tool">The tool data matching the action.</param>
    public void RequestClean(CleanableBase target, ToolDataSO tool)
    {
        if (target == null || tool == null) return;
        int id = target.GetInstanceID();

        if (!dirtinessDatabase.ContainsKey(id))
        {
            RegisterCleanable(target, target.initialDirtiness);
        }

        if (target.dirtData.requiredToolType != tool.specificCleaningType) return;

        float currentDirt = dirtinessDatabase[id];
        if (currentDirt <= 0) return;

        float resistance = target.dirtData.resistance > 0 ? target.dirtData.resistance : 1f;

        float multiplier = 1f;
        if (MetaProgressionManager.Instance != null)
        {
            multiplier = MetaProgressionManager.Instance.GetStatMultiplier(StatType.CleaningSpeed);
        }

        float cleanPower = (tool.cleanSpeed * multiplier / resistance) * Time.deltaTime;
        float newDirt = Mathf.Max(0f, currentDirt - cleanPower);

        dirtinessDatabase[id] = newDirt;

        if (localDecalLookup.ContainsKey(id))
        {
            localDecalLookup[id].UpdateVisuals(newDirt);
            lastVisualUpdateValue[id] = newDirt;
        }

        if (onDirtCleanedAmountEvent != null) onDirtCleanedAmountEvent.Raise(cleanPower);

        if (currentDirt > 0 && newDirt <= 0)
        {
            if (debugMode) Debug.Log($"[CleaningManager] COMPLETED: {target.name} cleaned!");
            if (onDecalFullyCleanedEvent != null) onDecalFullyCleanedEvent.Raise();

            if (target.dirtData.cleanVFX != null)
                Instantiate(target.dirtData.cleanVFX, target.transform.position, Quaternion.identity);

            OnDirtCleanedWithID?.Invoke(id);

            if (activeRegenerationList.Contains(id)) activeRegenerationList.Remove(id);
        }
    }
}