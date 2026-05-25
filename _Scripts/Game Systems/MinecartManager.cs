/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;
using UnityEngine.Splines;
using Unity.Cinemachine;

/// <summary>
/// Controls the minecart's departure sequence, verifying seat occupancy and managing
/// cinematic spline animations, camera field of view, and audio transitions.
/// </summary>
public class MinecartManager : MonoBehaviour
{
    [Header("Modular Seats")]
    [Tooltip("Drag all minecart seats here.")]
    public MinecartSeat[] seats;

    [Header("Listening To")]
    [Tooltip("Departure signal event channel triggered by the lever.")]
    public VoidEventChannelSO onDepartureTriggeredEvent;

    [Header("UI Settings")]
    [Tooltip("Empty UI Bar object shown when a seat is occupied.")]
    public GameObject holdToStartUI;

    [Header("Movement (Spline)")]
    public SplineAnimate splineAnimate;
    public float cinematicRideTime = 3.5f;

    [Header("Cinematic Effects")]
    public float fovIncreaseAmount = 15f;
    public GameObject fadeUIObject;
    public float fadeTriggerTime = 3f;
    public CinemachineCamera playerCinemachineCam;

    [Header("Audio & VFX")]
    public AudioSource cartLoopAudioSource;
    public ParticleSystem[] movementParticles;

    public bool IsDeparting { get; private set; } = false;
    private LevelDataSO selectedLevelToLoad;

    private void OnEnable()
    {
        ContractBoardManager.OnContractAccepted += OnLevelSelected;
        if (onDepartureTriggeredEvent != null)
            onDepartureTriggeredEvent.OnEventRaised += TryStartDeparture;
    }

    private void OnDisable()
    {
        ContractBoardManager.OnContractAccepted -= OnLevelSelected;
        if (onDepartureTriggeredEvent != null)
            onDepartureTriggeredEvent.OnEventRaised -= TryStartDeparture;
    }

    private void Start()
    {
        if (splineAnimate) splineAnimate.Pause();
    }

    private void Update()
    {
        if (holdToStartUI == null) return;

        if (!IsDeparting && IsAnySeatOccupied())
        {
            if (!holdToStartUI.activeSelf) holdToStartUI.SetActive(true);
        }
        else
        {
            if (holdToStartUI.activeSelf) holdToStartUI.SetActive(false);
        }
    }

    private void OnLevelSelected(LevelDataSO levelData)
    {
        selectedLevelToLoad = levelData;
    }

    private bool IsAnySeatOccupied()
    {
        if (seats == null || seats.Length == 0) return false;
        foreach (var seat in seats)
        {
            if (seat != null && seat.IsOccupied) return true;
        }
        return false;
    }

    private void TryStartDeparture()
    {
        if (IsDeparting) return;

        if (selectedLevelToLoad == null)
        {
            Debug.LogWarning("[MinecartManager] Cannot depart because no contract is selected!");
            return;
        }

        if (IsAnySeatOccupied())
        {
            StartDeparture();
        }
        else
        {
            Debug.LogWarning("[MinecartManager] Departure cancelled; the minecart is empty!");
        }
    }

    private void StartDeparture()
    {
        IsDeparting = true;
        StartCoroutine(DepartureRoutine());
    }

    private IEnumerator DepartureRoutine()
    {
        if (splineAnimate != null) splineAnimate.Play();

        if (movementParticles != null)
        {
            foreach (var ps in movementParticles)
                if (ps != null) ps.Play();
        }

        float rollingStartVolume = 1f;
        if (AudioManager.Instance != null && AudioManager.Instance.data != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.minecartStart, transform.position);

            if (cartLoopAudioSource != null && AudioManager.Instance.data.minecartLoop != null)
            {
                cartLoopAudioSource.clip = AudioManager.Instance.data.minecartLoop;
                cartLoopAudioSource.loop = true;
                rollingStartVolume = cartLoopAudioSource.volume;
                cartLoopAudioSource.Play();
            }
        }

        float startFOV = 60f;
        if (playerCinemachineCam != null) startFOV = playerCinemachineCam.Lens.FieldOfView;
        float targetFOV = startFOV + fovIncreaseAmount;

        if (fadeUIObject != null) fadeUIObject.SetActive(false);

        float timer = 0f;

        while (timer < cinematicRideTime)
        {
            timer += Time.deltaTime;
            float progress = timer / cinematicRideTime;

            if (playerCinemachineCam != null)
            {
                float currentFOV = Mathf.Lerp(startFOV, targetFOV, progress);
                var lens = playerCinemachineCam.Lens;
                lens.FieldOfView = currentFOV;
                playerCinemachineCam.Lens = lens;
            }

            float fadeStartTime = cinematicRideTime - fadeTriggerTime;
            if (timer >= fadeStartTime)
            {
                if (fadeUIObject != null && !fadeUIObject.activeSelf) fadeUIObject.SetActive(true);

                if (cartLoopAudioSource != null)
                {
                    float fadeProgress = (timer - fadeStartTime) / fadeTriggerTime;
                    cartLoopAudioSource.volume = Mathf.Lerp(rollingStartVolume, 0f, fadeProgress);
                }
            }
            yield return null;
        }

        if (playerCinemachineCam != null)
        {
            var lens = playerCinemachineCam.Lens;
            lens.FieldOfView = startFOV;
            playerCinemachineCam.Lens = lens;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadDungeonScene(selectedLevelToLoad.sceneName);
        }
    }
}