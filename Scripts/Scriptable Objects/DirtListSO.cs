// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple container ScriptableObject that exposes a list of available dirt data
/// entries for designers to reference or iterate through in tools. // short
/// </summary>
[CreateAssetMenu(fileName = "DirtListSO", menuName = "Scriptable Objects/DirtListSO")]
public class DirtListSO : ScriptableObject
{
    [Header("List of all dirt types")] // Inspector header
    public List<DirtDataSO> dirtList; // Collection of all dirt data assets - serialized
}
