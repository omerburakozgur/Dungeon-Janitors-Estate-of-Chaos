/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Handles interactions with the shop terminal, triggering the UI and cursor state updates.
/// </summary>
public class ShopInteractable : BaseInteractable
{
    [Header("UI Reference")]
    [SerializeField] private GameObject shopUIPanel;

    /// <summary>
    /// Returns the interaction prompt specific to the shop terminal.
    /// </summary>
    public override string GetInteractionPrompt()
    {
        return "[E] Open Terminal";
    }

    /// <summary>
    /// Triggers the shop UI to open and unlocks the cursor.
    /// </summary>
    public override void RequestInteract()
    {
        Debug.Log("Opening Shop...");
        if (ShopUIManager.Instance != null)
        {
            ShopUIManager.Instance.OpenShop();
            shopUIPanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public override void RequestHoldInteract() { }
    public override void RequestReleaseInteract() { }
}