/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

/// <summary>
/// Manages the radial selection tool wheel, allowing the player to seamlessly 
/// switch equipment by translating mouse directional input into slotted selections.
/// </summary>
public class ToolWheelManager : SingletonManager<ToolWheelManager>
{
    public static event System.Action<bool> OnWheelStateChanged;

    [Header("UI References")]
    [SerializeField] private GameObject wheelPanel;

    [Tooltip("Drag the parent object holding the slots and center text here. Do NOT drag the Background Dimmer!")]
    [SerializeField] private RectTransform wheelContainer;

    [SerializeField] private RectTransform selectionPointer;
    [SerializeField] private List<ToolWheelSlot> slots = new List<ToolWheelSlot>();

    [Header("Visual Polish References")]
    [SerializeField] private Image backgroundDimmer;
    [SerializeField] private TextMeshProUGUI centerToolNameText;

    [Header("Cartoony Pop Settings (DOTween)")]
    [SerializeField] private float popDuration = 0.25f;
    [SerializeField] private float hiddenScale = 0.3f;
    [SerializeField] private float visibleScale = 1.0f;
    [SerializeField] private float popOvershoot = 1.15f;

    [Header("Settings - Interaction")]
    [SerializeField] private float slowMotionScale = 0.1f;
    [SerializeField] private float mouseSensitivity = 5f;
    [SerializeField] private float minimumDeadzone = 10f;
    [SerializeField] private float maximumDistance = 50f;
    [SerializeField] private float pointerRotationOffset = 0f;

    private bool isWheelOpen = false;
    private Vector2 virtualMousePosition = Vector2.zero;
    private int currentlySelectedSlotIndex = -1;

    private void Start()
    {
        if (wheelPanel != null) wheelPanel.SetActive(false);
        if (backgroundDimmer != null) backgroundDimmer.color = new Color(0, 0, 0, 0);
        if (centerToolNameText != null) centerToolNameText.text = "";

        if (wheelContainer != null) wheelContainer.localScale = Vector3.one * hiddenScale;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.GetCurrentState() == GameState.Paused)
        {
            if (isWheelOpen) CloseWheelAndEquip(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Tab)) OpenWheel();
        else if (Input.GetKeyUp(KeyCode.Tab)) CloseWheelAndEquip(true);

        if (isWheelOpen) CalculateSelection();
    }

    private void OpenWheel()
    {
        isWheelOpen = true;
        OnWheelStateChanged?.Invoke(true);

        virtualMousePosition = Vector2.zero;
        currentlySelectedSlotIndex = -1;

        if (selectionPointer != null) selectionPointer.localRotation = Quaternion.Euler(0, 0, pointerRotationOffset);
        if (centerToolNameText != null) centerToolNameText.text = "";
        if (wheelPanel != null) wheelPanel.SetActive(true);

        if (backgroundDimmer != null)
        {
            backgroundDimmer.DOKill();
            backgroundDimmer.DOFade(0.6f, popDuration).SetUpdate(true);
        }

        if (wheelContainer != null)
        {
            wheelContainer.DOKill(true);
            wheelContainer.localScale = Vector3.one * hiddenScale;
            wheelContainer.DOScale(Vector3.one * visibleScale, popDuration).SetUpdate(true).SetEase(Ease.OutBack, popOvershoot);
        }

        Time.timeScale = slowMotionScale;
    }

    private void CloseWheelAndEquip(bool equipSelected)
    {
        isWheelOpen = false;
        OnWheelStateChanged?.Invoke(false);

        if (wheelContainer != null)
        {
            wheelContainer.DOKill(true);
            wheelContainer.DOScale(Vector3.one * hiddenScale, 0.2f).SetUpdate(true).SetEase(Ease.InBack);
        }

        if (backgroundDimmer != null)
        {
            backgroundDimmer.DOKill();
            backgroundDimmer.DOFade(0f, 0.2f).SetUpdate(true).OnComplete(() =>
            {
                if (wheelPanel != null) wheelPanel.SetActive(false);
            });
        }
        else
        {
            if (wheelPanel != null) wheelPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        foreach (var slot in slots) slot.Unhover();

        if (!equipSelected || currentlySelectedSlotIndex == -1) return;
        if (PlayerToolManager.Instance == null) return;

        PlayerToolManager.ToolType selectedCategory = slots[currentlySelectedSlotIndex].toolCategory;
        CleaningToolType selectedSpecificTool = slots[currentlySelectedSlotIndex].specificCleaningType;

        PlayerToolManager.Instance.SwitchTool(selectedCategory, selectedSpecificTool);
    }

    private void CalculateSelection()
    {
        if (slots.Count == 0) return;

        virtualMousePosition.x += Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        virtualMousePosition.y += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        float distance = virtualMousePosition.magnitude;

        if (distance > maximumDistance)
        {
            virtualMousePosition = virtualMousePosition.normalized * maximumDistance;
            distance = maximumDistance;
        }

        if (distance < minimumDeadzone) return;

        Vector2 direction = virtualMousePosition.normalized;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360f;

        if (selectionPointer != null)
            selectionPointer.localRotation = Quaternion.Euler(0, 0, -angle + pointerRotationOffset);

        float sliceSize = 360f / slots.Count;
        int newSlotIndex = Mathf.RoundToInt(angle / sliceSize) % slots.Count;

        if (newSlotIndex != currentlySelectedSlotIndex)
        {
            if (currentlySelectedSlotIndex != -1) slots[currentlySelectedSlotIndex].Unhover();

            currentlySelectedSlotIndex = newSlotIndex;
            slots[currentlySelectedSlotIndex].Hover();

            if (centerToolNameText != null)
            {
                centerToolNameText.text = slots[currentlySelectedSlotIndex].toolDisplayName;
                centerToolNameText.transform.DOKill();
                centerToolNameText.transform.localScale = Vector3.one * 0.8f;
                centerToolNameText.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }
    }
}