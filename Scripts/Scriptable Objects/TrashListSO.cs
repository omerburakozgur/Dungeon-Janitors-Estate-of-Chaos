// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject which contains categorized prefabs for different trash sizes.
/// Used by spawn systems to choose appropriate trash objects.
/// </summary>
[CreateAssetMenu(fileName = "TrashListSO", menuName = "Scriptable Objects/TrashListSO")]
public class TrashListSO : ScriptableObject
{
    [Header("Small Trash List")]
    public GameObject[] smallThrashList; // Prefab list for small trash

    [Header("Average Trash List")]
    public GameObject[] averageThrashList; // Prefab list for medium trash

    [Header("Big Trash List")]
    public GameObject[] bigThrashList; // Prefab list for large trash
}
