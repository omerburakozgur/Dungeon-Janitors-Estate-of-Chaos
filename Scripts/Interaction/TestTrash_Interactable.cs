// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Test interactable used for development. Inherits from BaseInteractable and
/// demonstrates the Request* interaction methods.
/// </summary>
public class TestTrash_Interactable : BaseInteractable
{
 // BaseInteractable provides GetInteractionPrompt via inspector field.

 /// <summary>
 /// Short-press interaction request. Should forward a request to the authoritative manager.
 /// </summary>
 public override void RequestInteract()
 {
 // Only send a request; do not perform authoritative state changes locally.
 Debug.Log($"Interact request (short press) received. Object: {gameObject.name}");

 // Example: TrashManager.Instance.RequestCollect(this.gameObject, trashData);
 }

 /// <summary>
 /// Hold interaction request (start of hold). Intended to begin carrying/physics logic.
 /// </summary>
 public override void RequestHoldInteract()
 {
 Debug.Log($"Hold request (press-and-hold) received. Object: {gameObject.name}");

 // TODO: Replace with PhysicsCarryManager.Instance.RequestCarry(this) in later sprint
 }

 /// <summary>
 /// Release interaction request (end of hold).
 /// </summary>
 public override void RequestReleaseInteract()
 {
 // Intentionally left blank for physics carry tests
 }
}