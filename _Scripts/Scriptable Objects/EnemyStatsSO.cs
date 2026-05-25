/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Defines the core statistical parameters for enemy entities.
/// </summary>
[CreateAssetMenu(fileName = "EnemyStatsSO", menuName = "Scriptable Objects/EnemyStatsSO")]
public class EnemyStatsSO : ScriptableObject
{
    [Header("Identity")]
    public string enemyName;
    public EnemyTier enemyTier;

    [Header("Stats")]
    public int maxHP;
    public int hpRegenRate;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5f;
    public float rotationSpeed = 5f;
    public float patrolWaitTime = 3f;

    [Header("Combat & Distance")]
    public int attackDamage;
    public float attackCooldown;
    public float attackDamageDelay = 0.5f;
    public float detectionRange = 10f;

    [Tooltip("Target stopping distance mapping execution points ensuring enemy isn't clipping player geometries. MUST be lower than Attack Range!")]
    public float stoppingDistance = 1.5f;

    [Tooltip("Actual required hit connection distance bounding max attack radii allowed.")]
    public float attackRange = 2.0f;

    [Header("Knockback & Weight")]
    [Tooltip("Resistance to knockback. 0 = Flies away, 1 = Immovable (Wall/Boss).")]
    [Range(0f, 1f)]
    public float knockbackResistance = 0f;

    [Tooltip("Percentage of stun duration ignored (0 = 0%, 0.9 = 90%).")]
    [Range(0f, 1f)]
    public float stunResistance = 0f;

    [Tooltip("How long the enemy is immune to stun after being stunned.")]
    public float stunImmunityWindow = 3.0f;
}