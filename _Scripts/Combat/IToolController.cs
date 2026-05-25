/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Contract implemented by components that control player tools (weapons, cleaning tools, etc.).
/// Provides lifecycle hooks for equipping/unequipping and input-driven actions.
/// </summary>
public interface IToolController
{
    ToolType SupportedToolType { get; }

    /// <summary>
    /// Called when the tool is equipped; provides the tool's configuration data for initialization.
    /// </summary>
    void OnEquip(ToolDataSO toolData);

    /// <summary>
    /// Called when the tool is unequipped to tear down visuals or state.
    /// </summary>
    void OnUnequip();

    /// <summary>
    /// Primary input action (e.g. left click) handler for this tool.
    /// </summary>
    void OnPrimaryAction();

    /// <summary>
    /// Called while the primary action is held; useful for charge attacks or continuous cleaning.
    /// </summary>
    /// <param name="isHolding">True when the input is currently held, false when released.</param>
    void OnPrimaryActionHold(bool isHolding);
}