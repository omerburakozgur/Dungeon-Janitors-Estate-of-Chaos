/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;
using DG.Tweening;

/// <summary>
/// Tracks and displays current mission objectives.
/// </summary>
[RequireComponent(typeof(UI_PopAnimator))]
public class UI_MissionTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI trackerText;

    [Tooltip("Text displayed upon completing the mission.")]
    [SerializeField] private TextMeshProUGUI missionCompleteText;

    [Tooltip("Panel container for mission completion.")]
    [SerializeField] private GameObject missionCompletePanel;

    private UI_PopAnimator popAnimator;

    private void Awake()
    {
        popAnimator = GetComponent<UI_PopAnimator>();
        if (missionCompleteText != null && missionCompletePanel != null)
        {
            missionCompleteText.gameObject.SetActive(false);
            missionCompletePanel.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        if (ActiveMissionManager.Instance != null)
        {
            ActiveMissionManager.Instance.OnObjectiveUpdated += UpdateUI;
            ActiveMissionManager.Instance.OnMissionComplete += HandleMissionComplete;

            UpdateUI(ActiveMissionManager.Instance.currentCleanedDirt, ActiveMissionManager.Instance.currentCollectedTrash);
        }
    }

    private void OnDestroy()
    {
        if (ActiveMissionManager.Instance != null)
        {
            ActiveMissionManager.Instance.OnObjectiveUpdated -= UpdateUI;
            ActiveMissionManager.Instance.OnMissionComplete -= HandleMissionComplete;
        }
    }

    private void UpdateUI(int cleaned, int trashed)
    {
        if (ActiveMissionManager.CurrentContract == null) return;

        int reqDirt = ActiveMissionManager.CurrentContract.requiredDirtToClean;
        int reqTrash = ActiveMissionManager.CurrentContract.requiredTrashToCollect;

        trackerText.text = $"Dirt Cleaned: {cleaned}/{reqDirt}" + $" - Trash Collected: {trashed}/{reqTrash}";

        if (popAnimator != null) popAnimator.TriggerPopOrPunch();
    }

    private void HandleMissionComplete()
    {
        if (missionCompleteText != null && missionCompletePanel != null)
        {
            missionCompleteText.gameObject.SetActive(true);
            missionCompletePanel.gameObject.SetActive(true);

            missionCompleteText.text = "<color=#00FF00>Mission Accomplished,!</color> You Can Extract";
            missionCompleteText.transform.DOPunchScale(Vector3.one * 0.15f, 0.5f).SetUpdate(true);
        }
    }
}