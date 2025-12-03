// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a charge bar for the throw mechanic. Listens to a FloatEventChannelSO
/// that provides a normalized charge value [0,1].
/// </summary>
public class UI_ThrowChargeBar : MonoBehaviour
{
    [Header("Listening To")]
    [SerializeField] private FloatEventChannelSO onThrowChargeChanged; // Event from PhysicsCarryManager

    [Header("Visuals")]
    [SerializeField] private GameObject barContainer; // Root container (background + fill)
    [SerializeField] private Image fillImage; // Image with Fill type to show charge

    private void OnEnable()
    {
        if (onThrowChargeChanged != null) onThrowChargeChanged.OnEventRaised += UpdateChargeVisual; // Subscribe
        if (barContainer != null) barContainer.SetActive(false); // Start hidden
    }
    private void OnDisable()
    {
        if (onThrowChargeChanged != null) onThrowChargeChanged.OnEventRaised -= UpdateChargeVisual; // Unsubscribe
    }

    private void UpdateChargeVisual(float chargeAmount)
    {
        if (barContainer == null || fillImage == null) return; // Guard

        if (chargeAmount <=0.01f)
        {
            barContainer.SetActive(false); // Hide when near zero
        }
        else
        {
            barContainer.SetActive(true);
            fillImage.fillAmount = chargeAmount; // Update fill
        }
    }
}