/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Manages the activation and clearing of visual trails attached to weapons during combat swings.
/// </summary>
public class WeaponTrailController : MonoBehaviour
{
    [Header("Target FX")]
    [Tooltip("Drag and drop here if the effect is a Particle System")]
    [SerializeField] private ParticleSystem targetParticle;

    [Tooltip("Drag and drop here if the effect is a Trail Renderer")]
    [SerializeField] private TrailRenderer targetTrail;

    private void Awake()
    {
        if (targetParticle == null) targetParticle = GetComponent<ParticleSystem>();
        if (targetTrail == null) targetTrail = GetComponent<TrailRenderer>();
    }

    private void OnEnable()
    {
        StopTrail();
    }

    /// <summary>
    /// Initiates trail emission, properly clearing any old lingering particles or trail data beforehand.
    /// </summary>
    public void StartTrail()
    {
        if (targetParticle != null)
        {
            // IMPORTANT: Stop and Clear explicitly to remove old particles
            targetParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // IMPORTANT: Reset simulation speed to ensure it plays correctly
            var main = targetParticle.main;
            main.simulationSpeed = 1f;
            targetParticle.Play(true);
        }

        if (targetTrail != null)
        {
            targetTrail.Clear();
            targetTrail.emitting = true;
        }
    }

    /// <summary>
    /// Halts all trail and particle emissions smoothly.
    /// </summary>
    public void StopTrail()
    {
        if (targetParticle != null)
        {
            targetParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }

        if (targetTrail != null)
        {
            targetTrail.emitting = false;
        }
    }

    private void OnDisable()
    {
        StopTrail();
    }
}