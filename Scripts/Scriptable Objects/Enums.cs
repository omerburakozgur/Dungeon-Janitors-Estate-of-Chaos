// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Project enums used across multiple systems (cleaning, tools, loot and game flow).
/// Keep this file limited to lightweight value types to avoid circular references. // short
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
    Cleaning,   // Cleaning mode - short
    Weapon,     // Combat mode - short
    Carry       // Carrying mode - short
}

/// <summary>
/// Subtypes for cleaning tools used by the CleaningManager to validate compatibility.
/// </summary>
public enum CleaningToolType
{
    None,
    Broom,      // Broom - short
    Mop,        // Mop - short
    Vacuum,     // Vacuum - short
    Bucket,     // Bucket - short
    Torch,      // Torch (special cases) - short
    ThrashBag   // Trash bag - short
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
    Common,
    Rare,
    Epic,
    Legendary
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
    Playing,    // Game is active - short
    Paused,     // Game is paused / menu open - short
    Cutscene    // In a non-interactive cutscene - short
}

public enum SceneState
{
    InMainMenu,
    InLobby,
    IsPlaying,
    IsPaused,
}

