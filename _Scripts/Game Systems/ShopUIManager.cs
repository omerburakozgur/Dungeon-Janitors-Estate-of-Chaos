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
/// Manages the in-game shop interface, animating its appearance and populating it with items 
/// retrieved from the meta-progression system.
/// </summary>
public class ShopUIManager : SingletonManager<ShopUIManager>
{
    [Header("References")]
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private CanvasGroup shopCanvasGroup;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private ShopItemSlot slotPrefab;
    [SerializeField] private TextMeshProUGUI totalGoldText;
    [SerializeField] private Button closeButton;

    [SerializeField] private UI_ShopDetailView detailView;

    protected override void Awake()
    {
        base.Awake();

        if (shopCanvasGroup == null && shopWindow != null)
            shopCanvasGroup = shopWindow.GetComponent<CanvasGroup>();

        if (shopWindow)
        {
            shopWindow.SetActive(false);
            if (shopCanvasGroup) shopCanvasGroup.alpha = 0f;
        }

        if (closeButton) closeButton.onClick.AddListener(CloseShop);
    }

    private void GenerateShopItems()
    {
        foreach (Transform child in itemsContainer)
        {
            Destroy(child.gameObject);
        }

        if (MetaProgressionManager.Instance == null) return;

        foreach (var item in MetaProgressionManager.Instance.allShopItems)
        {
            ShopItemSlot newSlot = Instantiate(slotPrefab, itemsContainer);
            newSlot.Setup(item, detailView);
        }
    }

    /// <summary>
    /// Triggers the shop interface to open, locks player movement, and populates the item grid.
    /// </summary>
    public void OpenShop()
    {
        if (shopWindow)
        {
            shopWindow.SetActive(true);

            if (shopCanvasGroup)
            {
                shopCanvasGroup.DOKill();
                shopCanvasGroup.DOFade(1f, 0.3f).SetUpdate(true);
                shopWindow.transform.DOKill(true);
                shopWindow.transform.localScale = Vector3.one * 0.9f;
                shopWindow.transform.DOScale(1f, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
            }
        }

        GenerateShopItems();
        UpdateGoldText();

        if (detailView != null) detailView.ClearView();

        if (HubUIManager.Instance != null)
        {
            HubUIManager.Instance.SetShopState(true);
            HubUIManager.Instance.SetCursorState(false);
            if (Player.Instance != null) Player.Instance.SetControlActive(false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Smoothly transitions the shop interface out, clearing visual models and restoring player control.
    /// </summary>
    public void CloseShop()
    {
        if (shopCanvasGroup)
        {
            shopCanvasGroup.DOKill();
            shopCanvasGroup.DOFade(0f, 0.2f).SetUpdate(true);
            shopWindow.transform.DOKill(true);
            shopWindow.transform.DOScale(0.9f, 0.2f).SetUpdate(true).SetEase(Ease.InQuad).OnComplete(() =>
            {
                if (shopWindow) shopWindow.SetActive(false);
                if (detailView != null) detailView.ClearView();
            });
        }
        else
        {
            if (shopWindow) shopWindow.SetActive(false);
        }

        if (HubUIManager.Instance != null)
        {
            HubUIManager.Instance.SetShopState(false);
            HubUIManager.Instance.SetCursorState(true);
            if (Player.Instance != null) Player.Instance.SetControlActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Refreshes the display text corresponding to the player's current currency reserves.
    /// </summary>
    public void UpdateGoldText()
    {
        int gold = PlayerPrefs.GetInt("TotalGold", 0);
        if (totalGoldText) totalGoldText.text = $"{gold} G";
    }
}