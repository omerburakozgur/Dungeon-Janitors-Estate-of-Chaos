// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Contract implemented by components that control player tools (weapons, cleaning tools, etc.).
/// Provides lifecycle hooks for equipping/unequipping and input-driven actions.
/// </summary>
public interface IToolController
{
    /// <summary>Indicates which ToolType this controller supports.</summary>
    ToolType SupportedToolType { get; } // Supported tool classification

    /// <summary>
    /// Called when the tool is equipped; provides the tool's configuration data for initialization.
    /// </summary>
    void OnEquip(ToolDataSO toolData); // Setup visuals/parameters when equipped

    /// <summary>Called when the tool is unequipped to tear down visuals or state.</summary>
    void OnUnequip(); // Cleanup when unequipped

    /// <summary>Primary input action (e.g. left click) handler for this tool.</summary>
    void OnPrimaryAction(); // Perform primary action (attack, clean, etc.)

    /// <summary>
    /// Called while the primary action is held; useful for charge attacks or continuous cleaning.
    /// </summary>
    /// <param name="isHolding">True when the input is currently held, false when released.</param>
    void OnPrimaryActionHold(bool isHolding); // Hold state notifications
}