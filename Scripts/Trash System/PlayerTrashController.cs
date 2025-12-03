// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Holds player-specific trash configuration such as maximum capacity.
/// The authoritative trash count is managed by TrashManager.
/// </summary>
public class PlayerTrashController : MonoBehaviour
{
    [Header("Settings")]
    public int maxTrash = 10; // Public so TrashManager can read the capacity

    [Header("Visuals")]
    [SerializeField] private Transform trashBagVisual; // Optional visual to update when capacity changes

    // Note: current trash amount is currently stored in TrashManager; during multiplayer refactor
    // this may be moved here (see MP Refactor section).
}