/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using DG.Tweening;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Splines;

/// <summary>
/// Handles cinematic spline sequences evaluating end of level functionality, transporting 
/// player entities alongside calculated progression states into loading screens appropriately.
/// </summary>
public class DungeonExtractionManager : MonoBehaviour
{
    [Header("Mode Settings")]
    public bool isDepartingFromHub = false;
    public string targetSceneName = "HubScene";

    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onExtractionFadeOutEvent;
    [SerializeField] private VoidEventChannelSO onShowLoadingEvent;

    [Header("Modular Seats")]
    [Tooltip("Drag and target all linked seats present within the cart here.")]
    public MinecartSeat[] seats;

    [Header("Sequence Settings")]
    public SplineAnimate splineAnimate;
    public SplineContainer extractionSpline;

    [Header("Timing Settings (Seconds)")]
    public float timeBeforeFade = 2.5f;
    public float fadeWaitDuration = 1.5f;

    [Header("Visual & VFX Settings")]
    public ParticleSystem[] extractionVFX;
    public float targetFOV = 90f;
    public float fovTransitionSpeed = 2f;

    [Header("Listening To")]
    [Tooltip("Listens to extraction triggers derived dynamically from lever interactive instances.")]
    public VoidEventChannelSO onExtractionTriggeredEvent;

    [Header("Audio")]
    public AudioSource cartLoopAudioSource;

    [HideInInspector] public bool isExtracting = false;

    private void OnEnable()
    {
        if (onExtractionTriggeredEvent != null)
            onExtractionTriggeredEvent.OnEventRaised += StartExtractionSequence;
    }

    private void OnDisable()
    {
        if (onExtractionTriggeredEvent != null)
            onExtractionTriggeredEvent.OnEventRaised -= StartExtractionSequence;
    }

    /// <summary>
    /// Primary entry point parsing seat checks moving player camera arrays directly towards exit.
    /// </summary>
    public void StartExtractionSequence()
    {
        if (!IsAnySeatOccupied()) return;

        StartCoroutine(ExtractionRoutine());
        StartCoroutine(FOVTransitionRoutine());
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

    private IEnumerator ExtractionRoutine()
    {
        isExtracting = true;

        if (cartLoopAudioSource != null && AudioManager.Instance != null && AudioManager.Instance.data != null)
        {
            cartLoopAudioSource.clip = AudioManager.Instance.data.minecartLoop;
            cartLoopAudioSource.loop = true;
            cartLoopAudioSource.Play();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.minecartStart, transform.position);
        }

        if (extractionVFX != null)
        {
            foreach (ParticleSystem vfx in extractionVFX)
            {
                if (vfx != null) { vfx.gameObject.SetActive(true); vfx.Play(); }
            }
        }

        if (splineAnimate != null && extractionSpline != null)
        {
            splineAnimate.Container = extractionSpline;
            splineAnimate.Easing = SplineAnimate.EasingMode.EaseIn;
            splineAnimate.ObjectForwardAxis = SplineComponent.AlignAxis.NegativeZAxis;
            splineAnimate.Restart(true);
            splineAnimate.Play();
        }

        yield return new WaitForSeconds(timeBeforeFade);

        if (onExtractionFadeOutEvent != null) onExtractionFadeOutEvent.Raise();

        yield return new WaitForSeconds(fadeWaitDuration);

        if (onShowLoadingEvent != null) onShowLoadingEvent.Raise();
        yield return null;

        if (isDepartingFromHub)
        {
            if (LevelManager.Instance != null)
                LevelManager.Instance.LoadDungeonScene(targetSceneName);
            else
                SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.SaveLastRunDataToPrefs();
                LevelManager.Instance.SecureLoot();
            }
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private IEnumerator FOVTransitionRoutine()
    {
        if (Camera.main == null) yield break;
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        if (brain == null || brain.ActiveVirtualCamera == null) yield break;
        CinemachineCamera vcam = brain.ActiveVirtualCamera as CinemachineCamera;
        if (vcam == null) yield break;

        float currentFOV = vcam.Lens.FieldOfView;
        while (isExtracting)
        {
            currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovTransitionSpeed);
            vcam.Lens.FieldOfView = currentFOV;
            if (Mathf.Abs(currentFOV - targetFOV) < 0.1f) { vcam.Lens.FieldOfView = targetFOV; break; }
            yield return null;
        }
    }
}