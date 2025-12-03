// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Listens to cleaning progress events and updates a UI progress bar and percentage text.
/// </summary>
public class UI_CleaningProgress : MonoBehaviour
{
 [Header("Listening To")]
 [SerializeField] private FloatEventChannelSO onDirtCleanedAmountEvent; // Raised by CleaningManager

 [Header("Visuals")]
 [SerializeField] private Image progressBarImage; // Image used for fill amount
 [SerializeField] private TextMeshProUGUI percentageText; // Percentage display

 [Header("Settings")]
 [SerializeField] private float LDD_CLEAN_GOAL =0.80f; // Goal threshold
 [SerializeField] private float TOTAL_LEVEL_DIRT =100f; // Placeholder total amount

 private float totalCleanedAmount =0f; // Accumulated cleaned amount
 private void Start() { UpdateProgress(0f); }

 private void OnEnable()
 {
 if (onDirtCleanedAmountEvent != null) onDirtCleanedAmountEvent.OnEventRaised += UpdateProgress;
 }
 private void OnDisable()
 {
 if (onDirtCleanedAmountEvent != null) onDirtCleanedAmountEvent.OnEventRaised -= UpdateProgress;
 }

 /// <summary>
 /// Add the provided cleaned amount to the total and update visuals.
 /// </summary>
 private void UpdateProgress(float amountCleaned)
 {
 totalCleanedAmount += amountCleaned;
 float progress = totalCleanedAmount / TOTAL_LEVEL_DIRT; // Normalized [0,1]

 if (progressBarImage != null) progressBarImage.fillAmount = progress; // Update fill
 if (percentageText != null) percentageText.text = $"{Mathf.Round(progress *100f)}%"; // Update text

 // Check for goal reached (integration point with LevelManager)
 if (progress >= LDD_CLEAN_GOAL)
 {
 // TODO: Notify LevelManager that goal has been reached
 }
 }
}