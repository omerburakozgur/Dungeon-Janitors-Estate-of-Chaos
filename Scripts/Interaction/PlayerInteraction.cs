// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Performs a forward raycast from the player's camera to discover IInteractable
/// objects and forwards input events from the InputManager as interaction requests.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform playerCameraTransform; // Player camera transform used for raycasts
    [SerializeField] private float interactRange =3f; // Maximum raycast distance
    [SerializeField] private LayerMask interactableLayer; // Layer mask for interactable objects

    [Header("UI Events")]
    [SerializeField] private StringEventChannelSO onInteractableHovered; // Broadcasts hovered prompt text

    private IInteractable currentTarget; // Currently hovered interactable
    private bool isHoldingInteract = false; // Track hold state for hold interactions

    private void Start()
    {
        if (playerCameraTransform == null)
        {
            playerCameraTransform = Camera.main.transform; // Fallback to main camera
        }
    }

    private void OnEnable()
    {
        // Subscribe to input events provided by centralized InputManager
        InputManager.Instance.OnInteractPressed += HandleInteractPressed;
        InputManager.Instance.OnHoldInteractPressed += HandleHoldInteractPressed;
        InputManager.Instance.OnHoldInteractCanceled += HandleHoldInteractCanceled;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return; // Guard for shutdown ordering
        InputManager.Instance.OnInteractPressed -= HandleInteractPressed;
        InputManager.Instance.OnHoldInteractPressed -= HandleHoldInteractPressed;
        InputManager.Instance.OnHoldInteractCanceled -= HandleHoldInteractCanceled;
    }

    void Update()
    {
        // Continuously look for an interactable target under the crosshair
        FindInteractableTarget();
    }

    /// <summary>
    /// Raycast forward from the camera to locate an IInteractable target and
    /// broadcast its interaction prompt via the configured event channel.
    /// </summary>
    private void FindInteractableTarget()
    {
        IInteractable newTarget = null;
        if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit hit, interactRange, interactableLayer))
        {
            newTarget = hit.collider.GetComponent<IInteractable>();
        }

        if (newTarget != currentTarget)
        {
            currentTarget = newTarget;

            // Broadcast prompt text for UI, empty string to clear
            if (currentTarget != null)
            {
                onInteractableHovered.Raise(currentTarget.GetInteractionPrompt());
            }
            else
            {
                onInteractableHovered.Raise("");
            }
        }
    }

    // --- Input Handlers ---

    private void HandleInteractPressed()
    {
        // Short press: forward a request to the current interactable
        if (currentTarget != null)
        {
            currentTarget.RequestInteract(); // Send request, manager will handle authority
        }
    }

    private void HandleHoldInteractPressed()
    {
        // Hold started
        isHoldingInteract = true;
        if (currentTarget != null)
        {
            currentTarget.RequestHoldInteract(); // Request hold interaction
        }
    }

    private void HandleHoldInteractCanceled()
    {
        // Hold released
        isHoldingInteract = false;
        if (currentTarget != null)
        {
            currentTarget.RequestReleaseInteract(); // Notify release if applicable
        }
    }
}