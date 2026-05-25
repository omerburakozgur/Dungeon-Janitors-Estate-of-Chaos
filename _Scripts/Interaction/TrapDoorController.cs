/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the trap door's visual and audio effects upon accepting a mission contract.
/// </summary>
public class TrapDoorController : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Animator component controlling the trap door.")]
    public Animator trapDoorAnimator;

    [Tooltip("Trigger parameter name used to transition to the open state.")]
    public string openTriggerName = "Open";

    [Header("Effects & Feedback")]
    public GameObject warningLight;

    [Tooltip("Particle system triggered when the trap door opens.")]
    public ParticleSystem dustVFX;

    private bool isOpen = false;

    private void OnEnable()
    {
        ContractBoardManager.OnContractAccepted += OpenTrapDoor;
    }

    private void OnDisable()
    {
        ContractBoardManager.OnContractAccepted -= OpenTrapDoor;
    }

    private void Start()
    {
        if (warningLight) warningLight.SetActive(false);
        if (dustVFX) dustVFX.Stop();
    }

    /// <summary>
    /// Initiates the trap door opening sequence when a contract is accepted.
    /// </summary>
    /// <param name="selectedLevel">The level data for the accepted contract.</param>
    private void OpenTrapDoor(LevelDataSO selectedLevel)
    {
        if (isOpen) return;

        isOpen = true;
        StartCoroutine(OpenDoorRoutine());
    }

    private IEnumerator OpenDoorRoutine()
    {
        if (warningLight) warningLight.SetActive(true);

        if (AudioManager.Instance != null && AudioManager.Instance.data.trapDoorUnlock != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.trapDoorUnlock, transform.position);
        }

        yield return new WaitForSeconds(0.5f);

        if (AudioManager.Instance != null && AudioManager.Instance.data.trapDoorCreak != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.trapDoorCreak, transform.position);
        }

        if (dustVFX != null)
        {
            dustVFX.Play();
        }

        if (trapDoorAnimator != null)
        {
            trapDoorAnimator.SetTrigger(openTriggerName);
        }
    }
}