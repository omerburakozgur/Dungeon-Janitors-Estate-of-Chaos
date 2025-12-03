// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using TMPro;
using UnityEngine;

/// <summary>
/// Listener that receives prompt text via a StringEventChannelSO and updates
/// a child TextMeshProUGUI element. This component remains active while
/// only the visual is toggled on/off.
/// </summary>
public class UI_InteractionPrompt : MonoBehaviour
{
 [Header("Listening")]
 [SerializeField] private StringEventChannelSO onInteractableHovered; // Shared channel used by PlayerInteraction

 [Header("Target Visual")]
 [SerializeField] private TextMeshProUGUI promptTextElement; // Child text element to control

 private void Awake()
 {
 if (promptTextElement != null) promptTextElement.gameObject.SetActive(false); // Hide initially
 }

 private void OnEnable()
 {
 if (onInteractableHovered != null) onInteractableHovered.OnEventRaised += UpdatePrompt; // Subscribe
 }

 private void OnDisable()
 {
 if (onInteractableHovered != null) onInteractableHovered.OnEventRaised -= UpdatePrompt; // Unsubscribe
 }

 /// <summary>
 /// Update the prompt text and visibility when the event is raised.
 /// </summary>
 private void UpdatePrompt(string newText)
 {
 if (promptTextElement == null) return;
 if (string.IsNullOrEmpty(newText))
 {
 promptTextElement.gameObject.SetActive(false); // Hide when empty
 }
 else
 {
 promptTextElement.gameObject.SetActive(true); // Show and update text
 promptTextElement.text = newText;
 }
 }
}