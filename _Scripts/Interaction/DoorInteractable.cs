/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Categorization representing the physical structure of the swinging door component.
/// </summary>
public enum DoorType
{
    Wood,
    Metal,
    HeavyGate
}

/// <summary>
/// Interaction wrapper tied directly to Unity Animations allowing 
/// doors to swing open based on player proximity constraints.
/// </summary>
public class DoorInteractable : BaseInteractable
{
    [Header("Door Settings")]
    [Tooltip("Animator component located on the Pivot object to handle rotation.")]
    [SerializeField] private Animator doorAnimator;

    [Tooltip("The exact name of the boolean parameter inside the Animator.")]
    [SerializeField] private string animatorBoolName = "IsOpen";

    [Tooltip("Material type of the door, determines which SFX to play.")]
    [SerializeField] private DoorType doorType = DoorType.Wood;

    [Header("Door State")]
    public bool isOpen = false;

    [Header("Spam Protection")]
    [Tooltip("How long (in seconds) the door ignores new interactions while animating.")]
    [SerializeField] private float interactionCooldown = 1.2f;

    private float nextAllowedInteractTime = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (doorAnimator != null)
        {
            doorAnimator.SetBool(animatorBoolName, isOpen);
        }
    }

    /// <summary>
    /// Overrides prompt text checking door current state flags tracking properly.
    /// </summary>
    /// <returns>Formatted interaction string contextually updating the door status.</returns>
    public override string GetInteractionPrompt()
    {
        return isOpen ? "[E] Close" : "[E] Open";
    }

    /// <summary>
    /// Event mapped triggering the main Interaction sequence executing animation locks.
    /// Incorporates positional math detecting player standpoint for correct realistic directional swings.
    /// </summary>
    public override void RequestInteract()
    {
        if (Time.time < nextAllowedInteractTime) return;

        nextAllowedInteractTime = Time.time + interactionCooldown;

        isOpen = !isOpen;

        if (doorAnimator != null)
        {
            if (isOpen)
            {
                Transform playerTransform = Player.Instance.transform;
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

                float openDirection = (dotProduct < 0) ? 1f : -1f;
                doorAnimator.SetFloat("OpenDirection", openDirection);
            }

            doorAnimator.SetBool(animatorBoolName, isOpen);
        }

        PlayDoorSound(isOpen);
    }

    /// <summary>
    /// Resolves hold-key interactions. Left functionally empty for doors.
    /// </summary>
    public override void RequestHoldInteract()
    {
    }

    /// <summary>
    /// Ends hold-key interaction. Left functionally empty for doors.
    /// </summary>
    public override void RequestReleaseInteract()
    {
    }

    private void PlayDoorSound(bool opening)
    {
        if (AudioManager.Instance == null || AudioManager.Instance.data == null) return;

        AudioClip clipToPlay = null;

        switch (doorType)
        {
            case DoorType.Wood:
                clipToPlay = opening ? AudioManager.Instance.data.woodDoorOpen : AudioManager.Instance.data.woodDoorClose;
                break;
            case DoorType.Metal:
                clipToPlay = opening ? AudioManager.Instance.data.metalDoorOpen : AudioManager.Instance.data.metalDoorClose;
                break;
            case DoorType.HeavyGate:
                clipToPlay = opening ? AudioManager.Instance.data.heavyGateOpen : AudioManager.Instance.data.heavyGateClose;
                break;
        }

        if (clipToPlay != null)
        {
            AudioManager.Instance.PlaySFX(clipToPlay, transform.position);
        }
    }
}