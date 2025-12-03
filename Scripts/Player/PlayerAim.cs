// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Handles player look/aim logic by rotating the player horizontally and the
/// camera root vertically. Subscribes to InputManager look events to receive deltas. // short
/// </summary>
public class PlayerAim : MonoBehaviour
{
    [Header("References")] // Reference to the camera root used for vertical rotation
    [SerializeField] private Transform cameraRoot; // Transform rotated on the X axis - serialized

    [Header("Settings")] // Sensitivity and vertical clamp
    [SerializeField] private float sensitivity = 2f; // Look sensitivity multiplier - serialized
    [SerializeField] private float clampY = 80f; // Vertical clamp in degrees - serialized

    // State variables used to store current input and rotation
    private Vector2 lookInput; // Cached look input from InputManager - runtime
    private float xRotation; // Current vertical rotation angle - runtime

    private void OnEnable()
    {
        InputManager.Instance.OnLookInput += HandleLookInput; // subscribe to look input - short
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnLookInput -= HandleLookInput; // unsubscribe - short
        }
    }

    private void HandleLookInput(Vector2 input)
    {
        // Store the look input; actual rotation applied in Update for deterministic timing
        lookInput = input; // short
    }

    void Update()
    {
        ManagePlayerLook(); // apply stored look input each frame - short
    }

    private void ManagePlayerLook()
    {
        // Horizontal rotation: rotate the player transform around Y axis
        transform.Rotate(Vector3.up * lookInput.x * sensitivity * Time.deltaTime); // yaw - short

        // Vertical rotation: adjust camera root pitch and clamp it
        xRotation -= lookInput.y * sensitivity * Time.deltaTime; // invert typical mouse Y for natural feel - short
        xRotation = Mathf.Clamp(xRotation, -clampY, clampY); // clamp pitch - short

        cameraRoot.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // apply pitch - short
    }
}