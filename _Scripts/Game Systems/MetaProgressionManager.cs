/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Orchestrates global saved progression data validating shop unlocks, persistent currency caches,
/// and returning explicit gameplay modifying parameters natively.
/// </summary>
public class MetaProgressionManager : SingletonManager<MetaProgressionManager>
{
    [Header("Economy Data")]
    [SerializeField] private int currentGold = 0;
    [SerializeField] private int currentSalvage = 0;

    [Header("Shop Database")]
    public ShopItemSO[] allShopItems;

    [Header("Broadcasting")]
    [SerializeField] private IntEventChannelSO onGoldChanged;
    [SerializeField] private IntEventChannelSO onSalvageChanged;

    private const string KEY_GOLD = "TotalGold";
    private const string KEY_SALVAGE = "TotalSalvage";

    private void Start()
    {
        LoadData();
    }

    /// <summary>
    /// Appends banked integer values ensuring structural formatting before enforcing disk saves properly.
    /// </summary>
    public void AddGold(int amount)
    {
        currentGold += amount;
        if (currentGold < 0) currentGold = 0;

        SaveData();
        if (onGoldChanged != null) onGoldChanged.Raise(currentGold);
    }

    /// <summary>
    /// Updates available total scrap/salvage triggering correct UI refresh broadcasts explicitly.
    /// </summary>
    public void AddSalvage(int amount)
    {
        currentSalvage += amount;
        if (currentSalvage < 0) currentSalvage = 0;

        SaveData();
        if (onSalvageChanged != null) onSalvageChanged.Raise(currentSalvage);
    }

    /// <summary>Returns active raw gold integer directly.</summary>
    public int GetGold() => currentGold;

    /// <summary>Returns active salvage integer directly.</summary>
    public int GetSalvage() => currentSalvage;

    /// <summary>
    /// Processes specific upgrade evaluations verifying funds scaling values appropriately safely.
    /// </summary>
    public bool TryPurchaseItem(ShopItemSO item)
    {
        int currentLevel = GetItemLevel(item);
        int cost = CalculateCost(item, currentLevel);

        if (currentLevel >= item.maxLevel)
        {
            Debug.Log("Item already at max level.");
            return false;
        }

        if (currentGold >= cost)
        {
            AddGold(-cost);
            SetItemLevel(item, currentLevel + 1);
            SaveData();
            Debug.Log($"Purchased {item.itemName}. New Level: {currentLevel + 1}");
            return true;
        }

        Debug.Log("Not enough gold!");
        return false;
    }

    /// <summary>
    /// Extracts registered indexed tier ranks formatting keys logically.
    /// </summary>
    public int GetItemLevel(ShopItemSO item)
    {
        return PlayerPrefs.GetInt($"Upgrade_{item.ItemID}", 0);
    }

    private void SetItemLevel(ShopItemSO item, int level)
    {
        PlayerPrefs.SetInt($"Upgrade_{item.ItemID}", level);
    }

    /// <summary>
    /// Parses expected exponential price indexing values validating runtime upgrades exactly.
    /// </summary>
    public int CalculateCost(ShopItemSO item, int currentLevel)
    {
        float multiplier = Mathf.Pow(1.5f, currentLevel);
        return Mathf.RoundToInt(item.baseCost * multiplier);
    }

    /// <summary>
    /// Combines unlocked additive multipliers matching specific structural stats applying to character values naturally.
    /// </summary>
    public float GetStatMultiplier(StatType statType)
    {
        float totalMultiplier = 1.0f;
        if (allShopItems == null) return totalMultiplier;

        foreach (var item in allShopItems)
        {
            if (item.itemType == ShopItemType.StatUpgrade && item.statType == statType)
            {
                int level = GetItemLevel(item);
                totalMultiplier += (item.statMultiplierPerLevel * level);
            }
        }
        return totalMultiplier;
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt(KEY_GOLD, currentGold);
        PlayerPrefs.SetInt(KEY_SALVAGE, currentSalvage);
        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        currentGold = PlayerPrefs.GetInt(KEY_GOLD, 0);
        currentSalvage = PlayerPrefs.GetInt(KEY_SALVAGE, 0);

        if (onGoldChanged != null) onGoldChanged.Raise(currentGold);
        if (onSalvageChanged != null) onSalvageChanged.Raise(currentSalvage);
    }
}