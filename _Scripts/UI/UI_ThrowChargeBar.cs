/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a charge bar for the throw mechanic. Listens to a FloatEventChannelSO.
/// </summary>
public class UI_ThrowChargeBar : MonoBehaviour
{
    [Header("Listening To")]
    [Tooltip("Event channel representing the current throw charge (0-1).")]
    [SerializeField] private FloatEventChannelSO onThrowChargeChanged;

    [Header("Visuals")]
    [SerializeField] private GameObject barContainer;
    [SerializeField] private Image fillImage;

    private void OnEnable()
    {
        if (onThrowChargeChanged != null) onThrowChargeChanged.OnEventRaised += UpdateChargeVisual;
        if (barContainer != null) barContainer.SetActive(false);
    }

    private void OnDisable()
    {
        if (onThrowChargeChanged != null) onThrowChargeChanged.OnEventRaised -= UpdateChargeVisual;
    }

    private void UpdateChargeVisual(float chargeAmount)
    {
        if (barContainer == null || fillImage == null) return;

        if (chargeAmount <= 0.01f)
        {
            barContainer.SetActive(false);
        }
        else
        {
            barContainer.SetActive(true);
            fillImage.fillAmount = chargeAmount;
        }
    }
}