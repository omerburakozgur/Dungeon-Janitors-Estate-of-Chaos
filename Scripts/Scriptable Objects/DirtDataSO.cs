// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

// Asset metadata and licensing information retained in project Readme.

/// <summary>
/// ScriptableObject that defines a dirt/stain type used by the cleaning system.
/// Contains visual references, required tool type and gameplay tuning parameters. // short
/// </summary>
[CreateAssetMenu(fileName = "New Dirt Data", menuName = "Scriptable Objects/Dirt Data")]
public class DirtDataSO : ScriptableObject
{
    [Header("Identity")] // Inspector: display name
    public string dirtName; // Friendly name displayed in editors - short

    [Header("Requirements")] // Inspector: cleaning requirements
    [Tooltip("Which tool type is required to clean this stain (validation by cleaning manager).")]
    public CleaningToolType requiredToolType; // Required tool enum - serialized

    [Header("Stats")] // Inspector: gameplay values
    [Tooltip("Stain resistance multiplier.1 = baseline; higher values make stains harder to remove.")]
    public float resistance = 1.0f; // Damage resistance for cleaning operations - short

    [Tooltip("Passive buildup rate applied per second (used by spawners/manager to increase dirt over time).")]
    public float stainBuildupRate; // Rate of passive dirt increase per second - serialized

    [Header("Visuals")] // Inspector: visual references
    [Tooltip("Optional VFX prefab spawned when the stain is fully cleaned.")]
    public GameObject cleanVFX; // Cleanup VFX - serialized

    [Tooltip("List of possible decal materials to apply; a random entry may be chosen at spawn.")]
    public List<Material> materials; // Visual material options - serialized
}
