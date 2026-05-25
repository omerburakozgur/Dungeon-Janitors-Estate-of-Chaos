/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using TMPro;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Listener that receives prompt text via a StringEventChannelSO and updates
/// a child TextMeshProUGUI element. Handles highly customizable scale pop and fade animations.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UI_InteractionPrompt : MonoBehaviour
{
    [Header("Listening")]
    [SerializeField] private StringEventChannelSO onInteractableHovered;

    [Header("Target Visuals")]
    [Tooltip("The text element displaying the prompt (e.g., 'E - Clean').")]
    [SerializeField] private TextMeshProUGUI promptTextElement;

    [Tooltip("The RectTransform that will be scaled. Usually the prompt text itself or its background container.")]
    [SerializeField] private RectTransform containerRect;

    [Tooltip("Automatically fetched if left empty. Used to fade the prompt in and out smoothly.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation Settings - Timings")]
    [Tooltip("Duration of the entire show/hide animation in seconds.")]
    [SerializeField] private float animationDuration = 0.25f;

    [Header("Animation Settings - Scale (Pop Effect)")]
    [Tooltip("The scale multiplier when the prompt first appears (before popping up).")]
    [SerializeField] private float startScale = 0.5f;
    [Tooltip("The final scale multiplier when the prompt is fully visible.")]
    [SerializeField] private float targetScale = 1.05f;
    [Tooltip("The easing curve used when the prompt scales up.")]
    [SerializeField] private Ease scaleInEase = Ease.OutBack;
    [Tooltip("The easing curve used when the prompt scales down and disappears.")]
    [SerializeField] private Ease scaleOutEase = Ease.InBack;

    [Header("Animation Settings - Fade")]
    [Tooltip("The final alpha (opacity) value when the prompt is fully visible.")]
    [Range(0f, 1f)]
    [SerializeField] private float targetAlpha = 1f;
    [Tooltip("The easing curve used for the fade-in effect.")]
    [SerializeField] private Ease fadeInEase = Ease.OutQuad;
    [Tooltip("The easing curve used for the fade-out effect.")]
    [SerializeField] private Ease fadeOutEase = Ease.InQuad;

    private bool isShowing = false;
    private string currentMessage = "";

    private void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (containerRect == null && promptTextElement != null)
            containerRect = promptTextElement.GetComponent<RectTransform>();

        if (canvasGroup != null) canvasGroup.alpha = 0f;
        if (containerRect != null) containerRect.localScale = Vector3.zero;
    }

    private void OnEnable()
    {
        if (onInteractableHovered != null) onInteractableHovered.OnEventRaised += UpdatePrompt;
    }

    private void OnDisable()
    {
        if (onInteractableHovered != null) onInteractableHovered.OnEventRaised -= UpdatePrompt;
    }

    /// <summary>
    /// Evaluates the incoming text and triggers the appropriate customizable DOTween animation.
    /// Empty strings trigger the hide animation.
    /// </summary>
    /// <param name="newText">The interaction prompt string</param>
    private void UpdatePrompt(string newText)
    {
        if (promptTextElement == null || canvasGroup == null || containerRect == null) return;

        if (string.IsNullOrEmpty(newText))
        {
            if (!isShowing) return;

            isShowing = false;
            currentMessage = "";

            canvasGroup.DOKill();
            containerRect.DOKill();

            canvasGroup.DOFade(0f, animationDuration).SetEase(fadeOutEase);
            containerRect.DOScale(Vector3.zero, animationDuration).SetEase(scaleOutEase);
        }
        else
        {
            if (isShowing && currentMessage == newText) return;

            isShowing = true;
            currentMessage = newText;
            promptTextElement.text = newText;

            canvasGroup.DOKill();
            containerRect.DOKill();

            canvasGroup.DOFade(targetAlpha, animationDuration).SetEase(fadeInEase);

            containerRect.localScale = Vector3.one * startScale;
            containerRect.DOScale(targetScale, animationDuration).SetEase(scaleInEase);
        }
    }
}