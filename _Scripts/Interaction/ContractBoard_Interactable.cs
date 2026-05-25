/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Interaction interface handler mapping interaction input towards opening Contract Board Manager UI.
/// Enables raycast targeting logic for the board.
/// </summary>
public class ContractBoard_Interactable : BaseInteractable
{
    /// <summary>
    /// Fired when the player presses the primary inspect hotkey targeting this object.
    /// Overrides typical interactable flow opening the contract selection menu.
    /// </summary>
    public override void RequestInteract()
    {
        Debug.Log($"Interact request received. Object: {gameObject.name}");

        if (ContractBoardManager.Instance != null)
        {
            ContractBoardManager.Instance.OpenContractUI();
        }
        else
        {
            Debug.LogWarning("ContractBoardManager not found!");
        }
    }

    /// <summary>
    /// Overridden method implementation resolving hold-key interactions (Not used here).
    /// </summary>
    public override void RequestHoldInteract()
    {
    }

    /// <summary>
    /// Overridden method implementation ending hold-key interaction (Not used here).
    /// </summary>
    public override void RequestReleaseInteract()
    {
    }
}