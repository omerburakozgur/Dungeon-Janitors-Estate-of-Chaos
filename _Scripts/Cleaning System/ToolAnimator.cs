/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Drives procedural tool animations. Uses DOTween for discrete actions (attacks, scrubbing) 
/// on the Action Pivot, and real-time procedural math in Update for sway and bobbing on the Sway Pivot.
/// </summary>
public class ToolAnimator_DOTween : MonoBehaviour
{
    public enum MotionType
    {
        Cleaning_Continuous,
        Combat_OneShot
    }

    public enum ContinuousStyle
    {
        Mop_ForwardBack,
        Vacuum_Vibrate,
        Scrubber_Aggressive
    }

    [Header("General Setup")]
    public MotionType motionType;
    public ContinuousStyle continuousStyle;

    [Header("Hierarchy Setup (Separated Pivots)")]
    [Tooltip("Topmost object that will only handle left/right swaying (e.g., Tool_Sway_Parent)")]
    [SerializeField] private Transform swayPivot;

    [Tooltip("Object where only cleaning and attack animations will be played (e.g., Tool_1_VisualOffset)")]
    [SerializeField] private Transform actionPivot;

    [Header("Procedural Sway & Bob (Game Feel)")]
    [SerializeField] private float swayRotationAmount = 2f;
    [SerializeField] private float swayPositionAmount = 0.02f;
    [SerializeField] private float swaySmoothness = 8f;
    [SerializeField] private float strafeTiltAmount = 2f;

    [Header("Movement Bobbing")]
    [SerializeField] private float bobSpeed = 12f;
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private CharacterController playerController;

    [Header("Continuous Cleaning Settings")]
    [SerializeField] private Vector3 scrubOffset = new Vector3(0, 0, 0.3f);
    [SerializeField] private float scrubVibration = 0.3f;
    [SerializeField] private float scrubDuration = 0.15f;
    [SerializeField] private Vector3 scrubRotation = new Vector3(10f, 0, 0);

    [Header("Combat Settings (Sword)")]
    [SerializeField] private Vector3 attackPosOffset = new Vector3(0, -0.2f, 0.7f);
    [SerializeField] private Vector3 attackRotOffset = new Vector3(60f, -30f, 0);
    [SerializeField] private float attackDuration = 0.25f;
    [SerializeField] private Ease attackEase = Ease.OutBack;

    [Header("VFX Connections")]
    [SerializeField] private WeaponTrailController weaponTrail;

    private Vector3 swayInitialPos;
    private Quaternion swayInitialRot;
    private Vector3 actionInitialPos;
    private Quaternion actionInitialRot;

    private Tween cleanPosTween;
    private Tween cleanRotTween;
    private bool isAttacking = false;
    private bool isCleaning = false;
    private float bobTimer = 0f;

    private void Awake()
    {
        if (playerController == null)
            playerController = GetComponentInParent<CharacterController>();

        if (swayPivot != null)
        {
            swayInitialPos = swayPivot.localPosition;
            swayInitialRot = swayPivot.localRotation;
        }
        else
        {
            Debug.LogError($"[ToolAnimator] Sway Pivot is not assigned!");
        }

        if (actionPivot != null)
        {
            actionInitialPos = actionPivot.localPosition;
            actionInitialRot = actionPivot.localRotation;
        }
        else
        {
            Debug.LogError($"[ToolAnimator] Action Pivot is not assigned!");
        }
    }

    private void OnEnable()
    {
        if (swayPivot != null) swayPivot.DOKill();
        if (actionPivot != null) actionPivot.DOKill();

        if (cleanPosTween != null) cleanPosTween.Kill();
        if (cleanRotTween != null) cleanRotTween.Kill();

        isAttacking = false;
        isCleaning = false;
        bobTimer = 0f;

        if (swayPivot != null)
        {
            swayPivot.localPosition = swayInitialPos;
            swayPivot.localRotation = swayInitialRot;
        }

        if (actionPivot != null)
        {
            actionPivot.localPosition = actionInitialPos;
            actionPivot.localRotation = actionInitialRot;
        }
    }

    private void Update()
    {
        HandleProceduralMotion();
    }

    private void HandleProceduralMotion()
    {
        if (swayPivot == null) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        float currentSpeed = (playerController != null) ? playerController.velocity.magnitude : new Vector2(moveX, moveY).magnitude;

        Vector3 targetPos = swayInitialPos;
        targetPos.x += Mathf.Clamp(-mouseX * swayPositionAmount, -0.1f, 0.1f);
        targetPos.y += Mathf.Clamp(-mouseY * swayPositionAmount, -0.1f, 0.1f);

        if (currentSpeed > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            targetPos.y += Mathf.Sin(bobTimer) * bobAmount;
            targetPos.x += Mathf.Cos(bobTimer * 0.5f) * (bobAmount * 0.5f);
        }
        else
        {
            bobTimer += Time.deltaTime * (bobSpeed * 0.25f);
            targetPos.y += Mathf.Sin(bobTimer) * (bobAmount * 0.2f);
        }

        swayPivot.localPosition = Vector3.Lerp(swayPivot.localPosition, targetPos, Time.deltaTime * swaySmoothness);

        Quaternion swayRot = Quaternion.AngleAxis(-mouseY * swayRotationAmount, Vector3.right) * Quaternion.AngleAxis(mouseX * swayRotationAmount, Vector3.up);
        Quaternion tiltRot = Quaternion.AngleAxis(-moveX * strafeTiltAmount, Vector3.forward);
        Quaternion finalRot = swayInitialRot * swayRot * tiltRot;

        swayPivot.localRotation = Quaternion.Slerp(swayPivot.localRotation, finalRot, Time.deltaTime * swaySmoothness);
    }

    /// <summary>
    /// Triggers a discrete one-shot attack animation if the tool is configured for combat.
    /// </summary>
    public void TriggerAttack()
    {
        if (motionType == MotionType.Combat_OneShot && !isAttacking)
        {
            Attack();
        }
    }

    /// <summary>
    /// Toggles the continuous scrubbing animation on or off.
    /// </summary>
    /// <param name="state">True to start scrubbing, false to stop.</param>
    public void SetCleaningState(bool state)
    {
        if (motionType != MotionType.Cleaning_Continuous) return;

        if (state && !isCleaning)
        {
            isCleaning = true;
            StartScrubbing();
        }
        else if (!state && isCleaning)
        {
            isCleaning = false;
            StopScrubbing();
        }
    }

    private void StartScrubbing()
    {
        if (actionPivot == null) return;

        actionPivot.DOKill();
        actionPivot.DOLocalMove(actionInitialPos, 0.1f).OnComplete(() =>
        {
            switch (continuousStyle)
            {
                case ContinuousStyle.Mop_ForwardBack:
                    cleanPosTween = actionPivot.DOLocalMove(actionInitialPos + scrubOffset, scrubDuration)
                        .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                    cleanRotTween = actionPivot.DOLocalRotate(actionInitialRot.eulerAngles + scrubRotation, scrubDuration)
                        .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
                    break;

                case ContinuousStyle.Vacuum_Vibrate:
                    cleanPosTween = actionPivot.DOShakePosition(scrubDuration, scrubVibration, 20, 90, false, true)
                        .SetLoops(-1, LoopType.Restart);
                    cleanRotTween = actionPivot.DOShakeRotation(scrubDuration, scrubRotation, 20, 90, true)
                        .SetLoops(-1, LoopType.Restart);
                    break;

                case ContinuousStyle.Scrubber_Aggressive:
                    cleanPosTween = actionPivot.DOLocalMove(actionInitialPos + scrubOffset, scrubDuration)
                        .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Flash);
                    cleanRotTween = actionPivot.DOLocalRotate(actionInitialRot.eulerAngles + scrubRotation, scrubDuration)
                        .SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Flash);
                    break;
            }
        });
    }

    private void StopScrubbing()
    {
        if (actionPivot == null) return;

        actionPivot.DOKill();
        if (cleanPosTween != null) cleanPosTween.Kill();
        if (cleanRotTween != null) cleanRotTween.Kill();

        actionPivot.DOLocalMove(actionInitialPos, 0.2f).SetEase(Ease.OutQuad);
        actionPivot.DOLocalRotateQuaternion(actionInitialRot, 0.2f).SetEase(Ease.OutQuad);
    }

    private void Attack()
    {
        if (actionPivot == null) return;

        isAttacking = true;
        if (weaponTrail != null) weaponTrail.StartTrail();

        actionPivot.DOKill();

        Sequence attackSeq = DOTween.Sequence();

        attackSeq.Append(actionPivot.DOLocalMove(actionInitialPos + attackPosOffset, attackDuration * 0.4f).SetEase(attackEase));
        attackSeq.Join(actionPivot.DOLocalRotate(actionInitialRot.eulerAngles + attackRotOffset, attackDuration * 0.4f).SetEase(attackEase));

        attackSeq.Append(actionPivot.DOLocalMove(actionInitialPos, attackDuration * 0.6f).SetEase(Ease.OutQuad));
        attackSeq.Join(actionPivot.DOLocalRotateQuaternion(actionInitialRot, attackDuration * 0.6f).SetEase(Ease.OutQuad));

        attackSeq.OnComplete(() =>
        {
            isAttacking = false;
            if (weaponTrail != null) weaponTrail.StopTrail();
        });

        attackSeq.OnKill(() =>
        {
            isAttacking = false;
            if (weaponTrail != null) weaponTrail.StopTrail();
        });
    }

    private void OnDestroy()
    {
        if (swayPivot != null) swayPivot.DOKill();
        if (actionPivot != null) actionPivot.DOKill();
    }

    private void OnDisable()
    {
        if (swayPivot != null) swayPivot.DOKill();
        if (actionPivot != null) actionPivot.DOKill();

        if (cleanPosTween != null) cleanPosTween.Kill();
        if (cleanRotTween != null) cleanRotTween.Kill();

        if (weaponTrail != null) weaponTrail.StopTrail();

        isAttacking = false;
        isCleaning = false;
    }
}