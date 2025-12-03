// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Component that reads player cleaning input and forwards cleaning requests
/// to the authoritative CleaningManager. Performs raycasts from a configurable
/// origin while the cleaning tool is active and triggers local visual feedback.
/// </summary>
public class CleaningController : MonoBehaviour
{
    [Header("Settings")] // Inspector grouping for basic tool settings
    [SerializeField] private ToolDataSO toolData; // Tool configuration (range, power, cooldown) - serialized
    [SerializeField] private LayerMask dirtLayer; // Layer mask to identify cleanable objects - serialized
    [SerializeField] private Transform raycastOrigin; // Origin transform for forward raycasts (defaults to main camera) - serialized

    [Header("Event Listening")] // Inspector grouping for event references
    [SerializeField] private BoolEventChannelSO cleaningStateChannel; // Event channel to toggle cleaning mode externally - serialized

    [Header("Visuals")] // Inspector grouping for visual helpers
    [SerializeField] private ToolAnimator_DOTween toolAnimator; // Animator that drives cleaning tool visuals - serialized

    // Runtime state
    private bool isCleaningActive = false; // Runtime flag indicating whether cleaning mode is active - short
    private CleaningManager cleaningManager; // Cached reference to the authoritative CleaningManager singleton - short

    /// <summary>
    /// Initialize references and apply sensible defaults.
    /// If no raycast origin is assigned, the main camera transform is used.
    /// </summary>
    private void Start()
    {
        cleaningManager = CleaningManager.Instance; // cache the cleaning manager singleton - short
        if (raycastOrigin == null && Camera.main != null) raycastOrigin = Camera.main.transform; // default to main camera if not set - short
    }

    /// <summary>
    /// Subscribe to the cleaning state event channel to listen for external toggles.
    /// </summary>
    private void OnEnable()
    {
        if (cleaningStateChannel != null)
            cleaningStateChannel.OnEventRaised += SetCleaningState; // subscribe to toggle events - short
    }

    /// <summary>
    /// Unsubscribe from event channels to avoid dangling callbacks when disabled.
    /// </summary>
    private void OnDisable()
    {
        if (cleaningStateChannel != null)
            cleaningStateChannel.OnEventRaised -= SetCleaningState; // unsubscribe - short
    }

    /// <summary>
    /// External API used by player/tool managers to enable or disable cleaning mode.
    /// Updates the animator state and the local active flag.
    /// </summary>
    /// <param name="state">True to enable cleaning, false to disable.</param>
    public void SetCleaningState(bool state)
    {
        isCleaningActive = state; // update local mode flag - short

        if (toolAnimator != null)
            toolAnimator.SetCleaningState(state); // update visual feedback via animator - short

        // Future: attach audio/haptic feedback here
    }

    /// <summary>
    /// Perform per-frame checks while cleaning mode is enabled.
    /// A forward raycast is executed each frame to detect cleanable surfaces.
    /// </summary>
    private void Update()
    {
        if (isCleaningActive)
        {
            PerformCleaningRaycast(); // execute detection & forwarding - short
        }
    }

    /// <summary>
    /// Execute a forward Physics.Raycast from the configured origin using the tool's max distance.
    /// If a CleanableBase component is hit, forward the cleaning request to the CleaningManager.
    /// </summary>
    private void PerformCleaningRaycast()
    {
        if (raycastOrigin == null) return; // guard: no origin assigned - short

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, toolData.maxDistance, dirtLayer))
        {
            // If the hit has a CleanableBase, pass the clean request to the authoritative manager
            if (hit.collider.TryGetComponent<CleanableBase>(out var cleanable))
            {
                if (cleaningManager != null)
                    cleaningManager.RequestClean(cleanable, toolData); // forward cleaning request along with tool metadata - short
            }
        }
    }
}