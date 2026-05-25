/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Controls the player's weapon interactions, handling input buffering for smoother combat,
/// hit detection, and visual/audio feedback routing.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("The core data defining how this weapon behaves (damage, cooldown, etc.).")]
    [SerializeField] private ToolDataSO currentWeaponData;

    [Header("Settings")]
    [Tooltip("Which layers can this weapon actually hit?")]
    [SerializeField] private LayerMask hitLayers;

    [Header("Dependencies")]
    [Tooltip("Reference to the player's camera to determine attack direction.")]
    [SerializeField] private Transform playerCamera;

    [Tooltip("The procedural animator handling the weapon swings.")]
    [SerializeField] private ToolAnimator_DOTween toolAnimator;

    [Header("Event Listening")]
    [Tooltip("Channel that triggers when the player presses the attack button.")]
    [SerializeField] private VoidEventChannelSO attackChannel;

    [Tooltip("Channel to listen for player death so we can lock attacks.")]
    [SerializeField] private VoidEventChannelSO onPlayerDeath;

    [Header("Hit VFX")]
    [Tooltip("How far along the normal should the hit VFX spawn to prevent clipping?")]
    [SerializeField] private float hitVFXOffsetValue = 0.5f;

    [Header("Combat Feel (Juice)")]
    [Tooltip("How long (in seconds) should we remember an attack input? Helps combat feel responsive even if clicked slightly early.")]
    [SerializeField] private float inputBufferTime = 0.2f;

    [Tooltip("Channel that triggers Cinemachine Impulse when an enemy is hit.")]
    [SerializeField] private VoidEventChannelSO onCameraStrikeChannel;

    [Header("Debug")]
    [Tooltip("Draws the attack hitbox in the scene view for easy tweaking.")]
    [SerializeField] private bool showHitboxGizmos = true;

    private float lastAttackTime = -999f;
    private float lastInputTime = -999f;
    private bool isAttackLocked = false;

    private void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (attackChannel != null)
            attackChannel.OnEventRaised += RegisterAttackInput;

        if (onPlayerDeath != null)
            onPlayerDeath.OnEventRaised += LockAttack;
    }

    private void OnDisable()
    {
        if (attackChannel != null)
            attackChannel.OnEventRaised -= RegisterAttackInput;

        if (onPlayerDeath != null)
            onPlayerDeath.OnEventRaised -= LockAttack;
    }

    private void LockAttack()
    {
        isAttackLocked = true;
    }

    private void RegisterAttackInput()
    {
        if (isAttackLocked || currentWeaponData == null) return;

        lastInputTime = Time.time;
    }

    private void Update()
    {
        bool hasBufferedInput = (Time.time - lastInputTime) <= inputBufferTime;
        bool isCooldownFinished = (Time.time - lastAttackTime) >= currentWeaponData.cooldown;

        if (hasBufferedInput && isCooldownFinished && !isAttackLocked)
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        lastInputTime = -999f;
        lastAttackTime = Time.time;

        if (toolAnimator != null)
            toolAnimator.TriggerAttack();

        if (AudioManager.Instance != null && AudioManager.Instance.data != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.data.swordSwing, transform.position, 0.8f, 0.1f);
        }
        CheckHit();
    }

    private void CheckHit()
    {
        float range = currentWeaponData.maxDistance;
        Vector3 center = playerCamera.position + (playerCamera.forward * (range * 0.5f));
        Vector3 halfExtents = new Vector3(currentWeaponData.toolRadius, currentWeaponData.toolRadius, range * 0.5f);

        Collider[] hits = Physics.OverlapBox(center, halfExtents, playerCamera.rotation, hitLayers);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out var damageable))
            {
                if (damageable.IsDead()) continue;

                Vector3 hitPoint = hit.ClosestPoint(playerCamera.position);
                Vector3 knockbackDir = (hit.transform.position - playerCamera.position).normalized;
                knockbackDir.y = 0;

                Vector3 hitNormal = (playerCamera.position - hitPoint).normalized;
                Vector3 vfxSpawnPoint = hitPoint + (hitNormal * hitVFXOffsetValue);

                if (CombatManager.Instance != null)
                {
                    CombatManager.Instance.ProcessPlayerHit(
                        damageable,
                        currentWeaponData.damage,
                        knockbackDir,
                        hitPoint,
                        currentWeaponData.stunDuration,
                        currentWeaponData.knockbackDuration
                    );
                }

                if (FeedbackManager.Instance != null)
                {
                    FeedbackManager.Instance.PlayHitVFX(vfxSpawnPoint, hitNormal, true);
                    FeedbackManager.Instance.TriggerHitStop(0.08f);
                }

                if (onCameraStrikeChannel != null)
                {
                    onCameraStrikeChannel.Raise();
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showHitboxGizmos || currentWeaponData == null || playerCamera == null) return;
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        float range = currentWeaponData.maxDistance;
        Vector3 center = playerCamera.position + (playerCamera.forward * (range * 0.5f));
        Vector3 size = new Vector3(currentWeaponData.toolRadius * 2, currentWeaponData.toolRadius * 2, range);
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(playerCamera.position, playerCamera.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawCube(Vector3.forward * (range * 0.5f), size);
    }
}