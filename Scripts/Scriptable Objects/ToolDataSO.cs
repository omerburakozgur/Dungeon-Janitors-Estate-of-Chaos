// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

[CreateAssetMenu(fileName = "New Tool Data", menuName = "Scriptable Objects/Tool Data")]
public class ToolDataSO : ScriptableObject
{
    [Header("Info")]
    public string toolName; // Display name of the tool
    public GameObject prefab; // Visual prefab shown in-hand

    [Header("Type Configuration")]
    public ToolType generalType; // Cleaning or Weapon
    public CleaningToolType specificCleaningType; // Specific cleaning classification

    [Header("Common Stats")]
    [Tooltip("Attack/cleaning cooldown expressed in seconds")]
    public float cooldown = 0.5f; // Seconds between uses
    [Tooltip("Effective interaction range (Raycast/BoxCast length)")]
    public float maxDistance = 2.5f; // Range in meters
    [Tooltip("Interaction area radius used for overlap checks")]
    public float toolRadius = 0.5f; // Radius for hit detection

    [Header("Cleaning Stats")]
    public float cleanSpeed = 0.5f; // How quickly it cleans per second (normalized)

    // --- Combat statistics ---
    [Header("Combat Stats")]
    [Tooltip("Damage applied per hit")]
    public float damage = 10f; // Damage per hit

    [Tooltip("Knockback impulse strength")]
    public float knockbackPower = 5f; // Knockback strength

    [Tooltip("Screen shake intensity to apply on hit (future use)")]
    public float screenShakeIntensity = 0.2f; // Screen shake amount
}