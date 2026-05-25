/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

/// <summary>
/// Singleton manager responsible for handling the player's tool inventory, 
/// switching equipment, managing the holstering states, and coordinating IK/visual updates.
/// </summary>
public class PlayerToolManager : SingletonManager<PlayerToolManager>
{
    [System.Serializable]
    public struct ToolSetup
    {
        public string label;
        public ToolType category;
        public CleaningToolType specificType;
        public GameObject toolObject;
    }

    public enum ToolType
    {
        None,
        Weapon,
        Cleaning
    }

    [Header("Dependencies")]
    private InputManager inputManager;

    [Header("Arm Reparenting Settings")]
    [SerializeField] private Transform fpsArmsRoot;
    [SerializeField] private Transform cameraEquippedParent;
    [SerializeField] private Transform bodyUnequippedParent;

    [Header("Holster & IK Settings")]
    [SerializeField] private TwoBoneIKConstraint rightArmIK;
    [SerializeField] private TwoBoneIKConstraint leftArmIK;
    [SerializeField] private float ikFadeDuration = 0.3f;

    public bool isHolstered { get; private set; } = false;

    [Header("Broadcasting Channels")]
    [SerializeField] private VoidEventChannelSO onCombatAttackChannel;
    [SerializeField] private BoolEventChannelSO onCleaningStateChannel;

    [Header("Tools in Inventory")]
    [SerializeField] private List<ToolSetup> availableTools = new List<ToolSetup>();

    private GameObject currentActiveToolModel = null;
    private int currentToolIndex = 0;

    [Header("State")]
    public ToolType currentCategory = ToolType.Weapon;
    public CleaningToolType currentSpecificTool = CleaningToolType.None;

    private Vector3 originalArmsLocalPos;
    private Quaternion originalArmsLocalRot;

    /// <summary>
    /// Initializes singleton logic, input dependencies, records arm offsets, and disables tools by default.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        inputManager = InputManager.Instance;

        if (fpsArmsRoot != null)
        {
            originalArmsLocalPos = fpsArmsRoot.localPosition;
            originalArmsLocalRot = fpsArmsRoot.localRotation;
        }

        foreach (var setup in availableTools)
        {
            if (setup.toolObject != null) setup.toolObject.SetActive(false);
        }
    }

    private void Start()
    {
        SelectToolByIndex(0);

        // Instantly hide it upon game start to ensure player spawns unarmed (Holster Mode)
        if (!isHolstered)
        {
            ToggleHolster();
        }
    }

    private void OnEnable()
    {
        if (inputManager != null)
        {
            inputManager.OnHotbar1Pressed += () => SelectToolByIndex(0);
            inputManager.OnHotbar2Pressed += () => SelectToolByIndex(1);
            inputManager.OnHotbar3Pressed += () => SelectToolByIndex(2);
            inputManager.OnHotbar4Pressed += () => SelectToolByIndex(3);
            inputManager.OnHotbar5Pressed += () => SelectToolByIndex(4);
            inputManager.OnHotbar6Pressed += () => SelectToolByIndex(5);

            inputManager.OnScrollInput += HandleScrollInput;
            inputManager.OnHolsterPressed += ToggleHolster;
            inputManager.OnAttackPressed += HandleAttackPressed;
            inputManager.OnAttackHeldStatus += HandleAttackHeldStatus;
        }
    }

    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnHotbar1Pressed -= () => SelectToolByIndex(0);
            inputManager.OnHotbar2Pressed -= () => SelectToolByIndex(1);
            inputManager.OnHotbar3Pressed -= () => SelectToolByIndex(2);
            inputManager.OnHotbar4Pressed -= () => SelectToolByIndex(3);
            inputManager.OnHotbar5Pressed -= () => SelectToolByIndex(4);
            inputManager.OnHotbar6Pressed -= () => SelectToolByIndex(5);

            inputManager.OnScrollInput -= HandleScrollInput;
            inputManager.OnHolsterPressed -= ToggleHolster;
            inputManager.OnAttackPressed -= HandleAttackPressed;
            inputManager.OnAttackHeldStatus -= HandleAttackHeldStatus;
        }
    }

    private void HandleScrollInput(float scrollDelta)
    {
        if (availableTools.Count <= 1) return;

        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            int newIndex = currentToolIndex;
            if (scrollDelta > 0) newIndex = (currentToolIndex + 1) % availableTools.Count;
            else newIndex = (currentToolIndex - 1 + availableTools.Count) % availableTools.Count;

            SelectToolByIndex(newIndex);
        }
    }

    private void SelectToolByIndex(int index)
    {
        if (index < 0 || index >= availableTools.Count) return;

        currentToolIndex = index;
        ToolSetup selected = availableTools[index];
        SwitchTool(selected.category, selected.specificType);
    }

    /// <summary>
    /// Processes the state transition between different tools and unholsters automatically.
    /// </summary>
    /// <param name="newCategory">The overarching type of the new tool.</param>
    /// <param name="specificTool">The exact enum value referencing the unique tool.</param>
    public void SwitchTool(ToolType newCategory, CleaningToolType specificTool)
    {
        if (currentCategory == newCategory && currentSpecificTool == specificTool && !isHolstered) return;

        if (currentCategory == ToolType.Cleaning) onCleaningStateChannel?.Raise(false);

        currentCategory = newCategory;
        currentSpecificTool = specificTool;

        if (isHolstered) ToggleHolster();
        else
        {
            RefreshToolVisibility();
            UpdateIKWeights(1f);
        }
    }

    /// <summary>
    /// Transitions the player character between carrying tools and an empty-handed stance.
    /// </summary>
    public void ToggleHolster()
    {
        isHolstered = !isHolstered;
        fpsArmsRoot.DOKill();

        if (isHolstered)
        {
            UpdateIKWeights(0f);
            if (currentCategory == ToolType.Cleaning) onCleaningStateChannel?.Raise(false);

            fpsArmsRoot.SetParent(bodyUnequippedParent, true);
            fpsArmsRoot.DOLocalMove(Vector3.zero, ikFadeDuration).SetEase(Ease.InOutSine);
            fpsArmsRoot.DOLocalRotateQuaternion(Quaternion.identity, ikFadeDuration).SetEase(Ease.InOutSine);

            DOVirtual.DelayedCall(ikFadeDuration * 0.8f, () => {
                if (isHolstered && currentActiveToolModel != null) currentActiveToolModel.SetActive(false);
            });
        }
        else
        {
            RefreshToolVisibility();
            fpsArmsRoot.SetParent(cameraEquippedParent, true);

            // Instead of snapping to Vector3.zero, target the correctly cached original local values
            fpsArmsRoot.DOLocalMove(originalArmsLocalPos, ikFadeDuration).SetEase(Ease.InOutSine);
            fpsArmsRoot.DOLocalRotateQuaternion(originalArmsLocalRot, ikFadeDuration).SetEase(Ease.InOutSine);

            UpdateIKWeights(1f);
        }
    }

    private void UpdateIKWeights(float targetWeight)
    {
        if (rightArmIK != null) DOTween.To(() => rightArmIK.weight, x => rightArmIK.weight = x, targetWeight, ikFadeDuration);
        if (leftArmIK != null) DOTween.To(() => leftArmIK.weight, x => leftArmIK.weight = x, targetWeight, ikFadeDuration);
    }

    private void RefreshToolVisibility()
    {
        if (isHolstered) return;
        if (currentActiveToolModel != null) currentActiveToolModel.SetActive(false);

        foreach (var setup in availableTools)
        {
            if (setup.category == currentCategory && setup.specificType == currentSpecificTool)
            {
                currentActiveToolModel = setup.toolObject;
                if (currentActiveToolModel != null) currentActiveToolModel.SetActive(true);
                break;
            }
        }
    }

    private void HandleAttackPressed() { if (!isHolstered && currentCategory == ToolType.Weapon) onCombatAttackChannel?.Raise(); }

    private void HandleAttackHeldStatus(bool isHeld) { if (!isHolstered && currentCategory == ToolType.Cleaning) onCleaningStateChannel?.Raise(isHeld); }

    /// <summary>
    /// Evaluates if the specific "TrashBucket" cleaning tool is actively equipped and in the player's hands.
    /// </summary>
    /// <returns>True if the trash bucket is the current drawn tool.</returns>
    public bool IsTrashBucketEquipped() => !isHolstered && currentCategory == ToolType.Cleaning && currentSpecificTool == CleaningToolType.TrashBucket;
}