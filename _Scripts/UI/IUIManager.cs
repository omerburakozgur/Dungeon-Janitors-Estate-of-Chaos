/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

/// <summary>
/// Interface defining essential UI management functionality shared across different game environments.
/// </summary>
public interface IUIManager
{
    /// <summary>
    /// Toggles the loading screen overlay.
    /// </summary>
    void ShowLoading(bool show);

    /// <summary>
    /// Configures the cursor locking state.
    /// </summary>
    void SetCursorState(bool isLocked);
}