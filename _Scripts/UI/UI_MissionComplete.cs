/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Displays the mission summary screen upon completion or failure.
/// </summary>
public class UI_MissionComplete : MonoBehaviour
{
    [Header("Visuals - Texts")]
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI salvageText;
    [SerializeField] private Button continueButton;

    [Header("Visuals - Colors")]
    [SerializeField] private Color successColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color failColor = new Color(1f, 0.2f, 0.2f);

    [Header("Animation")]
    [SerializeField] private float countDuration = 1.5f;

    private bool wasMissionSuccessful = false;
    private int earnedGold;
    private int earnedSalvage;

    private void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    /// <summary>
    /// Displays the results UI with mission outcome.
    /// </summary>
    public void Show(bool isSuccess, int gold, int salvage)
    {
        earnedGold = gold;
        earnedSalvage = salvage;

        gameObject.SetActive(true);

        wasMissionSuccessful = isSuccess;

        if (HubUIManager.Instance != null)
        {
            HubUIManager.Instance.SetCursorState(false);
        }

        if (isSuccess)
        {
            if (headerText)
            {
                headerText.text = "MISSION COMPLETE";
                headerText.color = successColor;

                if (AudioManager.Instance != null && AudioManager.Instance.data != null)
                {
                    AudioManager.Instance.PlayUI(AudioManager.Instance.data.missionSuccess, 0.8f);
                }
            }
            StartCoroutine(CountRoutine());
        }
        else
        {
            if (headerText)
            {
                headerText.text = "MISSION ABORTED";
                headerText.color = failColor;
            }
            UpdateTexts(0, 0);
        }
    }

    private IEnumerator CountRoutine()
    {
        float timer = 0f;

        while (timer < countDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(timer / countDuration);

            int currentGold = (int)Mathf.Lerp(0, earnedGold, progress);
            int currentSalvage = (int)Mathf.Lerp(0, earnedSalvage, progress);

            UpdateTexts(currentGold, currentSalvage);
            yield return null;
        }

        UpdateTexts(earnedGold, earnedSalvage);
    }

    private void UpdateTexts(int gold, int salvage)
    {
        if (goldText) goldText.text = $"+{gold} G";
        if (salvageText) salvageText.text = $"+{salvage} Scrap";
    }

    private void OnContinueClicked()
    {
        if (wasMissionSuccessful && MetaProgressionManager.Instance != null)
        {
            MetaProgressionManager.Instance.AddGold(earnedGold);
            MetaProgressionManager.Instance.AddSalvage(earnedSalvage);
        }

        PlayerPrefs.SetInt("HasLastRunData", 0);
        PlayerPrefs.Save();

        gameObject.SetActive(false);

        if (HubUIManager.Instance != null)
        {
            HubUIManager.Instance.ShowLoading(false);
            HubUIManager.Instance.ShowHUD();
            HubUIManager.Instance.UpdateEconomyUI();

            if (Player.Instance != null) Player.Instance.SetControlActive(true);
            HubUIManager.Instance.SetCursorState(true);
        }
    }
}