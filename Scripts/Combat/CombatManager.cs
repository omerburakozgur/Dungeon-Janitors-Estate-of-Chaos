// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;
using System;

/// <summary>
/// Combat system authority responsible for applying damage, handling death events
/// and serving as a single point of truth for combat-related logic. // short
/// </summary>
public class CombatManager : SingletonManager<CombatManager>
{
    // Event channels such as hit VFX or audio may be added here if needed (commented out in current code)
    // [SerializeField] private VoidEventChannelSO onEnemyHitChannel; // optional event channel - short

    /// <summary>
    /// Called by weapon systems to process a validated hit on a damageable target.
    /// Applies damage, knockback and notifies any interested systems. // short
    /// </summary>
    public void ProcessPlayerHit(IDamageable target, float damage, Vector3 knockbackForce, Vector3 hitPoint)
    {
        if (target == null) return; // guard - short

        //1) Apply damage using the target's implementation
        target.TakeDamage(damage, knockbackForce, hitPoint);

        if (true) Debug.Log($"[CombatManager] Damage applied: {damage} -> Target health changed."); // debug log - short

        //2) Optional feedback e.g., events for VFX/sound can be raised here
        // onEnemyHitChannel?.Raise(); // commented: enable if channel is present
    }

    /// <summary>
    /// Notifies systems when an enemy has died. Intended to be expanded to trigger
    /// loot drops, score updates or spawn effects. // short
    /// </summary>
    public void ReportEnemyDeath(GameObject enemyObj)
    {
        // Example: notify the LootManager to spawn loot at the death position
        // LootManager.Instance.SpawnLoot(enemyObj.transform.position);
        Debug.Log($"[CombatManager] Enemy died: {enemyObj.name}"); // debug log - short
    }
}