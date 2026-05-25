/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Project-wide enumerations for cleaning, tools, loot, and game flow states.
/// </summary>
public enum DirtType
{
    Grime,
    Blood,
    Goo,
    Oil
}

public enum ToolType
{
    None,
    Cleaning,
    Weapon,
    Carry
}

/// <summary>
/// Categorizes cleaning tools used by the CleaningManager for system validation.
/// </summary>
public enum CleaningToolType
{
    None,
    Mop,
    Broom,
    Vacuum,
    Scrubber,
    TrashBucket
}

public enum EquipmentTier
{
    Tier1,
    Tier2,
    Tier3,
    Tier4,
    Tier5
}

public enum EnemyTier
{
    Basic,
    Elite,
    Boss
}

public enum DirtTier
{
    Surface,
    Embedded,
    Tough,
    Ingrained
}

public enum LootTier
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum GameState
{
    Playing,
    Paused,
    Cutscene
}

public enum SceneState
{
    InMainMenu,
    InLobby,
    IsPlaying,
    IsPaused
}

public enum ShopItemType
{
    ToolUnlock,
    StatUpgrade
}

public enum StatType
{
    None,
    MoveSpeed,
    CleaningSpeed,
    MaxHealth,
    AttackDamage
}