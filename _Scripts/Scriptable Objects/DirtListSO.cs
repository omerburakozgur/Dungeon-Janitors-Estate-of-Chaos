/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container ScriptableObject that exposes a registry of all available dirt data entries 
/// for designers to reference or iterate through in management tools.
/// </summary>
[CreateAssetMenu(fileName = "DirtListSO", menuName = "Scriptable Objects/DirtListSO")]
public class DirtListSO : ScriptableObject
{
    [Header("List of all dirt types")]
    [Tooltip("The comprehensive list of all dirt types available in the project.")]
    public List<DirtDataSO> dirtList;
}