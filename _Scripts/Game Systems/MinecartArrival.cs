/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.Splines;
using System.Collections;

/// <summary>
/// Dictates entry sequencing for dynamic spline animations safely seating and unloading player prefabs appropriately.
/// </summary>
public class MinecartArrival : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private VoidEventChannelSO onArrivalFadeInEvent;

    [Header("Modular Seats")]
    [Tooltip("Vagondaki tum koltuklari (Seat) buraya surukle")]
    public MinecartSeat[] seats;

    [Header("Arrival Settings")]
    public SplineAnimate splineAnimate;
    public ParticleSystem[] arrivalVFX;
    public AudioSource cartLoopAudioSource;
    public GameObject[] interactablesToEnable;

    /// <summary>
    /// Initializes arrival transition parsing linked seat capacities and moving splines gracefully.
    /// </summary>
    public void StartArrivalSequence(GameObject player)
    {
        StartCoroutine(ArrivalRoutine(player));
    }

    private IEnumerator ArrivalRoutine(GameObject player)
    {
        if (seats != null && seats.Length > 0 && seats[0] != null && player != null)
        {
            seats[0].BoardPlayer(player);
            if (onArrivalFadeInEvent != null) onArrivalFadeInEvent.Raise();

            UIManager.Instance.ToggleHoldProgressBarUI(false);
        }

        if (arrivalVFX != null)
        {
            foreach (ParticleSystem vfx in arrivalVFX)
            {
                if (vfx != null)
                {
                    vfx.gameObject.SetActive(true);
                    vfx.Play();
                }
            }
        }

        if (cartLoopAudioSource != null && AudioManager.Instance != null && AudioManager.Instance.data != null)
        {
            cartLoopAudioSource.clip = AudioManager.Instance.data.minecartLoop;
            cartLoopAudioSource.loop = true;
            cartLoopAudioSource.Play();
        }

        float rideTime = 3f;
        if (splineAnimate != null)
        {
            splineAnimate.Play();
            rideTime = splineAnimate.Duration;
        }

        yield return new WaitForSeconds(rideTime);

        if (arrivalVFX != null)
        {
            foreach (ParticleSystem vfx in arrivalVFX)
            {
                if (vfx != null) vfx.Stop();
            }
        }

        if (cartLoopAudioSource != null) cartLoopAudioSource.Stop();

        if (AudioManager.Instance != null && AudioManager.Instance.data != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.minecartStart, transform.position);
        }

        if (seats != null)
        {
            foreach (var seat in seats)
            {
                if (seat != null && seat.IsOccupied)
                {
                    seat.UnboardPlayer(true);
                }
            }
        }

        if (interactablesToEnable != null)
        {
            foreach (GameObject obj in interactablesToEnable)
            {
                if (obj != null) obj.SetActive(true);
            }
        }
    }
}