using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages the visual progression bar for cleaning tasks.
/// </summary>
[RequireComponent(typeof(UI_PopAnimator))]
public class UI_CleaningProgress : MonoBehaviour
{
    [Tooltip("Event channel triggered when dirt is cleaned.")]
    [SerializeField] private FloatEventChannelSO onDirtCleanedAmountEvent;

    [Tooltip("The UI Image component acting as the progress bar.")]
    [SerializeField] private Image progressBarImage;

    [Tooltip("Gradient to transition the bar color based on progress.")]
    [SerializeField] private Gradient progressGradient;

    [Tooltip("Smoothing speed for the progress bar animation.")]
    [SerializeField] private float updateSpeed = 5f;

    [Tooltip("Text component displaying percentage.")]
    [SerializeField] private TextMeshProUGUI percentageText;

    [Tooltip("Threshold at which the text changes color.")]
    [SerializeField] private float LDD_CLEAN_GOAL = 0.80f;

    [Tooltip("Total amount of dirt required to complete the level.")]
    [SerializeField] private float TOTAL_LEVEL_DIRT = 100f;

    private UI_PopAnimator popAnimator;
    private float totalCleanedAmount = 0f;
    private float targetProgress = 0f;

    private void Awake()
    {
        popAnimator = GetComponent<UI_PopAnimator>();
        if (progressBarImage)
        {
            progressBarImage.fillAmount = 0f;
            UpdateColor(0f);
        }
        UpdateText(0f);
    }

    private void OnEnable()
    {
        if (onDirtCleanedAmountEvent) onDirtCleanedAmountEvent.OnEventRaised += AddProgress;
    }

    private void OnDisable()
    {
        if (onDirtCleanedAmountEvent) onDirtCleanedAmountEvent.OnEventRaised -= AddProgress;
    }

    private void AddProgress(float amountCleaned)
    {
        totalCleanedAmount += amountCleaned;
        targetProgress = Mathf.Clamp01(totalCleanedAmount / TOTAL_LEVEL_DIRT);
        popAnimator.TriggerPopOrPunch();
    }

    private void Update()
    {
        if (progressBarImage != null && Mathf.Abs(progressBarImage.fillAmount - targetProgress) > 0.001f)
        {
            float currentFill = Mathf.Lerp(progressBarImage.fillAmount, targetProgress, Time.unscaledDeltaTime * updateSpeed);
            progressBarImage.fillAmount = currentFill;
            UpdateColor(currentFill);
            UpdateText(currentFill);
        }
    }

    private void UpdateColor(float ratio)
    {
        if (progressGradient != null) progressBarImage.color = progressGradient.Evaluate(ratio);
    }

    private void UpdateText(float ratio)
    {
        if (percentageText)
        {
            percentageText.text = $"{Mathf.RoundToInt(ratio * 100f)}%";
            if (ratio >= LDD_CLEAN_GOAL) percentageText.color = Color.green;
        }
    }
}