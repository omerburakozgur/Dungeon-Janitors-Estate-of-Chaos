// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using TMPro;
using UnityEngine;

/// <summary>
/// Example UI manager that listens to salvage updates and displays the value.
/// </summary>
public class UIManagerExample : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO onSalvageUpdatedChannel; // Channel reference
    [SerializeField] private TextMeshProUGUI salvageText; // UI text element

    private void OnEnable()
    {
        // Subscribe to channel
        onSalvageUpdatedChannel.OnEventRaised += UpdateSalvageText;
    }
    private void OnDisable()
    {
        // Unsubscribe
        onSalvageUpdatedChannel.OnEventRaised -= UpdateSalvageText;
    }
    private void UpdateSalvageText(int newAmount)
    {
        salvageText.text = $"Salvage: {newAmount}"; // Update UI
    }
}