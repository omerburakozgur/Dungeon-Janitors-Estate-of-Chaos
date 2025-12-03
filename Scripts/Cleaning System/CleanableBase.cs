// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
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
    [Header("Data")] // Inspector grouping for data reference
    /// <summary>
    /// Scriptable object containing configuration for this dirt type.
    /// Serialized so designers can configure the decal, required tool ids and other metadata. // short
    /// </summary>
    public DirtDataSO dirtData; // Dirt type configuration - serialized

    [Header("Settings")] // Inspector grouping for runtime settings
    /// <summary>
    /// Initial normalized dirtiness value (range0..1).1 = fully dirty,0 = clean.
    /// This value is sampled at spawn/initialization and passed to the manager. // short
    /// </summary>
    [Range(0, 1)] public float initialDirtiness = 1.0f; // Starting dirtiness (0..1)

    /// <summary>
    /// If true, the object's collider will be disabled once the cleanable is fully cleaned
    /// to prevent further interactions. Useful for single-use decals. // short
    /// </summary>
    public bool destroyColliderOnClean = true; // Disable collider on full clean - serialized

    [Header("Visuals")] // Inspector grouping for visual references
    /// <summary>
    /// Reference to the DecalProjector used to render the dirt decal. If null the component
    /// will try to cache one in Awake. // short
    /// </summary>
    [SerializeField] private DecalProjector decalProjector; // Decal projector reference - serialized

    [Header("Visuals - World Space UI")] // Inspector grouping for world-space UI
    /// <summary>
    /// Optional world-space image used to render a progress bar above the object.
    /// This is intended to provide immediate feedback to the player. // short
    /// </summary>
    [SerializeField] private Image localProgressBarImage; // World-space progress image - serialized

    /// <summary>
    /// Optional percentage text displayed next to the local progress bar.
    /// Uses TextMeshPro for crisp UI text. // short
    /// </summary>
    [SerializeField] private TextMeshProUGUI localPercentageText; // World-space percentage text - serialized

    private bool isInitialized = false; // Tracks whether Initialize() has been executed - runtime flag

    /// <summary>
    /// Ensure required component references are cached at Awake.
    /// We attempt to cache the decal projector if it was not assigned in the inspector. // short
    /// </summary>
    private void Awake()
    {
        if (decalProjector == null) decalProjector = GetComponent<DecalProjector>(); // cache decal projector if not assigned
    }

    /// <summary>
    /// Initializes the cleanable with explicit dirt data and a starting dirtiness value.
    /// Intended to be called by spawners or managers prior to registration. // short
    /// </summary>
    /// <param name="data">Dirt type configuration to apply.</param>
    /// <param name="dirtiness">Normalized starting dirt value (0..1).</param>
    public void Initialize(DirtDataSO data, float dirtiness)
    {
        this.dirtData = data; // assign provided data reference
        this.initialDirtiness = dirtiness; // set provided starting dirt value

        // If a decal projector exists and the dirt data offers multiple materials,
        // pick one at random to add visual variety between instances. // short
        if (decalProjector != null && data.materials != null && data.materials.Count > 0)
        {
            Material randomMat = data.materials[Random.Range(0, data.materials.Count)]; // choose a random material
            decalProjector.material = randomMat; // apply chosen material to decal
        }

        isInitialized = true; // mark as initialized for later lifecycle steps
    }

    /// <summary>
    /// Register with the CleaningManager and apply initial visuals.
    /// Uses lazy initialization fallback if Initialize wasn't called manually. // short
    /// </summary>
    private void Start()
    {
        if (CleaningManager.Instance == null) return; // if authority missing, skip registration

        if (!isInitialized && dirtData != null)
        {
            Initialize(dirtData, initialDirtiness); // perform lazy initialization
        }

        CleaningManager.Instance.RegisterCleanable(this, initialDirtiness); // register with the authoritative manager
        UpdateVisuals(initialDirtiness); // set visuals to the configured initial value
    }

    /// <summary>
    /// Update the visual representation to match the provided normalized dirt value.
    /// This method is invoked by the CleaningManager whenever the dirtiness changes. // short
    /// </summary>
    /// <param name="newDirtValue">Normalized dirt value (0..1) to apply to visuals.</param>
    public void UpdateVisuals(float newDirtValue)
    {
        //1) Update decal fade factor to visually represent dirtiness
        if (decalProjector != null)
        {
            decalProjector.fadeFactor = newDirtValue; // set projector fade factor
        }

        //2) Update optional local world-space UI (progress bar & percentage)
        UpdateLocalProgressBar(newDirtValue); // update world UI elements

        //3) Optionally disable the collider when the object becomes fully clean
        if (newDirtValue <= 0 && destroyColliderOnClean)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false; // disable collider to stop interactions
        }
    }

    /// <summary>
    /// Update the local world-space progress image and optional percentage text.
    /// Keeps the UI hidden when the value is effectively zero to avoid flicker. // short
    /// </summary>
    /// <param name="newDirtValue">Normalized dirtiness (0..1) to present on the UI.</param>
    private void UpdateLocalProgressBar(float newDirtValue)
    {
        if (localProgressBarImage == null) return; // nothing to update if no UI image provided

        localProgressBarImage.fillAmount = newDirtValue; // set image fill for progress

        if (localPercentageText != null)
        {
            int percentage = Mathf.RoundToInt(newDirtValue * 100f); // convert normalized value to integer percentage
            localPercentageText.text = $"{percentage}%"; // update displayed percentage text
        }

        // Toggle visibility using a very small threshold to avoid rapid show/hide flicker
        localProgressBarImage.gameObject.SetActive(newDirtValue > 0.001f);
    }
}