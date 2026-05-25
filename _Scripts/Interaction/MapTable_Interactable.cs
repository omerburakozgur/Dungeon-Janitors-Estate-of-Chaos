/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Interaction handler for the map table in the hub scene.
/// Initiates the mission selection or level loading process.
/// </summary>
public class MapTable_Interactable : BaseInteractable
{
    [Header("Map Settings")]
    [Tooltip("Interaction text displayed in the UI.")]
    [SerializeField] private string promptText = "[E] Select Mission";

    /// <summary>
    /// Returns the assigned prompt text for mission selection.
    /// </summary>
    public override string GetInteractionPrompt()
    {
        return promptText;
    }

    /// <summary>
    /// Triggers the LevelManager to start the mission sequence.
    /// </summary>
    public override void RequestInteract()
    {
        if (LevelManager.Instance != null)
        {
            Debug.Log(">> Requesting Mission Start...");
            LevelManager.Instance.LoadMission();
        }
        else
        {
            Debug.LogError("LevelManager is missing in the scene!");
        }
    }

    public override void RequestHoldInteract() { }
    public override void RequestReleaseInteract() { }
}