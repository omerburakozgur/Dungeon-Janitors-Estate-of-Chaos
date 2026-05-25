/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
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
    string GetInteractionPrompt();

    /// <summary>
    /// Request a one-shot interaction (short press). Implementations should not perform
    /// authority-side logic here but forward the request to the appropriate manager.
    /// </summary>
    void RequestInteract();

    /// <summary>
    /// Request the start of a hold interaction (for example: begin carrying or charging an action).
    /// </summary>
    void RequestHoldInteract();

    /// <summary>
    /// Request the release of a hold interaction previously started with RequestHoldInteract.
    /// </summary>
    void RequestReleaseInteract();
}