using UnityEngine;

/// <summary>
/// Yerde duran ve toplanabilen değerli eşya.
/// TDD-SYS-003: Toplandığında MetaProgressionManager'a değerini ekler.
/// </summary>
public class LootItem : BaseInteractable
{
    [Header("Loot Data")]
    [SerializeField] private LootDataSO lootData;

    // UI'da görünecek isim (Örn: "[E] Pick up: Gold Chalice")
    public override string GetInteractionPrompt()
    {
        string itemName = lootData != null ? lootData.itemName : "Unknown Item";
        return $"[E] Pick up: {itemName}";
    }

    public override void RequestInteract()
    {
        if (lootData == null) return;

        // --- AUDIO EKLE ---
        if (AudioManager.Instance && AudioManager.Instance.data)
        {
            // UI sesi olarak çalar (Net duyulur)
            AudioManager.Instance.PlayUI(AudioManager.Instance.data.lootPickup, 0.8f);

        }
        // YENİ: LevelManager'ın geçici cüzdanına ekle
        if (LevelManager.Instance != null)
        {
            // Şimdilik sadece Gold ekliyoruz, salvage 0
            LevelManager.Instance.AddSessionLoot(lootData.lootValue, 0);

            if (FeedbackManager.Instance != null)
            {
                FeedbackManager.Instance.ShowGold(transform.position, lootData.lootValue);
            }

            // Log
            Debug.Log($"Collected (At Risk): {lootData.itemName} (+{lootData.lootValue} G)");
        }

        Destroy(gameObject);
    }

    public override void RequestHoldInteract() { }
    public override void RequestReleaseInteract() { }
}