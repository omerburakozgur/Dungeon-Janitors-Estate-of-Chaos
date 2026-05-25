/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Component representing a world-space cleanable decal/object.
/// Provides a concise public API used by the authoritative cleaning system
/// and manages the visual representation (decal fade and optional world UI).
/// </summary>
public class CleanableBase : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("Scriptable object containing configuration for this dirt type.")]
    public DirtDataSO dirtData;

    [Header("Settings")]
    [Tooltip("Initial normalized dirtiness value (range 0..1). 1 = fully dirty, 0 = clean. Sampled at spawn.")]
    [Range(0, 1)] public float initialDirtiness = 1.0f;

    [Tooltip("If true, the object's collider will be disabled once fully cleaned to prevent further interactions.")]
    public bool destroyColliderOnClean = true;

    [Header("Visuals")]
    [Tooltip("Reference to the DecalProjector used to render the dirt decal.")]
    [SerializeField] private DecalProjector decalProjector;

    [Header("Visuals - World Space UI")]
    [Tooltip("Optional world-space image used to render a progress bar above the object.")]
    [SerializeField] private Image localProgressBarImage;

    [Tooltip("Optional percentage text displayed next to the local progress bar.")]
    [SerializeField] private TextMeshProUGUI localPercentageText;

    private bool isInitialized = false;

    private void Awake()
    {
        if (decalProjector == null)
            decalProjector = GetComponent<DecalProjector>();
    }

    /// <summary>
    /// Initializes the cleanable with explicit dirt data and a starting dirtiness value.
    /// Intended to be called by spawners or managers prior to registration.
    /// </summary>
    /// <param name="data">Dirt type configuration to apply.</param>
    /// <param name="startingDirtiness">Normalized starting dirt value (0..1).</param>
    public void Initialize(DirtDataSO data, float startingDirtiness)
    {
        dirtData = data;
        initialDirtiness = Mathf.Clamp01(startingDirtiness);

        if (CleaningManager.Instance != null)
        {
            // Future registration logic
        }

        UpdateLocalProgressBar(initialDirtiness);
        isInitialized = true;
    }

    private void Start()
    {
        if (CleaningManager.Instance == null) return;

        if (!isInitialized && dirtData != null)
        {
            Initialize(dirtData, initialDirtiness);
        }

        CleaningManager.Instance.RegisterCleanable(this, initialDirtiness);
        UpdateVisuals(initialDirtiness);
    }

    /// <summary>
    /// Update the visual representation to match the provided normalized dirt value.
    /// Invoked by the CleaningManager whenever the dirtiness changes.
    /// </summary>
    /// <param name="newDirtValue">Normalized dirt value (0..1) to apply to visuals.</param>
    public void UpdateVisuals(float newDirtValue)
    {
        if (decalProjector != null)
        {
            decalProjector.fadeFactor = newDirtValue;
        }

        UpdateLocalProgressBar(newDirtValue);

        if (newDirtValue <= 0 && destroyColliderOnClean)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    private void UpdateLocalProgressBar(float newDirtValue)
    {
        if (localProgressBarImage == null) return;

        localProgressBarImage.fillAmount = newDirtValue;

        if (localPercentageText != null)
        {
            int percentage = Mathf.RoundToInt(newDirtValue * 100f);
            localPercentageText.text = $"{percentage}%";
        }

        localProgressBarImage.gameObject.SetActive(newDirtValue > 0.001f);
    }
}