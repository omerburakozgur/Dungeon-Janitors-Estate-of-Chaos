/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Represents an instant-kill environmental hazard.
/// Routes fatal damage requests to the central combat authority to ensure proper death handling.
/// </summary>
public class LavaZone : MonoBehaviour
{
    [Header("Deadly Settings")]
    [Tooltip("Amount of damage applied to ensure instant death.")]
    [SerializeField] private float fatalDamage = 9999f;

    [Header("Feedback")]
    [Tooltip("Sound effect played when an entity falls into the lava.")]
    [SerializeField] private AudioClip burnSound;

    [Tooltip("AudioSource used to play the burn sound. If null, PlayClipAtPoint is used.")]
    [SerializeField] private AudioSource audioSource;

    [Tooltip("Visual effect spawned at the point of impact when an entity burns.")]
    [SerializeField] private GameObject burnVFX;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            KillPlayer(other.gameObject);
        }
    }

    /// <summary>
    /// Processes the death logic for the player, including feedback and authoritative damage routing.
    /// </summary>
    /// <param name="player">The player GameObject that entered the hazard.</param>
    private void KillPlayer(GameObject player)
    {
        if (player.TryGetComponent<IDamageable>(out var damageable))
        {
            if (damageable.IsDead()) return;

            if (burnVFX != null)
            {
                Instantiate(burnVFX, player.transform.position, Quaternion.identity);
            }

            if (audioSource != null && burnSound != null)
            {
                if (!audioSource.isPlaying) audioSource.PlayOneShot(burnSound);
            }
            else if (burnSound != null)
            {
                AudioSource.PlayClipAtPoint(burnSound, transform.position);
            }

            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.ProcessPlayerHit(damageable, fatalDamage, Vector3.zero, transform.position);
            }
        }
    }
}