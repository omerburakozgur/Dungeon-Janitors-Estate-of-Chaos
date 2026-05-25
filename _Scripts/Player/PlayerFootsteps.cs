/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Detects floor surface via Raycast and plays appropriate audio clips via AudioManager.
/// </summary>
public class PlayerFootsteps : MonoBehaviour
{
    [Header("Settings")]
    public float raycastDistance = 1.5f;
    public LayerMask floorLayerMask;
    public Transform raycastOrigin;

    public bool canPlayAudio = true;

    private float lastFootstepTime = 0f;
    private const float StepCooldown = 0.15f;

    /// <summary>
    /// Triggers a footstep sound effect based on the floor material tag.
    /// </summary>
    public void PlayDynamicFootstep()
    {
        if (!canPlayAudio) return;

        if (Time.time - lastFootstepTime < StepCooldown) return;

        lastFootstepTime = Time.time;

        if (AudioManager.Instance == null || AudioManager.Instance.data == null) return;

        AudioClip[] selectedClips = AudioManager.Instance.data.footSteps;
        Vector3 origin = raycastOrigin != null ? raycastOrigin.position : transform.position + (Vector3.up * 0.5f);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, raycastDistance, floorLayerMask))
        {
            string floorTag = hit.collider.tag;
            switch (floorTag)
            {
                case "Wood": selectedClips = AudioManager.Instance.data.woodFootsteps; break;
                case "Stone": selectedClips = AudioManager.Instance.data.stoneFootsteps; break;
                case "Carpet": selectedClips = AudioManager.Instance.data.carpetFootsteps; break;
                case "Metal": selectedClips = AudioManager.Instance.data.metalFootsteps; break;
            }
        }

        if (selectedClips != null && selectedClips.Length > 0)
        {
            AudioClip clip = selectedClips[Random.Range(0, selectedClips.Length)];
            AudioManager.Instance.PlaySFX(clip, transform.position, 0.5f, 0.1f);
        }
    }
}