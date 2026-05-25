/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Handles player cleaning input, raycasting to detect dirt, and driving dynamic
/// visual (VFX), audio feedback, and tool trails based on the surface and tool used.
/// </summary>
public class CleaningController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private ToolDataSO toolData;
    [SerializeField] private LayerMask dirtLayer;

    [Tooltip("Layer where trash resides. Required for raycast to detect trash items.")]
    [SerializeField] private LayerMask trashLayer;

    [Tooltip("UI Prompt display object on screen (e.g: [Left Click] Clean: Blood).")]
    [SerializeField] private GameObject cleaningPromptObject;
    [SerializeField] private Transform raycastOrigin;

    [Header("Event Listening")]
    [SerializeField] private BoolEventChannelSO cleaningStateChannel;

    [Tooltip("Listens to player death to lock cleaning actions.")]
    [SerializeField] private VoidEventChannelSO onPlayerDeath;

    [Header("Visuals & Animation")]
    [SerializeField] private ToolAnimator_DOTween toolAnimator;

    [Tooltip("The trail/particle controller attached to the cleaning tool itself (e.g., water splashes).")]
    [SerializeField] private WeaponTrailController toolTrail;

    private AudioSource cleaningAudioSource;
    private bool isCleaningActive = false;
    private bool isCleaningLocked = false;

    private GameObject currentVFXInstance;
    private ParticleSystem currentVFXParticle;
    private DirtDataSO currentDirtData;

    private TMPro.TextMeshProUGUI promptText;
    private UI_PopAnimator promptAnimator;
    private string lastPromptMessage = "";
    private bool isToolTrailActive = false;

    private void Awake()
    {
        if (cleaningPromptObject != null)
        {
            promptText = cleaningPromptObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            promptAnimator = cleaningPromptObject.GetComponent<UI_PopAnimator>();
        }
    }

    private void Start()
    {
        if (raycastOrigin == null && Camera.main != null) raycastOrigin = Camera.main.transform;

        cleaningAudioSource = gameObject.AddComponent<AudioSource>();
        cleaningAudioSource.loop = true;
        cleaningAudioSource.playOnAwake = false;
        cleaningAudioSource.spatialBlend = 0.1f;

        if (AudioManager.Instance != null && AudioManager.Instance.SFXMixerGroup != null)
        {
            cleaningAudioSource.outputAudioMixerGroup = AudioManager.Instance.SFXMixerGroup;
        }
    }

    private void OnEnable()
    {
        if (cleaningStateChannel != null) cleaningStateChannel.OnEventRaised += HandleCleaningState;
        if (onPlayerDeath != null) onPlayerDeath.OnEventRaised += LockCleaning;
    }

    private void OnDisable()
    {
        if (cleaningStateChannel != null) cleaningStateChannel.OnEventRaised -= HandleCleaningState;
        if (onPlayerDeath != null) onPlayerDeath.OnEventRaised -= LockCleaning;

        if (toolTrail != null) toolTrail.StopTrail();
        StopFeedback();
    }

    private void LockCleaning()
    {
        isCleaningLocked = true;
        HandleCleaningState(false);
    }

    private void HandleCleaningState(bool state)
    {
        if (isCleaningLocked) return;

        isCleaningActive = state;

        if (toolAnimator != null)
            toolAnimator.SetCleaningState(state);

        if (!state)
        {
            if (toolTrail != null) toolTrail.StopTrail();
            isToolTrailActive = false;
            StopFeedback();
        }
    }

    private void Update()
    {
        if (isCleaningLocked)
        {
            if (cleaningPromptObject != null) cleaningPromptObject.gameObject.SetActive(false);
            return;
        }

        UpdateHoverPrompt();

        if (isCleaningActive)
        {
            PerformCleaningRaycast();
        }
    }

    private void PerformCleaningRaycast()
    {
        if (raycastOrigin == null) return;

        LayerMask combinedMask = dirtLayer | trashLayer;

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, toolData.maxDistance, combinedMask))
        {
            if (hit.collider.TryGetComponent<CleanableBase>(out var cleanable))
            {
                if (cleanable.dirtData.requiredToolType == toolData.specificCleaningType)
                {
                    if (CleaningManager.Instance != null)
                    {
                        CleaningManager.Instance.RequestClean(cleanable, toolData);
                    }
                    HandleDynamicFeedback(cleanable.dirtData, hit.point, hit.normal);

                    if (toolTrail != null && !isToolTrailActive)
                    {
                        toolTrail.StartTrail();
                        isToolTrailActive = true;
                    }

                    return;
                }
            }
            else if (hit.collider.TryGetComponent<TrashItem>(out var trashItem))
            {
                if (toolData.specificCleaningType == CleaningToolType.TrashBucket)
                {
                    trashItem.SpawnFloatingText(hit.point);
                    if (TrashManager.Instance != null) TrashManager.Instance.RequestCollect(trashItem.gameObject, trashItem.trashData);

                    if (toolTrail != null)
                    {
                        toolTrail.StartTrail();
                        CancelInvoke(nameof(StopTrashTrail));
                        Invoke(nameof(StopTrashTrail), 0.25f);
                    }
                    return;
                }
            }
        }

        StopFeedback();
    }

    private void HandleDynamicFeedback(DirtDataSO dirtData, Vector3 hitPoint, Vector3 hitNormal)
    {
        if (dirtData != currentDirtData)
        {
            currentDirtData = dirtData;
            SwitchVFXPrefab(dirtData.cleaningVFX);
        }

        if (currentVFXInstance != null)
        {
            currentVFXInstance.transform.position = hitPoint;
            currentVFXInstance.transform.forward = hitNormal;

            if (currentVFXParticle != null && !currentVFXParticle.isPlaying)
            {
                currentVFXParticle.Play();
            }
        }

        AudioClip targetClip = GetAudioClipForCurrentTool();
        if (cleaningAudioSource.clip != targetClip)
        {
            cleaningAudioSource.clip = targetClip;
        }

        if (!cleaningAudioSource.isPlaying && cleaningAudioSource.clip != null)
        {
            cleaningAudioSource.time = Random.Range(0f, cleaningAudioSource.clip.length);
            cleaningAudioSource.Play();
        }
    }

    private void StopFeedback()
    {
        if (cleaningAudioSource != null && cleaningAudioSource.isPlaying)
        {
            cleaningAudioSource.Stop();
        }

        if (currentVFXParticle != null && currentVFXParticle.isPlaying)
        {
            currentVFXParticle.Stop();
        }

        if (toolTrail != null && toolData.specificCleaningType != CleaningToolType.TrashBucket && isToolTrailActive)
        {
            toolTrail.StopTrail();
            isToolTrailActive = false;
        }
    }

    private void SwitchVFXPrefab(GameObject newPrefab)
    {
        if (currentVFXInstance != null)
        {
            Destroy(currentVFXInstance);
        }

        if (newPrefab != null)
        {
            currentVFXInstance = Instantiate(newPrefab);
            currentVFXParticle = currentVFXInstance.GetComponent<ParticleSystem>();
        }
        else
        {
            currentVFXInstance = null;
            currentVFXParticle = null;
        }
    }

    private AudioClip GetAudioClipForCurrentTool()
    {
        if (AudioManager.Instance == null || AudioManager.Instance.data == null) return null;

        switch (toolData.specificCleaningType)
        {
            case CleaningToolType.Mop: return AudioManager.Instance.data.mopLoop;
            case CleaningToolType.Broom: return AudioManager.Instance.data.broomLoop;
            case CleaningToolType.Vacuum: return AudioManager.Instance.data.vacuumLoop;
            default: return AudioManager.Instance.data.defaultCleaningLoop;
        }
    }

    private void UpdateHoverPrompt()
    {
        if (raycastOrigin == null || cleaningPromptObject == null || promptText == null) return;

        LayerMask combinedMask = dirtLayer | trashLayer;
        string newPromptMessage = "";
        bool shouldShow = false;

        if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, toolData.maxDistance, combinedMask))
        {
            if (hit.collider.TryGetComponent<CleanableBase>(out var cleanable))
            {
                shouldShow = true;
                if (cleanable.dirtData.requiredToolType == toolData.specificCleaningType)
                    newPromptMessage = $"Clean: {cleanable.dirtData.dirtName}";
                else
                    newPromptMessage = $"<color=red>Wrong Tool!</color> Requires {cleanable.dirtData.requiredToolType}";
            }
            else if (hit.collider.TryGetComponent<TrashItem>(out var trashItem))
            {
                shouldShow = true;
                if (toolData.specificCleaningType == CleaningToolType.TrashBucket)
                    newPromptMessage = $"Collect: {trashItem.trashData.itemName}";
                else
                    newPromptMessage = $"<color=red>Wrong Tool!</color> Requires Trash Bucket";
            }
        }

        if (shouldShow)
        {
            if (!cleaningPromptObject.activeSelf) cleaningPromptObject.SetActive(true);

            if (newPromptMessage != lastPromptMessage)
            {
                promptText.text = newPromptMessage;
                lastPromptMessage = newPromptMessage;

                if (promptAnimator != null) promptAnimator.TriggerPopOrPunch();
            }
        }
        else
        {
            if (cleaningPromptObject.activeSelf)
            {
                cleaningPromptObject.SetActive(false);
                lastPromptMessage = "";
            }
        }
    }

    private void StopTrashTrail()
    {
        if (toolTrail != null && toolData.specificCleaningType == CleaningToolType.TrashBucket)
        {
            toolTrail.StopTrail();
        }
    }
}