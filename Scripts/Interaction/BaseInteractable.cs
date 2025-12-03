// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Base class for data-driven interactable objects.
/// Implements IInteractable and provides a configurable interaction prompt
/// for UI systems. Attach to any GameObject that should be interactable. // short
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")] // Inspector grouping for interaction configuration
    [Tooltip("Text displayed to the player indicating the interaction key or action.")]
    [SerializeField] protected string interactionPrompt = "[E] Interact"; // Prompt shown in UI - serialized

    protected virtual void Awake()
    {
        // Ensure this GameObject is placed on the dedicated 'Interactable' layer used by raycasting
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer == -1)
        {
            Debug.LogError("Required layer named 'Interactable' not found. Please add it in Project Settings -> Tags and Layers."); // Designer-friendly error - short
        }
        else
        {
            gameObject.layer = interactableLayer; // Assign the interactable layer - short
        }
    }

    /// <summary>
    /// Returns the configured interaction prompt string used by UI systems.
    /// </summary>
    public virtual string GetInteractionPrompt()
    {
        return interactionPrompt; // Provide prompt for display - short
    }

    // The following methods are abstract to force concrete implementations in derived classes.
    public abstract void RequestInteract(); // One-shot interaction (tap) - short
    public abstract void RequestHoldInteract(); // Begin hold interaction - short
    public abstract void RequestReleaseInteract(); // End hold interaction - short
}