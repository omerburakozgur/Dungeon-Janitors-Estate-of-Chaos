/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;

/// <summary>
/// Updates the UI counter when trash collection state changes.
/// </summary>
[RequireComponent(typeof(UI_PopAnimator))]
public class UI_TrashCounter : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO onTrashCountChanged;
    [SerializeField] private TextMeshProUGUI counterText;

    private UI_PopAnimator popAnimator;

    private void Awake()
    {
        popAnimator = GetComponent<UI_PopAnimator>();
        if (counterText != null) counterText.text = "0";
    }

    private void OnEnable()
    {
        if (onTrashCountChanged) onTrashCountChanged.OnEventRaised += UpdateCounter;
    }

    private void OnDisable()
    {
        if (onTrashCountChanged) onTrashCountChanged.OnEventRaised -= UpdateCounter;
    }

    private void UpdateCounter(int currentCount)
    {
        if (counterText != null) counterText.text = currentCount.ToString();
        popAnimator.TriggerPopOrPunch();
    }
}