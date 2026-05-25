/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Interface for objects that can receive damage and respond physically.
/// Implement this to provide a standard API for applying damage, knockback and querying health.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Apply damage to the object, optionally with a knockback impulse and hit location for VFX.
    /// Added 'stunDuration' parameter for CC mechanics.
    /// </summary>
    /// <param name="amount">Amount of damage to subtract from health.</param>
    /// <param name="knockbackForce">Optional impulse vector applied to the object.</param>
    /// <param name="hitPoint">Optional world position where the hit occurred (for VFX placement).</param>
    /// <param name="stunDuration">Length in seconds the entity should be incapacitated.</param>
    /// <param name="knockbackDuration">Length in seconds governing the length of the physical pushback state.</param>
    void TakeDamage(float amount, Vector3? knockbackForce = null, Vector3? hitPoint = null, float stunDuration = 0f, float knockbackDuration = 0.2f);

    /// <summary>
    /// Returns the current health value of the object.
    /// </summary>
    float GetHealth();

    /// <summary>
    /// Returns true when the object is considered dead (health <=0).
    /// </summary>
    bool IsDead();
}