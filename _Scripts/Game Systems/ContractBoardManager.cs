/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System;
using DG.Tweening;

/// <summary>
/// Manages the UI and logic for accepting available dungeon contracts.
/// Retrieves level data and initializes UI visual states using DOTween.
/// </summary>
public class ContractBoardManager : SingletonManager<ContractBoardManager>
{
    public static event Action<LevelDataSO> OnContractAccepted;

    [Header("Available Contracts")]
    public LevelDataSO[] availableLevels;

    [Header("UI References")]
    [SerializeField] private GameObject contractUIPanel;
    [SerializeField] private Transform slotsContainer;
    [SerializeField] private UI_ContractSlot contractSlotPrefab;
    [SerializeField] private UI_ContractDetailView detailView;

    private CanvasGroup panelCanvasGroup;

    /// <summary>
    /// Initializes UI panel states and caches components.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (contractUIPanel)
        {
            panelCanvasGroup = contractUIPanel.GetComponent<CanvasGroup>();
            contractUIPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Opens the Contract Board UI with a stylized DOTween animated entry and halts player movement.
    /// </summary>
    public void OpenContractUI()
    {
        if (contractUIPanel)
        {
            contractUIPanel.SetActive(true);

            if (panelCanvasGroup)
            {
                panelCanvasGroup.DOKill();
                panelCanvasGroup.alpha = 0f;
                panelCanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);

                contractUIPanel.transform.DOKill(true);
                contractUIPanel.transform.localScale = Vector3.one * 0.9f;
                contractUIPanel.transform.DOScale(1f, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
            }
        }

        if (detailView) detailView.ClearView();
        GenerateContractSlots();

        if (HubUIManager.Instance != null) HubUIManager.Instance.SetCursorState(false);
        if (Player.Instance != null) Player.Instance.SetControlActive(false);
    }

    private void GenerateContractSlots()
    {
        foreach (Transform child in slotsContainer) Destroy(child.gameObject);

        foreach (LevelDataSO level in availableLevels)
        {
            UI_ContractSlot newSlot = Instantiate(contractSlotPrefab, slotsContainer);
            newSlot.Setup(level, detailView);
        }
    }

    /// <summary>
    /// Closes the Contract Board UI mapping DOTween closing animation while restoring player control vectors.
    /// </summary>
    public void CloseContractUI()
    {
        if (panelCanvasGroup)
        {
            panelCanvasGroup.DOKill();
            panelCanvasGroup.DOFade(0f, 0.2f).SetUpdate(true);
            contractUIPanel.transform.DOKill(true);
            contractUIPanel.transform.DOScale(0.9f, 0.2f).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() =>
            {
                contractUIPanel.SetActive(false);
            });
        }
        else
        {
            if (contractUIPanel) contractUIPanel.SetActive(false);
        }

        if (HubUIManager.Instance != null) HubUIManager.Instance.SetCursorState(true);
        if (Player.Instance != null) Player.Instance.SetControlActive(true);
    }

    /// <summary>
    /// Validates the accepted contract binding the level data logic and alerts active event systems.
    /// </summary>
    /// <param name="selectedLevel">Data reference to the chosen contract constraints.</param>
    public void AcceptContract(LevelDataSO selectedLevel)
    {
        Debug.Log($"<color=yellow>CONTRACT SIGNED: {selectedLevel.levelDisplayName}</color>");

        ActiveMissionManager.CurrentContract = selectedLevel;

        CloseContractUI();
        OnContractAccepted?.Invoke(selectedLevel);
    }
}