// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Interface for objects that can receive damage and respond physically.
/// Implement this to provide a standard API for applying damage, knockback and querying health.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Apply damage to the object, optionally with a knockback impulse and hit location for VFX.
    /// </summary>
    /// <param name="amount">Amount of damage to subtract from health.</param>
    /// <param name="knockbackForce">Optional impulse vector applied to the object.</param>
    /// <param name="hitPoint">Optional world position where the hit occurred (for VFX placement).</param>
    void TakeDamage(float amount, Vector3? knockbackForce = null, Vector3? hitPoint = null);

    /// <summary>Returns the current health value of the object.</summary>
    float GetHealth(); // Accessor for current health

    /// <summary>Returns true when the object is considered dead (health <=0).</summary>
    bool IsDead(); // True when health is depleted
}