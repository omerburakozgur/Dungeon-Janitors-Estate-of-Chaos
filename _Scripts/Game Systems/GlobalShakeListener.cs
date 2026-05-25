/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Attaches impulse nodes mapping camera shake frequencies responding immediately upon events triggered globally.
/// Centralizes the Cinemachine shake mechanism into an easily manipulated manager script.
/// </summary>
[RequireComponent(typeof(CinemachineImpulseSource))]
public class GlobalShakeListener : MonoBehaviour
{
    [Header("Impact Events")]
    [SerializeField] private VoidEventChannelSO onCameraShakeChannel;
    [SerializeField] private VoidEventChannelSO onCameraStrikeChannel;

    [Header("Movement Events")]
    [Tooltip("Channel triggered when the player takes a step.")]
    [SerializeField] private VoidEventChannelSO onFootstepShakeChannel;

    [Header("Shake Settings")]
    [SerializeField] private float defaultForce = 1f;
    [SerializeField] private float strikeMultiplier = 0.6f;

    [Tooltip("Multiplier for footstep camera shake. Should be very subtle.")]
    [SerializeField] private float footstepMultiplier = 0.05f;

    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void OnEnable()
    {
        if (onCameraShakeChannel != null) onCameraShakeChannel.OnEventRaised += TriggerDamageShake;
        if (onCameraStrikeChannel != null) onCameraStrikeChannel.OnEventRaised += TriggerStrikeShake;
        if (onFootstepShakeChannel != null) onFootstepShakeChannel.OnEventRaised += TriggerFootstepShake;
    }

    private void OnDisable()
    {
        if (onCameraShakeChannel != null) onCameraShakeChannel.OnEventRaised -= TriggerDamageShake;
        if (onCameraStrikeChannel != null) onCameraStrikeChannel.OnEventRaised -= TriggerStrikeShake;
        if (onFootstepShakeChannel != null) onFootstepShakeChannel.OnEventRaised -= TriggerFootstepShake;
    }

    private void TriggerDamageShake() { ApplyShake(defaultForce); }
    private void TriggerStrikeShake() { ApplyShake(defaultForce * strikeMultiplier); }

    private void TriggerFootstepShake()
    {
        Vector3 stepDirection = new Vector3(Random.Range(-0.2f, 0.2f), -1f, 0f).normalized;
        impulseSource.GenerateImpulse(stepDirection * (defaultForce * footstepMultiplier));
    }

    private void ApplyShake(float force)
    {
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        randomDirection.y *= 0.5f;
        impulseSource.GenerateImpulse(randomDirection * force);
    }
}