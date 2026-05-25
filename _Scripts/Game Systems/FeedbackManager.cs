/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Singleton manager handling floating impact texts, loot read-outs, VFX effects and Juicing logic.
/// </summary>
public class FeedbackManager : SingletonManager<FeedbackManager>
{
    [Header("Prefabs")]
    [SerializeField] private FloatingText damageTextPrefab;
    [SerializeField] private FloatingText goldTextPrefab;

    [Header("Settings")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private Color goldColor = Color.yellow;
    [SerializeField] private Color critColor = new Color(1f, 0.5f, 0f);

    [Header("Hit Stop Settings")]
    [Tooltip("How much Time.timeScale is reduced during hit stop effects. (0 = complete freeze)")]
    [SerializeField][Range(0f, 1f)] private float hitStopScale = 0f;

    [Header("VFX Prefabs")]
    [SerializeField] private GameObject bloodVFXPrefab;
    [SerializeField] private GameObject hitSparkPrefab;

    [Header("UI Effects")]
    [Tooltip("Assigned UI_BloodOverlay layer Canvas object reference.")]
    [SerializeField] private GameObject bloodOverlayPanel;

    private bool isFrozen = false;

    /// <summary>
    /// Instantiates floating text instances visualizing raw damage points dynamically.
    /// </summary>
    public void ShowDamage(Vector3 position, float amount, bool isCrit = false)
    {
        if (damageTextPrefab == null) return;

        Vector3 spawnPos = position + Vector3.up * 1.5f;

        FloatingText instance = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

        instance.transform.LookAt(Camera.main.transform);
        instance.transform.Rotate(0, 180, 0);

        float fontSize = isCrit ? 1.5f : 1f;
        instance.transform.localScale = Vector3.one * fontSize;

        string text = Mathf.RoundToInt(amount).ToString();
        if (isCrit) text += "!";

        instance.Setup(text, isCrit ? critColor : damageColor);
    }

    /// <summary>
    /// Renders text components reflecting acquired currencies.
    /// </summary>
    public void ShowGold(Vector3 position, int amount)
    {
        if (goldTextPrefab == null) return;

        Vector3 spawnPos = position + Vector3.up * 1.0f;
        FloatingText instance = Instantiate(goldTextPrefab, spawnPos, Quaternion.identity);

        instance.transform.LookAt(Camera.main.transform);
        instance.transform.Rotate(0, 180, 0);

        instance.Setup($"+{amount} G", goldColor);
    }

    /// <summary>
    /// Executes timescale interruptions creating impact weight 'Hit Stops' during combat engagements.
    /// </summary>
    /// <param name="duration">Realtime seconds timescale runs suppressed.</param>
    public void TriggerHitStop(float duration)
    {
        if (isFrozen) return;

        StartCoroutine(HitStopRoutine(duration));
    }

    private System.Collections.IEnumerator HitStopRoutine(float duration)
    {
        isFrozen = true;

        float originalScale = Time.timeScale;
        Time.timeScale = hitStopScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalScale;

        isFrozen = false;
    }

    /// <summary>
    /// Activates corresponding ParticleSystem instances correlating with attack types against targeted material groups.
    /// </summary>
    public void PlayHitVFX(Vector3 position, Vector3 normal, bool isOrganic = true)
    {
        GameObject prefabToSpawn = isOrganic ? bloodVFXPrefab : hitSparkPrefab;

        if (prefabToSpawn != null)
        {
            GameObject vfx = Instantiate(prefabToSpawn, position, Quaternion.LookRotation(normal));

            Destroy(vfx, 2f);
        }
    }

    /// <summary>
    /// Instigates full screen bleeding UI panels resetting animations efficiently upon trigger.
    /// </summary>
    public void ShowBloodSplat()
    {
        if (bloodOverlayPanel != null)
        {
            bloodOverlayPanel.SetActive(false);
            bloodOverlayPanel.SetActive(true);
        }
    }
}