/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles visual updates for the health bar UI.
/// </summary>
public class UI_HealthBar : MonoBehaviour
{
    [Header("Listening To")]
    [SerializeField] private FloatEventChannelSO onHealthChanged;

    [Header("Visuals - Bar")]
    [Tooltip("The filled image component representing health.")]
    [SerializeField] private Image healthFillImage;

    [Tooltip("Gradient for health color (Right = Healthy, Left = Low).")]
    [SerializeField] private Gradient healthGradient;

    [Tooltip("Smoothing speed for the health bar lerp.")]
    [SerializeField] private float updateSpeed = 5f;

    [Header("Visuals - Text")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private bool showPercentageSign = true;

    private float targetFill = 1f;

    private void OnEnable()
    {
        if (onHealthChanged != null)
            onHealthChanged.OnEventRaised += UpdateTarget;

        targetFill = 1f;
        if (healthFillImage)
        {
            healthFillImage.fillAmount = 1f;
            UpdateColor(1f);
        }
        UpdateText(1f);
    }

    private void OnDisable()
    {
        if (onHealthChanged != null)
            onHealthChanged.OnEventRaised -= UpdateTarget;
    }

    private void UpdateTarget(float ratio)
    {
        targetFill = ratio;
    }

    private void Update()
    {
        if (healthFillImage == null) return;

        if (Mathf.Abs(healthFillImage.fillAmount - targetFill) > 0.001f)
        {
            healthFillImage.fillAmount = Mathf.Lerp(healthFillImage.fillAmount, targetFill, Time.deltaTime * updateSpeed);
            UpdateColor(healthFillImage.fillAmount);
            UpdateText(healthFillImage.fillAmount);
        }
    }

    private void UpdateColor(float ratio)
    {
        if (healthGradient != null)
        {
            healthFillImage.color = healthGradient.Evaluate(ratio);
        }
    }

    private void UpdateText(float ratio)
    {
        if (healthText != null)
        {
            int percent = Mathf.RoundToInt(ratio * 100f);

            if (showPercentageSign)
                healthText.text = $"%{percent}";
            else
                healthText.text = $"{percent}";
        }
    }
}