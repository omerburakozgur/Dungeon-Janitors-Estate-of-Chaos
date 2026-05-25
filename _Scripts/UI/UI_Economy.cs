/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;

/// <summary>
/// Manages and displays the economy (Gold and Salvage) in the UI.
/// </summary>
[RequireComponent(typeof(UI_PopAnimator))]
public class UI_Economy : MonoBehaviour
{
    [SerializeField] private IntEventChannelSO onGoldChanged;
    [SerializeField] private IntEventChannelSO onSalvageChanged;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private string goldFormat = "{0} Gold <color=#FFD700>(+{1})</color>";
    [SerializeField] private TextMeshProUGUI salvageText;

    private UI_PopAnimator popAnimator;
    private int safeGold = 0, sessionGold = 0;
    private bool isInitialized = false;

    private void Awake() { popAnimator = GetComponent<UI_PopAnimator>(); }

    private void OnEnable()
    {
        if (onGoldChanged) onGoldChanged.OnEventRaised += UpdateSafeGold;
        if (onSalvageChanged) onSalvageChanged.OnEventRaised += UpdateSalvage;
        if (LevelManager.Instance)
        {
            LevelManager.Instance.OnSessionGoldChanged += UpdateSessionGold;
            UpdateSessionGold(LevelManager.Instance.GetSessionGold(), false);
        }
    }

    private void OnDisable()
    {
        if (onGoldChanged) onGoldChanged.OnEventRaised -= UpdateSafeGold;
        if (onSalvageChanged) onSalvageChanged.OnEventRaised -= UpdateSalvage;
        if (LevelManager.Instance) LevelManager.Instance.OnSessionGoldChanged -= UpdateSessionGold;
    }

    private void Start()
    {
        if (MetaProgressionManager.Instance)
        {
            UpdateSafeGold(MetaProgressionManager.Instance.GetGold(), false);
            UpdateSalvage(MetaProgressionManager.Instance.GetSalvage(), false);
        }
        isInitialized = true;
    }

    private void UpdateSafeGold(int amount) => UpdateSafeGold(amount, true);

    private void UpdateSafeGold(int amount, bool showUI)
    {
        safeGold = amount;
        RefreshGoldDisplay();
        if (showUI && isInitialized) popAnimator.TriggerPopOrPunch();
    }

    private void UpdateSessionGold(int amount) => UpdateSessionGold(amount, true);

    private void UpdateSessionGold(int amount, bool showUI)
    {
        sessionGold = amount;
        RefreshGoldDisplay();
        if (showUI && isInitialized) popAnimator.TriggerPopOrPunch();
    }

    private void UpdateSalvage(int amount) => UpdateSalvage(amount, true);

    private void UpdateSalvage(int amount, bool showUI)
    {
        if (salvageText) salvageText.text = $"{amount} Scrap";
        if (showUI && isInitialized) popAnimator.TriggerPopOrPunch();
    }

    private void RefreshGoldDisplay()
    {
        if (goldText) goldText.text = sessionGold > 0 ? string.Format(goldFormat, safeGold, sessionGold) : $"{safeGold} Gold";
    }
}