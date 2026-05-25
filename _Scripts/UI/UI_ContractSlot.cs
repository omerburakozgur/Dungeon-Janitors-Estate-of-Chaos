/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// Represents an interactive contract slot in the UI.
/// </summary>
public class UI_ContractSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private Image paperBackground;

    private LevelDataSO myLevelData;
    private UI_ContractDetailView detailView;
    private RectTransform rectTransform;

    private float randomZRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Initializes the slot with level data and the reference to the detail view.
    /// </summary>
    public void Setup(LevelDataSO data, UI_ContractDetailView view)
    {
        myLevelData = data;
        detailView = view;

        if (levelNameText && data != null)
            levelNameText.text = data.levelDisplayName;

        randomZRotation = Random.Range(-4f, 4f);
        rectTransform.localRotation = Quaternion.Euler(0, 0, randomZRotation);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOKill(true);

        rectTransform.DOScale(1.1f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
        rectTransform.DORotate(Vector3.zero, 0.2f).SetUpdate(true).SetEase(Ease.OutQuad);

        if (paperBackground) paperBackground.DOColor(Color.white, 0.2f).SetUpdate(true);

        if (detailView != null && myLevelData != null)
        {
            detailView.ShowDetails(myLevelData);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOKill(true);

        rectTransform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.InQuad);
        rectTransform.DORotate(new Vector3(0, 0, randomZRotation), 0.2f).SetUpdate(true).SetEase(Ease.InQuad);

        if (paperBackground) paperBackground.DOColor(new Color(0.9f, 0.9f, 0.9f, 1f), 0.2f).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.DOPunchScale(new Vector3(-0.05f, -0.05f, 0), 0.15f).SetUpdate(true);
    }
}