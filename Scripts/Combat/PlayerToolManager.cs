// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Manages player's equipped tools and forwards input events to the
/// appropriate subsystems (combat and cleaning). Handles model visibility
/// and broadcasts state through event channels.
/// </summary>
public class PlayerToolManager : MonoBehaviour
{
    [Header("Dependencies")]
    private InputManager inputManager; // Cached reference to the input manager

    [Header("Broadcasting Channels")]
    [SerializeField] private VoidEventChannelSO onCombatAttackChannel; // Raised when an attack should occur
    [SerializeField] private BoolEventChannelSO onCleaningStateChannel; // Raised while cleaning is held/released

    [Header("Tool Models")]
    [SerializeField] private GameObject weaponModelParent; // Root object for weapon visuals
    [SerializeField] private GameObject cleaningModelParent; // Root object for cleaning visuals

    [Header("State")]
    [SerializeField] private ToolType currentTool = ToolType.Weapon; // Currently selected tool

    /// <summary>
    /// Enumeration of available tools the player can equip.
    /// </summary>
    public enum ToolType { None, Weapon, Cleaning }

    /// <summary>
    /// Acquire references early in the lifecycle.
    /// </summary>
    private void Awake()
    {
        // Get the singleton input manager instance
        inputManager = InputManager.Instance; // Cache input manager instance
    }

    /// <summary>
    /// Subscribe to input events and refresh visual state when enabled.
    /// </summary>
    private void OnEnable()
    {
        // Subscribe to input events if input manager is available
        if (inputManager != null)
        {
            inputManager.OnHotbar1Pressed += SelectCleaningTool; // Hotbar1 selects cleaning tool
            inputManager.OnHotbar2Pressed += SelectWeaponTool; // Hotbar2 selects weapon tool
            inputManager.OnAttackPressed += HandleAttackPressed; // Attack pressed forwarded
            inputManager.OnAttackHeldStatus += HandleAttackHeldStatus; // Attack held status forwarded
        }

        // Ensure the correct model visibility after enabling
        RefreshToolVisibility(); // Update models based on current tool
    }

    /// <summary>
    /// Unsubscribe from input events and clear any transient state when disabled.
    /// </summary>
    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnHotbar1Pressed -= SelectCleaningTool; // Unsubscribe
            inputManager.OnHotbar2Pressed -= SelectWeaponTool; // Unsubscribe
            inputManager.OnAttackPressed -= HandleAttackPressed; // Unsubscribe
            inputManager.OnAttackHeldStatus -= HandleAttackHeldStatus; // Unsubscribe
        }

        // Safety: if we were cleaning when the object disables, broadcast stop signal
        if (currentTool == ToolType.Cleaning && onCleaningStateChannel != null)
        {
            onCleaningStateChannel.Raise(false); // Ensure cleaning state is cleared
        }
    }

    // --- LOGIC ---

    /// <summary>
    /// Forward single-press attack events to combat channel when weapon is equipped.
    /// </summary>
    private void HandleAttackPressed()
    {
        if (currentTool == ToolType.Weapon)
            onCombatAttackChannel?.Raise(); // Trigger combat attack event
    }

    /// <summary>
    /// Forward hold/release status for cleaning tool using the cleaning state channel.
    /// </summary>
    /// <param name="isHeld">True when the input is held, false when released</param>
    private void HandleAttackHeldStatus(bool isHeld)
    {
        if (currentTool == ToolType.Cleaning)
            onCleaningStateChannel?.Raise(isHeld); // Broadcast cleaning held state
    }

    private void SelectWeaponTool() { SwitchTool(ToolType.Weapon); } // Shortcut to select weapon
    private void SelectCleaningTool() { SwitchTool(ToolType.Cleaning); } // Shortcut to select cleaning tool

    /// <summary>
    /// Switch currently equipped tool and handle exit-from-cleaning state.
    /// </summary>
    /// <param name="newTool">Tool type to switch to</param>
    private void SwitchTool(ToolType newTool)
    {
        if (currentTool == newTool) return; // No change

        // If we leave cleaning mode, ensure we broadcast stop
        if (currentTool == ToolType.Cleaning)
            onCleaningStateChannel?.Raise(false); // Stop cleaning

        currentTool = newTool; // Update state
        RefreshToolVisibility(); // Update visuals
    }

    /// <summary>
    /// Toggle visibility of tool model parents according to current tool.
    /// </summary>
    private void RefreshToolVisibility()
    {
        if (weaponModelParent != null)
            weaponModelParent.SetActive(currentTool == ToolType.Weapon); // Show/hide weapon

        if (cleaningModelParent != null)
            cleaningModelParent.SetActive(currentTool == ToolType.Cleaning); // Show/hide cleaning tool
    }
}