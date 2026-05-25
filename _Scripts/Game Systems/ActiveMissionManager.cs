/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System;

/// <summary>
/// Manages active mission objectives, progress, and rewards based on the current contract.
/// </summary>
public class ActiveMissionManager : SingletonManager<ActiveMissionManager>
{
    public static LevelDataSO CurrentContract;

    [Header("Mission Progress")]
    public int currentCleanedDirt = 0;
    public int currentCollectedTrash = 0;
    public bool isMissionComplete = false;

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO onTrashCollectedChannel;

    public event Action<int, int> OnObjectiveUpdated;
    public event Action OnMissionComplete;

    private void Start()
    {
        if (CurrentContract == null) return;

        DirtSpawner dirtSpawner = FindObjectOfType<DirtSpawner>();
        if (dirtSpawner != null)
        {
            dirtSpawner.SpawnAllDirts(CurrentContract.proceduralDirtCount);
        }

        TrashSpawner trashSpawner = FindObjectOfType<TrashSpawner>();
        if (trashSpawner != null)
        {
            trashSpawner.SpawnTrash(CurrentContract.proceduralTrashCount);
        }

        OnObjectiveUpdated?.Invoke(currentCleanedDirt, currentCollectedTrash);
    }

    private void OnEnable()
    {
        if (CleaningManager.Instance != null) CleaningManager.Instance.OnDirtCleanedWithID += HandleDirtCleaned;
        if (onTrashCollectedChannel != null) onTrashCollectedChannel.OnEventRaised += HandleTrashCollected;
    }

    private void OnDisable()
    {
        if (CleaningManager.Instance != null) CleaningManager.Instance.OnDirtCleanedWithID -= HandleDirtCleaned;
        if (onTrashCollectedChannel != null) onTrashCollectedChannel.OnEventRaised -= HandleTrashCollected;
    }

    private void HandleDirtCleaned(int instanceId)
    {
        if (CurrentContract == null) return;
        currentCleanedDirt++;

        if (!isMissionComplete)
            CheckWinCondition();
        else if (LevelManager.Instance != null)
            LevelManager.Instance.AddSessionLoot(CurrentContract.bonusGoldPerExtraDirt, 0);

        OnObjectiveUpdated?.Invoke(currentCleanedDirt, currentCollectedTrash);
    }

    private void HandleTrashCollected()
    {
        if (CurrentContract == null) return;
        currentCollectedTrash++;

        if (!isMissionComplete)
            CheckWinCondition();
        else if (LevelManager.Instance != null)
            LevelManager.Instance.AddSessionLoot(CurrentContract.bonusGoldPerExtraTrash, CurrentContract.bonusSalvagePerExtraTrash);

        OnObjectiveUpdated?.Invoke(currentCleanedDirt, currentCollectedTrash);
    }

    private void CheckWinCondition()
    {
        if (currentCleanedDirt >= CurrentContract.requiredDirtToClean &&
            currentCollectedTrash >= CurrentContract.requiredTrashToCollect)
        {
            isMissionComplete = true;
            Debug.Log("<color=green>[ActiveMissionManager] MISSION ACCOMPLISHED!</color>");

            if (LevelManager.Instance != null)
                LevelManager.Instance.AddSessionLoot(CurrentContract.baseGoldReward, CurrentContract.baseSalvageReward);

            OnMissionComplete?.Invoke();
        }
    }

    private void CheckObjectives()
    {
        OnObjectiveUpdated?.Invoke(currentCleanedDirt, currentCollectedTrash);

        if (currentCleanedDirt >= CurrentContract.requiredDirtToClean &&
            currentCollectedTrash >= CurrentContract.requiredTrashToCollect)
        {
            isMissionComplete = true;
            OnMissionComplete?.Invoke();
        }
    }

    /// <summary>
    /// Calculates the final end-of-mission rewards, including base and bonus amounts.
    /// </summary>
    /// <param name="totalGold">Calculated total gold.</param>
    /// <param name="totalSalvage">Calculated total salvage.</param>
    public void CalculateFinalRewards(out int totalGold, out int totalSalvage)
    {
        totalGold = 0;
        totalSalvage = 0;

        if (CurrentContract == null) return;

        if (isMissionComplete)
        {
            totalGold += CurrentContract.baseGoldReward;
            totalSalvage += CurrentContract.baseSalvageReward;

            int extraDirt = Mathf.Max(0, currentCleanedDirt - CurrentContract.requiredDirtToClean);
            int extraTrash = Mathf.Max(0, currentCollectedTrash - CurrentContract.requiredTrashToCollect);

            totalGold += (extraDirt * CurrentContract.bonusGoldPerExtraDirt) + (extraTrash * CurrentContract.bonusGoldPerExtraTrash);
            totalSalvage += (extraTrash * CurrentContract.bonusSalvagePerExtraTrash);

            Debug.Log($"[MissionManager] Base: {CurrentContract.baseGoldReward}G | Bonus Dirt: {extraDirt} | Bonus Trash: {extraTrash} | Final Gold: {totalGold}");
        }
        else
        {
            Debug.Log("[MissionManager] Mission aborted! No contract rewards given.");
        }
    }
}