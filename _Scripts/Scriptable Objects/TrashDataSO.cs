/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Properties defining a trash item, such as name and relative size.
/// </summary>
[CreateAssetMenu(fileName = "TrashDataSO", menuName = "Scriptable Objects/TrashDataSO")]
public class TrashDataSO : ScriptableObject
{
    [Header("Name")]
    public string itemName;

    [Header("Trash Size")]
    [Tooltip("Size value used for logic calculations.")]
    public int trashSize;
}