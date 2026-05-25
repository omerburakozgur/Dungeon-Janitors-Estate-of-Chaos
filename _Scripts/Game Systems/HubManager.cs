/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using TMPro;

/// <summary>
/// Manages high-level interactions within the hub scene, establishing cursor constraints and economy initialization.
/// </summary>
public class HubManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text element displaying the total banked gold count.")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Tooltip("Text element displaying the total banked salvage count.")]
    [SerializeField] private TextMeshProUGUI salvageText;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int totalGold = PlayerPrefs.GetInt(PlayerPrefsKeys.TOTAL_GOLD, 0);
        int totalSalvage = PlayerPrefs.GetInt(PlayerPrefsKeys.TOTAL_SALVAGE, 0);

        if (goldText != null)
            goldText.text = (totalGold.ToString() + " Gold");

        if (salvageText != null)
            salvageText.text = (totalSalvage.ToString() + " Salvage");
    }

    /// <summary>
    /// Deletes all local saved progression for testing and resets HUD visual values.
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        if (goldText) goldText.text = "0";
        if (salvageText) salvageText.text = "0";
    }
}