/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Base class for data-driven interactable objects.
/// Implements IInteractable and provides a configurable interaction prompt
/// for UI systems. Attach to any GameObject that should be interactable.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    [Tooltip("Text displayed to the player indicating the interaction key or action.")]
    [SerializeField] protected string interactionPrompt = "[E] Interact";

    protected virtual void Awake()
    {
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer == -1)
        {
            Debug.LogError("Required layer named 'Interactable' not found. Please add it in Project Settings -> Tags and Layers.");
        }
        else
        {
            gameObject.layer = interactableLayer;
        }
    }

    /// <summary>
    /// Returns the configured interaction prompt string used by UI systems.
    /// </summary>
    /// <returns>The string prompt to show on the UI.</returns>
    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    /// <summary>
    /// Invoked for a one-shot (tap) interaction request.
    /// </summary>
    public abstract void RequestInteract();

    /// <summary>
    /// Invoked when a hold interaction begins.
    /// </summary>
    public abstract void RequestHoldInteract();

    /// <summary>
    /// Invoked when a hold interaction is released.
    /// </summary>
    public abstract void RequestReleaseInteract();
}