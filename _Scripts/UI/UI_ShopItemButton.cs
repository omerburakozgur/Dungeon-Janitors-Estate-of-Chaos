/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Handles individual shop item buttons, hover effects, and selection notification.
/// </summary>
public class UI_ShopItemButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Item Data")]
    public string itemName;
    [TextArea] public string itemDescription;
    public int itemPrice;
    [Tooltip("The 3D model prefab to display in the shop viewer.")]
    public GameObject item3DModelPrefab;

    [Header("Visuals & UI")]
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Color normalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color hoverColor = new Color(0.4f, 0.4f, 0.4f, 1f);

    private UI_ShopDetailView detailViewReference;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (buttonBackground) buttonBackground.color = normalColor;
    }

    private void Start()
    {
        detailViewReference = FindFirstObjectByType<UI_ShopDetailView>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.DOKill(true);
        rectTransform.DOScale(1.05f, 0.2f).SetUpdate(true).SetEase(Ease.OutBack);
        rectTransform.DOAnchorPosX(15f, 0.2f).SetUpdate(true).SetEase(Ease.InOutBack);

        if (buttonBackground)
        {
            buttonBackground.DOKill();
            buttonBackground.DOColor(hoverColor, 0.2f).SetUpdate(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.DOKill(true);
        rectTransform.DOScale(1f, 0.2f).SetUpdate(true).SetEase(Ease.InQuad);
        rectTransform.DOAnchorPosX(0f, 0.2f).SetUpdate(true).SetEase(Ease.InQuad);

        if (buttonBackground)
        {
            buttonBackground.DOKill();
            buttonBackground.DOColor(normalColor, 0.2f).SetUpdate(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        rectTransform.DOPunchScale(new Vector3(-0.1f, -0.1f, 0), 0.15f, 1).SetUpdate(true);
        NotifyDetailView();
    }

    private void NotifyDetailView()
    {
        if (detailViewReference != null)
        {
            detailViewReference.ShowItemDetails(itemName, itemDescription, itemPrice, item3DModelPrefab);
        }
    }
}