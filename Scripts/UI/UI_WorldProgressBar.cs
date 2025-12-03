// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// World-space progress bar that faces the main camera and displays a dirt/clean percentage.
/// Intended to be driven by Cleanable objects calling UpdateBar.
/// </summary>
public class UI_WorldProgressBar : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image progressBarImage; // Image used for fill amount
    [SerializeField] private TextMeshProUGUI percentageText; // Percentage label

    private Camera mainCamera; // Cached main camera reference

    private void Start()
    {
        mainCamera = Camera.main; // Cache main camera
    }

    /// <summary>
    /// Billboard logic executed after Update: rotate to face camera while preserving up vector.
    /// </summary>
    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Rotate only around the up axis so the bar remains upright
            transform.LookAt(transform.position + mainCamera.transform.forward, mainCamera.transform.up);
        }
    }

    /// <summary>
    /// Update the visual fill and percentage text.
    /// </summary>
    /// <param name="currentDirtValue">Normalized value where1.0 = fully dirty,0.0 = fully clean.</param>
    public void UpdateBar(float currentDirtValue)
    {
        //1) Update fill image if assigned
        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = currentDirtValue; // Set Image.fillAmount (0-1)
        }

        //2) Update percentage text if assigned
        if (percentageText != null)
        {
            int percentage = Mathf.RoundToInt(currentDirtValue * 100f);
            percentageText.text = $"{percentage}%"; // Display as percent
        }

        //3) Toggle visibility: hide when nearly zero
        gameObject.SetActive(currentDirtValue > 0.001f);
    }
}