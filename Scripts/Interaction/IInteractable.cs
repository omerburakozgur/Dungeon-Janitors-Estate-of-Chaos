// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Interface that marks objects as interactable by the player.
/// Implement this on any GameObject that should expose an interaction prompt
/// and respond to interaction requests routed from central managers.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Returns a short UI prompt that explains how the player can interact with this object
    /// (for example: "[E] Pick up: Bone").
    /// </summary>
    string GetInteractionPrompt(); // Provide a concise prompt for display

    /// <summary>
    /// Request a one-shot interaction (short press). Implementations should not perform
    /// authority-side logic here but forward the request to the appropriate manager.
    /// </summary>
    void RequestInteract(); // One-shot interaction request

    /// <summary>
    /// Request the start of a hold interaction (for example: begin carrying or charging an action).
    /// </summary>
    void RequestHoldInteract(); // Begin hold interaction

    /// <summary>
    /// Request the release of a hold interaction previously started with RequestHoldInteract.
    /// </summary>
    void RequestReleaseInteract(); // End hold interaction
}