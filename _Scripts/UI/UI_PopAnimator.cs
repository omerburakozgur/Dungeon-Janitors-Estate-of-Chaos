using UnityEngine;
using DG.Tweening;

/// <summary>
/// A generic, reusable UI animator component.
/// Handles cartoony pop-in, punch scales, fading, auto-hiding, and listens to the Tool Wheel.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UI_PopAnimator : MonoBehaviour
{
    [Header("Animation Settings (DOTween)")]
    [Tooltip("Duration of the fade animation.")]
    [SerializeField] private float fadeDuration = 0.3f;
    [Tooltip("Time before the UI element automatically hides.")]
    [SerializeField] private float hideDelay = 2.0f;

    [Tooltip("The size of the UI when it is completely hidden.")]
    public float hiddenScale = 0.3f;
    [Tooltip("The normal size of the UI when it is fully visible on screen.")]
    public float visibleScale = 1.0f;
    [Tooltip("How much the UI overshoots its target scale during the pop animation.")]
    public float popOvershoot = 1.15f;
    [Tooltip("How intensely the UI punches/pulses when a new item is collected while already visible.")]
    public float punchAmount = 0.15f;

    private CanvasGroup canvasGroup;
    private float hideTimer = 0f;
    private bool isVisible = false;
    private bool isForcedOpen = false;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        transform.localScale = Vector3.one * hiddenScale;
    }

    private void OnEnable()
    {
        ToolWheelManager.OnWheelStateChanged += HandleWheelState;
    }

    private void OnDisable()
    {
        ToolWheelManager.OnWheelStateChanged -= HandleWheelState;
        canvasGroup.DOKill();
        transform.DOKill();
    }

    private void HandleWheelState(bool isOpen)
    {
        isForcedOpen = isOpen;
        if (isOpen) ShowCartoony();
        else hideTimer = hideDelay;
    }

    /// <summary>
    /// Triggers a visual feedback animation.
    /// </summary>
    public void TriggerPopOrPunch()
    {
        hideTimer = hideDelay;

        if (!isVisible)
        {
            ShowCartoony();
        }
        else if (!isForcedOpen)
        {
            // Pulse if already visible and not forced open by TAB
            transform.DOKill(true);
            transform.localScale = Vector3.one * visibleScale;
            transform.DOPunchScale(Vector3.one * punchAmount, 0.2f, 5, 1f).SetUpdate(true);
        }
    }

    private void ShowCartoony()
    {
        isVisible = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).SetEase(Ease.OutQuad);

        transform.DOKill(true);
        transform.localScale = Vector3.one * hiddenScale;
        transform.DOScale(Vector3.one * visibleScale, fadeDuration).SetUpdate(true).SetEase(Ease.OutBack, popOvershoot);
    }

    private void HideCartoony()
    {
        isVisible = false;
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).SetEase(Ease.InQuad);

        transform.DOKill(true);
        transform.DOScale(Vector3.one * hiddenScale, fadeDuration).SetUpdate(true).SetEase(Ease.InBack);
    }

    private void Update()
    {
        if (isVisible && !isForcedOpen)
        {
            hideTimer -= Time.unscaledDeltaTime;
            if (hideTimer <= 0f) HideCartoony();
        }
    }
}