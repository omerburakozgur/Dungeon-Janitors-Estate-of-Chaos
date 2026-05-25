/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Configuration for tools, defining both utility (cleaning) and combat behavior.
/// </summary>
[CreateAssetMenu(fileName = "New Tool Data", menuName = "Scriptable Objects/Tool Data")]
public class ToolDataSO : ScriptableObject
{
    [Header("Info")]
    public string toolName;
    public GameObject prefab;

    [Header("Type Configuration")]
    public ToolType generalType;
    public CleaningToolType specificCleaningType;

    [Header("Common Stats")]
    [Tooltip("Seconds between usage.")]
    public float cooldown = 0.5f;
    [Tooltip("Maximum distance for hit detection.")]
    public float maxDistance = 2.5f;
    [Tooltip("Detection radius for overlap checks.")]
    public float toolRadius = 0.5f;

    [Header("Cleaning Stats")]
    public float cleanSpeed = 0.5f;

    [Header("Combat Stats")]
    [Tooltip("Damage applied per hit.")]
    public float damage = 10f;
    [Tooltip("Base stun duration in seconds.")]
    public float stunDuration = 1.0f;
    [Tooltip("Duration of physical knockback effect.")]
    public float knockbackDuration = 0.2f;
    [Tooltip("Strength of the knockback impulse.")]
    public float knockbackPower = 5f;
    [Tooltip("Screen shake intensity during hit.")]
    public float screenShakeIntensity = 0.2f;
}