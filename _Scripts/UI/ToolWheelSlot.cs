/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Represents a single slot in the tool wheel, handling hover state and visual animations.
/// </summary>
public class ToolWheelSlot : MonoBehaviour
{
    [Header("Data")]
    public PlayerToolManager.ToolType toolCategory;
    public CleaningToolType specificCleaningType;

    [Tooltip("The name of the tool that will be displayed in the center of the wheel.")]
    public string toolDisplayName;

    [Header("Visuals")]
    [SerializeField] private Image slotBackground;
    [SerializeField] private Color normalColor = new Color(0, 0, 0, 0.5f);
    [SerializeField] private Color highlightedColor = new Color(1f, 0.8f, 0.2f, 0.9f);

    [Header("Animation Settings")]
    [SerializeField] private float hoverScale = 1.25f;
    [SerializeField] private float animDuration = 0.2f;

    private void Start()
    {
        Unhover(true);
    }

    /// <summary>
    /// Applies visual highlight effects when the slot is selected.
    /// </summary>
    public void Hover()
    {
        transform.DOKill();
        if (slotBackground) slotBackground.DOKill();

        transform.DOScale(Vector3.one * hoverScale, animDuration).SetEase(Ease.OutBack).SetUpdate(true);
        if (slotBackground) slotBackground.DOColor(highlightedColor, animDuration).SetUpdate(true);
    }

    /// <summary>
    /// Reverts visual effects when the slot is no longer selected.
    /// </summary>
    /// <param name="instant">If true, skip the transition animation.</param>
    public void Unhover(bool instant = false)
    {
        transform.DOKill();
        if (slotBackground) slotBackground.DOKill();

        if (instant)
        {
            transform.localScale = Vector3.one;
            if (slotBackground) slotBackground.color = normalColor;
        }
        else
        {
            transform.DOScale(Vector3.one, animDuration).SetUpdate(true);
            if (slotBackground) slotBackground.DOColor(normalColor, animDuration).SetUpdate(true);
        }
    }
}