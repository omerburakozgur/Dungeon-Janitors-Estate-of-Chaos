/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

/// <summary>
/// A centralized data holder for all PlayerPrefs keys used across the project.
/// Prevents magic strings and ensures consistency when saving or loading data.
/// </summary>
public static class PlayerPrefsKeys
{
    // --- GRAPHICS & GAMEPLAY ---
    public const string SENSITIVITY = "MouseSensitivity";
    public const string RES_WIDTH = "ResWidth";
    public const string RES_HEIGHT = "ResHeight";
    public const string FULLSCREEN = "FullscreenMode";
    public const string QUALITY = "QualityLevel";
    public const string FPS = "TargetFPS";

    // --- AUDIO MIXER ---
    public const string MIXER_MASTER = "MasterVol";
    public const string MIXER_MUSIC = "MusicVol";
    public const string MIXER_SFX = "SFXVol";
    public const string MIXER_AMBIENT = "AmbientVol";
    public const string MIXER_UI = "UIVol";

    // --- ECONOMY & META PROGRESSION ---
    public const string TOTAL_GOLD = "TotalGold";
    public const string TOTAL_SALVAGE = "TotalSalvage";
}