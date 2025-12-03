// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Simple player salvage inventory. Broadcasts salvage updates through an IntEventChannelSO.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // Inspector'da onSalvageUpdatedChannelSO'yu sürükle
    [SerializeField] private IntEventChannelSO onSalvageUpdatedChannel; // Channel to broadcast updates
    private int currentSalvage; // Cached salvage amount

    /// <summary>
    /// Add salvage and notify listeners via the configured channel.
    /// </summary>
    /// <param name="amount">Amount to add to salvage.</param>
    public void AddSalvage(int amount)
    {
        currentSalvage += amount;
        onSalvageUpdatedChannel.Raise(currentSalvage); // Publish change
    }
}