/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Manages the visual details and 3D preview of shop items.
/// </summary>
public class UI_ShopDetailView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemStatsText;
    [SerializeField] private TextMeshProUGUI itemPriceText;
    [SerializeField] private RawImage modelDisplayImage;

    [Header("3D Showcase Setup")]
    [Tooltip("The camera rendering the 3D model for the shop preview.")]
    [SerializeField] private Camera showcaseCamera;
    [SerializeField] private Transform showcaseSpawnPoint;
    [SerializeField] private float rotationSpeed = 45f;
    [Tooltip("Name of the layer used strictly for 3D UI objects.")]
    [SerializeField] private string ui3DLayerName = "UI_3D";

    [Header("Auto-Framing Settings")]
    [Tooltip("Multiplier for the camera distance relative to object size.")]
    [SerializeField] private float framingPadding = 1.2f;

    private GameObject current3DModel;
    private Tween typewriterTween;
    private int ui3DLayer;

    private void Awake()
    {
        ui3DLayer = LayerMask.NameToLayer(ui3DLayerName);
        ClearView();
    }

    private void Update()
    {
        if (current3DModel != null)
        {
            current3DModel.transform.Rotate(Vector3.up * rotationSpeed * Time.unscaledDeltaTime, Space.World);
        }
    }

    /// <summary>
    /// Displays item information and updates the 3D model preview.
    /// </summary>
    public void ShowItemDetails(string itemName, string description, int price, GameObject modelPrefab)
    {
        itemNameText.text = itemName;
        itemPriceText.text = $"{price} Gold";
        itemStatsText.text = "";

        if (typewriterTween != null) typewriterTween.Kill();
        int textLength = description.Length;
        typewriterTween = DOTween.To(() => 0, x => itemStatsText.text = description.Substring(0, x), textLength, 1.5f)
            .SetUpdate(true)
            .SetEase(Ease.Linear);

        if (current3DModel != null)
        {
            Destroy(current3DModel);
        }

        if (modelPrefab != null && showcaseSpawnPoint != null)
        {
            current3DModel = Instantiate(modelPrefab, showcaseSpawnPoint);

            Collider col = current3DModel.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            SetLayerRecursively(current3DModel, ui3DLayer);

            FrameObjectAndCamera(current3DModel);
        }

        modelDisplayImage.transform.DOKill();
        modelDisplayImage.transform.localScale = Vector3.one * 0.8f;
        modelDisplayImage.transform.DOScale(1f, 0.3f).SetUpdate(true).SetEase(Ease.OutBack);
    }

    private void FrameObjectAndCamera(GameObject targetObj)
    {
        if (showcaseCamera == null) return;

        Renderer[] renderers = targetObj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // Center the object based on its geometric bounds
        targetObj.transform.position = showcaseSpawnPoint.position - bounds.center + targetObj.transform.position;

        bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);

        // Calculate distance based on FOV and object extent to frame the shot
        float objectSize = bounds.extents.magnitude;
        float cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * showcaseCamera.fieldOfView);
        float distance = (objectSize * framingPadding) / cameraView;

        showcaseCamera.transform.position = bounds.center - (showcaseSpawnPoint.forward * distance);
        showcaseCamera.transform.LookAt(bounds.center);
    }

    /// <summary>
    /// Resets the view to the default state.
    /// </summary>
    public void ClearView()
    {
        itemNameText.text = "SELECT AN ITEM";
        itemStatsText.text = "Waiting for selection...";
        itemPriceText.text = "-";

        if (current3DModel != null) Destroy(current3DModel);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, newLayer);
    }

    private void OnDisable() { if (typewriterTween != null) typewriterTween.Kill(); }
}