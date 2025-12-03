// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// An interactable that allows the player to empty their trash inventory.
/// Implements IInteractable so PlayerInteraction can detect and request actions.
/// </summary>
public class TrashContainer : MonoBehaviour, IInteractable
{
 /// <summary>
 /// Interaction prompt shown to the player.
 /// </summary>
 public string GetInteractionPrompt()
 {
 return "[E] Empty Trash"; // Shown in UI
 }

 /// <summary>
 /// Request received from player to perform interaction; forwards to TrashManager.
 /// </summary>
 public void RequestInteract()
 {
 TrashManager.Instance.RequestEmptyTrash(); // Authoritative request
 }

 public void RequestHoldInteract() { }
 public void RequestReleaseInteract() { }
}