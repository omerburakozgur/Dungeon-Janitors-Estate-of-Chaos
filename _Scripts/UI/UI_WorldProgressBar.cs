/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// World-space progress bar that faces the main camera.
/// </summary>
public class UI_WorldProgressBar : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Image progressBarImage;
    [SerializeField] private TextMeshProUGUI percentageText;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (mainCamera != null)
        {
            // Billboard effect: rotate to face camera while keeping the bar upright
            transform.LookAt(transform.position + mainCamera.transform.forward, mainCamera.transform.up);
        }
    }

    /// <summary>
    /// Updates the visual fill and percentage text.
    /// </summary>
    /// <param name="currentDirtValue">Normalized value (0.0 = clean, 1.0 = dirty).</param>
    public void UpdateBar(float currentDirtValue)
    {
        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = currentDirtValue;
        }

        if (percentageText != null)
        {
            int percentage = Mathf.RoundToInt(currentDirtValue * 100f);
            percentageText.text = $"{percentage}%";
        }

        gameObject.SetActive(currentDirtValue > 0.001f);
    }
}