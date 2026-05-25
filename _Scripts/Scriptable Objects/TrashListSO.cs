/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container for categorized trash prefabs, used by spawn systems.
/// </summary>
[CreateAssetMenu(fileName = "TrashListSO", menuName = "Scriptable Objects/TrashListSO")]
public class TrashListSO : ScriptableObject
{
    [Header("Small Trash List")]
    [Tooltip("Pool of small-sized trash prefabs.")]
    public GameObject[] smallThrashList;

    [Header("Average Trash List")]
    [Tooltip("Pool of medium-sized trash prefabs.")]
    public GameObject[] averageThrashList;

    [Header("Big Trash List")]
    [Tooltip("Pool of large-sized trash prefabs.")]
    public GameObject[] bigThrashList;
}