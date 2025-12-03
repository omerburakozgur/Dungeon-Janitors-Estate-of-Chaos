// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using System.Collections.Generic;
using UnityEngine;

// ScriptableObject representing a trash item type with basic properties such as name and size.
[CreateAssetMenu(fileName = "TrashDataSO", menuName = "Scriptable Objects/TrashDataSO")]
public class TrashDataSO : ScriptableObject
{
    [Header("Name")]
    public string itemName; // Display name of the trash item

    [Header("Trash Size")]
    public int trashSize; // Arbitrary size/value used by game logic
}
