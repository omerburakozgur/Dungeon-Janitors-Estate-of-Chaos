// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject defining configurable stats for an enemy archetype.
/// Designers can tune movement, combat and health parameters per enemy type. // short
/// </summary>
[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Name")] // Display name in inspector
    public string name; // Enemy display name - serialized

    [Header("Tier")]
    public EnemyTier enemyTier; // Difficulty or tier classification - serialized

    [Header("Enemy Movement")]
    public float patrolSpeed; // Speed while patrolling - serialized
    public float chaseSpeed; // Speed while chasing the player - serialized

    [Header("Enemy Stats")]
    public int maxHP; // Maximum health - serialized
    public int hpRegenRate; // Health regeneration per second (if used) - serialized

    [Header("Enemy Combat")]
    public int attackDamage; // Damage dealt per attack - serialized
    public int attackRange; // Attack range in units - serialized
    public int attackCooldown; // Time between attacks in seconds (or frames depending on usage) - serialized
}
