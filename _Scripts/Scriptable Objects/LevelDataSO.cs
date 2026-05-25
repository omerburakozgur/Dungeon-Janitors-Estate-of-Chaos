/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Stores data for dungeon levels, including rewards, mission objectives, and scene references.
/// </summary>
[CreateAssetMenu(fileName = "New Level Data", menuName = "Scriptable Objects/Level Data")]
public class LevelDataSO : ScriptableObject
{
    [Header("Level Information")]
    public string levelDisplayName;
    [TextArea(2, 4)]
    public string levelDescription;
    public Sprite levelThumbnail;

    [Header("Future Proofing (Video)")]
    public VideoClip previewVideo;

    [Header("Scene Loading")]
    public string sceneName;

    [Header("Difficulty & Rewards")]
    public string difficultyLevel = "Normal";
    public int baseGoldReward = 100;
    public int baseSalvageReward = 50;

    [Header("Mission Objectives")]
    public int proceduralDirtCount = 50;

    [Tooltip("Total trash count to be spawned in the level.")]
    public int proceduralTrashCount = 25;

    public int requiredDirtToClean = 40;
    public int requiredTrashToCollect = 15;

    [Header("Bonus Rewards (Per Extra Item)")]
    [Tooltip("Gold reward for every extra stain cleaned beyond requirements.")]
    public int bonusGoldPerExtraDirt = 50;

    [Tooltip("Gold reward for every extra piece of trash collected beyond requirements.")]
    public int bonusGoldPerExtraTrash = 25;

    [Tooltip("Salvage reward for every extra piece of trash collected beyond requirements.")]
    public int bonusSalvagePerExtraTrash = 1;
}