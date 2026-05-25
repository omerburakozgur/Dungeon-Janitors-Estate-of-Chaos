/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Handles player look/aim logic by rotating the player horizontally and the
/// camera root vertically. Subscribes to InputManager look events.
/// </summary>
public class PlayerAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraRoot;

    [Header("Settings")]
    [Tooltip("Multiplier to convert slider value to screen-space sensitivity.")]
    [SerializeField] private float sensitivityMultiplier = 100f;
    [Tooltip("Maximum vertical rotation limit.")]
    [SerializeField] private float clampY = 80f;

    private float currentSensitivity = 1.0f;
    private Vector2 lookInput;
    private float xRotation;

    private void Awake()
    {
        currentSensitivity = PlayerPrefs.GetFloat(PlayerPrefsKeys.SENSITIVITY, 1.0f);
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnLookInput += HandleLookInput;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.OnLookInput -= HandleLookInput;
    }

    private void Update()
    {
        ManagePlayerLook();
    }

    /// <summary>
    /// Updates sensitivity dynamically from external settings (e.g., Options Menu).
    /// </summary>
    /// <param name="newSensitivity">The new sensitivity value.</param>
    public void UpdateSensitivity(float newSensitivity)
    {
        currentSensitivity = newSensitivity;
    }

    private void HandleLookInput(Vector2 input)
    {
        lookInput = input;
    }

    private void ManagePlayerLook()
    {
        float finalSens = currentSensitivity * sensitivityMultiplier * Time.deltaTime;

        transform.Rotate(Vector3.up * lookInput.x * finalSens);

        xRotation -= lookInput.y * finalSens;
        xRotation = Mathf.Clamp(xRotation, -clampY, clampY);

        if (cameraRoot != null)
        {
            cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}