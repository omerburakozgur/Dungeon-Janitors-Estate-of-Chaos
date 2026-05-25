/* ==========================================================\
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines a dirt/stain type used by the cleaning system.
/// Contains visual references, required tool type, and gameplay tuning parameters.
/// </summary>
[CreateAssetMenu(fileName = "New Dirt Data", menuName = "Scriptable Objects/Dirt Data")]
public class DirtDataSO : ScriptableObject
{
    [Header("Identity")]
    public string dirtName;

    [Header("Requirements")]
    [Tooltip("Which tool type is required to clean this stain.")]
    public CleaningToolType requiredToolType;

    [Header("Stats")]
    [Tooltip("Stain resistance multiplier. 1 = baseline; higher values make stains harder to remove.")]
    public float resistance = 1.0f;

    [Tooltip("Passive buildup rate applied per second.")]
    public float stainBuildupRate;

    [Header("Visuals")]
    [Tooltip("VFX prefab spawned when the stain is FULLY cleaned.")]
    public GameObject cleanVFX;

    [Tooltip("VFX prefab spawned continuously while the player is actively scrubbing.")]
    public GameObject cleaningVFX;

    [Tooltip("List of possible decal materials to apply; a random entry may be chosen at spawn.")]
    public List<Material> decalMaterials;
}