/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Represents a consumable health potion that restores player health upon interaction.
/// </summary>
public class HealthPotion : BaseInteractable
{
    [Header("Potion Settings")]
    [Tooltip("Amount of health restored when consumed.")]
    [SerializeField] private float healAmount = 25f;

    [SerializeField] private string itemName = "Health Potion";

    /// <summary>
    /// Gets the formatted interaction prompt for the UI.
    /// </summary>
    public override string GetInteractionPrompt()
    {
        return $"[E] Drink {itemName}";
    }

    /// <summary>
    /// Initiates the healing process when the player interacts with the potion.
    /// </summary>
    public override void RequestInteract()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.Heal(healAmount);

                if (AudioManager.Instance && AudioManager.Instance.data)
                {
                    AudioManager.Instance.PlayUI(AudioManager.Instance.data.potionDrink);
                }

                Destroy(gameObject);
            }
        }
    }

    public override void RequestHoldInteract() { }
    public override void RequestReleaseInteract() { }
}