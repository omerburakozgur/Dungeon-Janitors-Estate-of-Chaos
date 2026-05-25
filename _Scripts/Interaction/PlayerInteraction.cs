/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Performs a forward raycast from the player's camera to discover IInteractable
/// objects and forwards input events from the InputManager as interaction requests.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableLayer;

    [Header("UI Events")]
    [SerializeField] private StringEventChannelSO onInteractableHovered;

    private IInteractable currentTarget;
    private string lastSentPrompt = "";
    private bool isHoldingInteract = false;

    private void Start()
    {
        if (playerCameraTransform == null)
        {
            playerCameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnInteractPressed += HandleInteractPressed;
        InputManager.Instance.OnHoldInteractPressed += HandleHoldInteractPressed;
        InputManager.Instance.OnHoldInteractCanceled += HandleHoldInteractCanceled;
    }

    private void OnDisable()
    {
        if (InputManager.Instance == null) return;
        InputManager.Instance.OnInteractPressed -= HandleInteractPressed;
        InputManager.Instance.OnHoldInteractPressed -= HandleHoldInteractPressed;
        InputManager.Instance.OnHoldInteractCanceled -= HandleHoldInteractCanceled;
    }

    private void Update()
    {
        if (playerCameraTransform == null) return;

        if (!isHoldingInteract)
        {
            CheckForInteractables();
        }
    }

    private void CheckForInteractables()
    {
        Ray ray = new Ray(playerCameraTransform.position, playerCameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactRange, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable != currentTarget)
            {
                currentTarget = interactable;
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget = null;
                lastSentPrompt = "";
                onInteractableHovered.Raise("");
                return;
            }
        }

        if (currentTarget != null)
        {
            string currentPrompt = currentTarget.GetInteractionPrompt();

            if (currentPrompt != lastSentPrompt)
            {
                lastSentPrompt = currentPrompt;
                onInteractableHovered.Raise(currentPrompt);
            }
        }
    }

    private void HandleInteractPressed()
    {
        if (PhysicsCarryManager.Instance != null && PhysicsCarryManager.Instance.IsCarrying())
        {
            PhysicsCarryManager.Instance.DropObject();
            return;
        }

        if (currentTarget != null)
        {
            currentTarget.RequestInteract();
        }
    }

    private void HandleHoldInteractPressed()
    {
        isHoldingInteract = true;
        if (currentTarget != null)
        {
            currentTarget.RequestHoldInteract();
        }
    }

    private void HandleHoldInteractCanceled()
    {
        isHoldingInteract = false;
        if (currentTarget != null)
        {
            currentTarget.RequestReleaseInteract();
        }
    }
}