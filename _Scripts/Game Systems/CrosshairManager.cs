/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls crosshair animation sizing and coloring parameters based on currently hovered layer interactions dynamically.
/// </summary>
public class CrosshairManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Target visual Crosshair Image residing within main UI canvas hierarchical element.")]
    [SerializeField] private Image crosshairImage;

    [Header("Settings")]
    [Tooltip("How far forward checking logic extends from center point of main perspective lens.")]
    [SerializeField] private float checkDistance = 10f;
    [Tooltip("LayerMask targets defining objects validating logic hits during interaction sequence checks.")]
    [SerializeField] private LayerMask checkLayers;

    [Header("Colors")]
    [Tooltip("Neutral resting state Crosshair appearance color scheme format.")]
    [SerializeField] private Color defaultColor = new Color(1f, 1f, 1f, 0.5f);
    [Tooltip("Detected valid attackable enemy entity state display color format.")]
    [SerializeField] private Color enemyColor = Color.red;
    [Tooltip("Detected Loot or general interactive logic hit condition color display output.")]
    [SerializeField] private Color interactColor = Color.yellow;

    [Header("Juice (Scale Punch)")]
    [Tooltip("Target scaling coefficient simulating punch animation upon acquiring interactive visual lock.")]
    [SerializeField] private float punchScale = 1.5f;
    [Tooltip("Contraction return pacing reverting UI punch scale size back to standard dimensional vectors.")]
    [SerializeField] private float smoothTime = 10f;

    private Transform mainCamera;
    private Color targetColor;
    private float currentScale = 1f;

    private CrosshairState currentState = CrosshairState.Default;
    private enum CrosshairState { Default, Enemy, Interactable }

    private void Awake()
    {
        if (Camera.main != null) mainCamera = Camera.main.transform;
        targetColor = defaultColor;
    }

    private void Update()
    {
        CheckTarget();
        UpdateVisuals();
    }

    private void CheckTarget()
    {
        if (mainCamera == null) return;

        Ray ray = new Ray(mainCamera.position, mainCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, checkDistance, checkLayers))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out IDamageable enemy))
            {
                SetState(CrosshairState.Enemy);
                return;
            }

            if (hit.collider.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                SetState(CrosshairState.Interactable);
                return;
            }
        }

        SetState(CrosshairState.Default);
    }

    private void SetState(CrosshairState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        currentScale = punchScale;

        switch (newState)
        {
            case CrosshairState.Default:
                targetColor = defaultColor;
                break;
            case CrosshairState.Enemy:
                targetColor = enemyColor;
                break;
            case CrosshairState.Interactable:
                targetColor = interactColor;
                break;
        }
    }

    private void UpdateVisuals()
    {
        if (crosshairImage == null) return;

        crosshairImage.color = Color.Lerp(crosshairImage.color, targetColor, Time.deltaTime * 10f);

        currentScale = Mathf.Lerp(currentScale, 1f, Time.deltaTime * smoothTime);
        crosshairImage.transform.localScale = Vector3.one * currentScale;
    }
}