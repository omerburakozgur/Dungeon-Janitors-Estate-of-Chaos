/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Displays details for a selected mission contract.
/// </summary>
public class UI_ContractDetailView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI difficultyText;
    [SerializeField] private TextMeshProUGUI goldRewardText;
    [SerializeField] private TextMeshProUGUI salvageRewardText;
    [SerializeField] private TextMeshProUGUI objectivesText;

    [Tooltip("Displays a preview of the level. Supports video target textures in future.")]
    [SerializeField] private Image previewImage;

    [SerializeField] private Button acceptButton;

    private Tween typewriterTween;
    private LevelDataSO currentSelectedLevel;

    private void Awake()
    {
        ClearView();
        acceptButton.onClick.AddListener(OnAcceptClicked);
    }

    /// <summary>
    /// Populates the UI view with specific level data.
    /// </summary>
    /// <param name="levelData">The data object containing level information.</param>
    public void ShowDetails(LevelDataSO levelData)
    {
        currentSelectedLevel = levelData;

        titleText.text = levelData.levelDisplayName;
        difficultyText.text = $"Threat: {levelData.difficultyLevel}";
        goldRewardText.text = $"{levelData.baseGoldReward} Gold";
        salvageRewardText.text = $"{levelData.baseSalvageReward} Salvage";
        objectivesText.text = $"OBJECTIVES: Clean {levelData.requiredDirtToClean} Dirt - Collect {levelData.requiredTrashToCollect} Trash";

        if (levelData.levelThumbnail != null)
        {
            previewImage.sprite = levelData.levelThumbnail;
            previewImage.color = Color.white;

            previewImage.DOFade(0f, 0f).SetUpdate(true);
            previewImage.DOFade(1f, 0.4f).SetUpdate(true);
        }

        descriptionText.text = "";
        if (typewriterTween != null) typewriterTween.Kill();

        int textLength = levelData.levelDescription.Length;
        typewriterTween = DOTween.To(() => 0, x => descriptionText.text = levelData.levelDescription.Substring(0, x), textLength, 1.5f)
            .SetUpdate(true)
            .SetEase(Ease.Linear);

        acceptButton.interactable = true;
        acceptButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f).SetUpdate(true);
    }

    /// <summary>
    /// Resets the UI view to default state.
    /// </summary>
    public void ClearView()
    {
        currentSelectedLevel = null;
        titleText.text = "SELECT A CONTRACT";
        descriptionText.text = "Awaiting employee selection...";
        difficultyText.text = "-";
        goldRewardText.text = "-";
        salvageRewardText.text = "-";
        objectivesText.text = "OBJECTIVES:\n-";
        previewImage.color = new Color(0, 0, 0, 0);
        acceptButton.interactable = false;
    }

    private void OnAcceptClicked()
    {
        if (currentSelectedLevel != null && ContractBoardManager.Instance != null)
        {
            ContractBoardManager.Instance.AcceptContract(currentSelectedLevel);
        }
    }
}