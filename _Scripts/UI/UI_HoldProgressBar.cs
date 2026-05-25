/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple progress bar for hold-interaction actions.
/// </summary>
public class UI_HoldProgressBar : MonoBehaviour
{
    [Header("UI References")]
    public Image progressBarImage;

    [Header("Listening To")]
    public FloatEventChannelSO onProgressEvent;

    private void OnEnable()
    {
        if (onProgressEvent != null)
            onProgressEvent.OnEventRaised += UpdateProgressBar;
    }

    private void OnDisable()
    {
        if (onProgressEvent != null)
            onProgressEvent.OnEventRaised -= UpdateProgressBar;
    }

    private void Start()
    {
        if (progressBarImage != null) progressBarImage.fillAmount = 0f;
    }

    private void UpdateProgressBar(float fillRatio)
    {
        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = fillRatio;

            if (fillRatio <= 0f || fillRatio >= 1f)
            {
                progressBarImage.fillAmount = 0f;
            }
        }
    }
}