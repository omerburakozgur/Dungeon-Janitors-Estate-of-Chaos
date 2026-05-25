/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides a developer interface for debugging economy in the hub.
/// </summary>
public class HubDebugUI : MonoBehaviour
{
    [Header("Panel Controls")]
    [Tooltip("The menu panel container to toggle.")]
    [SerializeField] private GameObject contentPanel;
    [SerializeField] private Button toggleButton;

    [Header("Cheat Buttons")]
    [SerializeField] private Button add1000GoldBtn;
    [SerializeField] private Button add5000GoldBtn;
    [SerializeField] private Button clearSaveBtn;

    private bool isOpen = false;

    private void Start()
    {
        if (contentPanel != null) contentPanel.SetActive(false);
        isOpen = false;

        if (toggleButton != null) toggleButton.onClick.AddListener(ToggleMenu);
        if (add1000GoldBtn != null) add1000GoldBtn.onClick.AddListener(() => AddGold(1000));
        if (add5000GoldBtn != null) add5000GoldBtn.onClick.AddListener(() => AddGold(5000));
        if (clearSaveBtn != null) clearSaveBtn.onClick.AddListener(ResetSaveData);
    }

    private void ToggleMenu()
    {
        isOpen = !isOpen;
        if (contentPanel != null) contentPanel.SetActive(isOpen);
    }

    private void AddGold(int amount)
    {
        int currentGold = PlayerPrefs.GetInt("TotalGold", 0);
        int newGold = currentGold + amount;
        PlayerPrefs.SetInt("TotalGold", newGold);
        PlayerPrefs.Save();

        Debug.Log($"[CHEAT] Added {amount} Gold. New Balance: {newGold}");
    }

    private void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("[CHEAT] Save Data Cleared!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}