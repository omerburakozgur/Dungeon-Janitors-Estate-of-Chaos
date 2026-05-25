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
/// Represents a single item slot in the shop UI, handling hover and purchase input.
/// </summary>
public class ShopItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private Button buyButton;

    [Header("Visual Polish (DOTween)")]
    [SerializeField] private Image backgroundPanel;
    [SerializeField] private Color normalColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private ShopItemSO currentItem;
    private UI_ShopDetailView detailViewReference;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (backgroundPanel) backgroundPanel.color = normalColor;
    }

    /// <summary>
    /// Initializes the slot with item data and a reference to the detail view.
    /// </summary>
    /// <param name="item">The shop item data.</param>
    /// <param name="detailView">The detail panel to update when hovered.</param>
    public void Setup(ShopItemSO item, UI_ShopDetailView detailView)
    {
        currentItem = item;
        detailViewReference = detailView;

        nameText.text = item.itemName;
        iconImage.sprite = item.icon;

        UpdateUI();
    }

    private void UpdateUI()
    {
        // Logic to update cost and level text
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOKill(true);
        rectTransform.DOScale(1.05f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);

        if (backgroundPanel)
        {
            backgroundPanel.DOKill();
            backgroundPanel.DOColor(hoverColor, 0.2f).SetUpdate(true);
        }

        if (detailViewReference != null && currentItem != null)
        {
            detailViewReference.ShowItemDetails(currentItem.itemName, currentItem.description, currentItem.baseCost, currentItem.item3DModelPrefab);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOKill(true);
        rectTransform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.OutQuad);

        if (backgroundPanel)
        {
            backgroundPanel.DOKill();
            backgroundPanel.DOColor(normalColor, 0.2f).SetUpdate(true);
        }
    }
}