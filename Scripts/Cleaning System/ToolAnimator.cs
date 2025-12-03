// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;
using DG.Tweening; // DOTween tweening library used for procedural animations

/// <summary>
/// Drives procedural tool animations for both continuous cleaning motions (mop) and
/// single-shot combat motions (sword). Uses DOTween to sequence and loop transform tweens.
/// </summary>
public class ToolAnimator_DOTween : MonoBehaviour
{
 /// <summary>Animation mode used by this tool instance.</summary>
 public enum MotionType { Cleaning_Continuous, Combat_OneShot } // Motion mode selection

 [Header("General Setup")] // Inspector grouping for general settings
 public MotionType motionType; // Selected behaviour for this tool instance

 [Header("Idle Settings")] // Inspector grouping for idle parameters
 [SerializeField] private float bobAmount =0.05f; // Vertical idle bob amplitude (meters)
 [SerializeField] private float bobDuration =2f; // Duration of a single idle bob cycle (seconds)
 [SerializeField] private float swayAmount =1.5f; // Strength of mouse-based sway rotation (degrees)

 [Header("Cleaning Settings (Mop)")] // Inspector grouping for cleaning-specific parameters
 [SerializeField] private float scrubDistance =0.3f; // Forward travel distance for scrubbing motion (meters)
 [SerializeField] private float scrubDuration =0.15f; // Duration of one scrub half-cycle (seconds)
 [SerializeField] private Vector3 scrubRotation = new Vector3(10f,0,0); // Rotation applied during scrubbing

 [Header("Combat Settings (Sword)")] // Inspector grouping for combat-specific parameters
 [SerializeField] private Vector3 attackPosOffset = new Vector3(0, -0.2f,0.7f); // Local position offset for attack
 [SerializeField] private Vector3 attackRotOffset = new Vector3(60f, -30f,0); // Local rotation offset for attack
 [SerializeField] private float attackDuration =0.25f; // Total attack duration (seconds)
 [SerializeField] private Ease attackEase = Ease.OutBack; // Tween easing used for attack feel

 private Vector3 initialPos; // Cached initial local position
 private Quaternion initialRot; // Cached initial local rotation

 // Tween references used to control and safely stop animations
 private Tween idleTween; // Reference to the looping idle tween
 private Tween cleanPosTween; // Reference to scrub position tween
 private Tween cleanRotTween; // Reference to scrub rotation tween
 private bool isAttacking = false; // Runtime flag indicating an attack sequence is active
 private bool isCleaning = false; // Runtime flag indicating cleaning loop is active

 /// <summary>
 /// Cache initial transform values and start the idle animation.
 /// </summary>
 private void Start()
 {
 initialPos = transform.localPosition; // Cache starting local position
 initialRot = transform.localRotation; // Cache starting local rotation

 StartIdle(); // Begin continuous idle animation
 }

 /// <summary>
 /// Update is used to handle mouse-driven sway when not performing a higher-priority animation.
 /// </summary>
 private void Update()
 {
 HandleSway(); // Apply rotational sway based on current mouse input
 }

 // --- Public API: called by external systems / event channels ---

 /// <summary>
 /// Trigger a single attack animation when the tool is configured for combat.
 /// </summary>
 public void TriggerAttack()
 {
 if (motionType == MotionType.Combat_OneShot)
 {
 Attack(); // Execute the attack sequence
 }
 }

 /// <summary>
 /// Enable or disable continuous cleaning motion (mop). External systems should call this
 /// to reflect the tool being actively used or put away.
 /// </summary>
 /// <param name="state">True to start cleaning loop, false to stop it.</param>
 public void SetCleaningState(bool state)
 {
 if (motionType != MotionType.Cleaning_Continuous) return; // Ignore if not a cleaning tool

 if (state && !isCleaning)
 {
 isCleaning = true; // Mark active
 StartScrubbing(); // Start scrub loop
 }
 else if (!state && isCleaning)
 {
 isCleaning = false; // Mark inactive
 StopScrubbing(); // Stop scrub loop and restore pose
 }
 }

 // --- Cleaning (looping scrubbing) implementation ---

 /// <summary>
 /// Begin the continuous scrubbing motion by pausing idle and creating looping DOTween tweens.
 /// </summary>
 private void StartScrubbing()
 {
 idleTween.Pause(); // Pause idle animation while scrubbing

 // Forward/backward position loop using a Yoyo loop to create a scrub movement
 cleanPosTween = transform.DOLocalMove(initialPos + (Vector3.forward * scrubDistance), scrubDuration)
 .SetLoops(-1, LoopType.Yoyo) // Infinite yoyo loop
 .SetEase(Ease.InOutSine); // Smooth easing for natural motion

 // Oscillating rotation during scrubbing to add visual variety
 cleanRotTween = transform.DOLocalRotate(initialRot.eulerAngles + scrubRotation, scrubDuration)
 .SetLoops(-1, LoopType.Yoyo)
 .SetEase(Ease.InOutSine);
 }

 /// <summary>
 /// Stop the scrubbing tweens, return the tool to its initial pose and resume idle animation.
 /// </summary>
 private void StopScrubbing()
 {
 // Terminate active scrub tweens
 cleanPosTween.Kill();
 cleanRotTween.Kill();

 // Smoothly restore original local transform
 transform.DOLocalMove(initialPos,0.2f);
 transform.DOLocalRotate(initialRot.eulerAngles,0.2f);

 idleTween.Play(); // Resume idle breathing animation
 }

 // --- Combat attack sequence implementation ---

 /// <summary>
 /// Execute a single attack sequence using a DOTween Sequence that combines position and rotation changes.
 /// </summary>
 private void Attack()
 {
 isAttacking = true; // Mark attack active
 idleTween.Pause(); // Pause idle while attacking

 Sequence attackSeq = DOTween.Sequence(); // Create sequence container

 //1) Quick strike towards the attack offset using configured easing
 attackSeq.Append(transform.DOLocalMove(initialPos + attackPosOffset, attackDuration *0.4f).SetEase(attackEase));
 attackSeq.Join(transform.DOLocalRotate(initialRot.eulerAngles + attackRotOffset, attackDuration *0.4f).SetEase(attackEase));

 //2) Slower return to the original pose
 attackSeq.Append(transform.DOLocalMove(initialPos, attackDuration *0.6f).SetEase(Ease.OutQuad));
 attackSeq.Join(transform.DOLocalRotate(initialRot.eulerAngles, attackDuration *0.6f).SetEase(Ease.OutQuad));

 // When the sequence completes, clear the attack flag and resume idle
 attackSeq.OnComplete(() =>
 {              
 isAttacking = false; // Attack finished
 idleTween.Play(); // Resume idle animation
 });
 }

 // --- Idle and mouse sway handling ---

 /// <summary>
 /// Start a looping idle "breathing" animation that slightly moves the tool up and down.
 /// </summary>
 private void StartIdle()
 {
 idleTween = transform.DOLocalMoveY(initialPos.y + bobAmount, bobDuration)
 .SetLoops(-1, LoopType.Yoyo)
 .SetEase(Ease.InOutSine); // Continuous slow easing for natural feel
 }

 /// <summary>
 /// Apply a subtle rotational sway driven by the player's mouse input when not performing other actions.
 /// Uses Slerp for stable interpolation and responsive feel.
 /// </summary>
 private void HandleSway()
 {
 if (isAttacking) return; // Do not apply sway while performing an attack

 float mouseX = Input.GetAxis("Mouse X") * swayAmount; // Horizontal mouse delta
 float mouseY = Input.GetAxis("Mouse Y") * swayAmount; // Vertical mouse delta

 Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right); // Pitch contribution
 Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up); // Yaw contribution

 Quaternion targetRot = initialRot * rotationX * rotationY; // Compose target rotation

 // Smoothly interpolate towards the target rotation for responsive yet stable sway
 transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime *8f);
 }

 /// <summary>
 /// Ensure all DOTween tweens are killed when this object is destroyed to prevent stray callbacks.
 /// </summary>
 private void OnDestroy()
 {
 transform.DOKill(); // Kill all tweens targeting this transform
 }

 /// <summary>
 /// OnDisable is used to stop and kill tweens when the component is disabled to avoid errors.
 /// </summary>
 private void OnDisable()
 {
 // Safely terminate any running tweens
 idleTween.Kill();
 cleanPosTween.Kill();
 cleanRotTween.Kill();
 transform.DOKill(); // Extra safeguard to remove any remaining tweens
 }
}