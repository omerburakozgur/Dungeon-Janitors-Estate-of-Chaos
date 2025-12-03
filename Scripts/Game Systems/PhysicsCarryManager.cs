// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using System.Collections;

/// <summary>
/// Physics-based object carrying manager.
/// Features: authoritative pickup/drop, scroll-controlled hold distance, charged throws,
/// wall clipping prevention, and collision handling fixes.
/// </summary>
public class PhysicsCarryManager : SingletonManager<PhysicsCarryManager>
{
 [Header("Settings - Holding")]
 [SerializeField] private float holdDistance =2.5f; // Default hold distance
 [SerializeField] private float minHoldDistance =1.5f; // Minimum allowed hold distance
 [SerializeField] private float maxHoldDistance =4.5f; // Maximum allowed hold distance
 [SerializeField] private float scrollSensitivity =0.5f; // Scroll wheel sensitivity
 [SerializeField] private float moveSmoothing =15f; // Lerp smoothing for object movement

 [Header("Settings - Throwing")]
 [SerializeField] private float minThrowForce =5f; // Minimum throw impulse
 [SerializeField] private float maxThrowForce =25f; // Maximum throw impulse
 [SerializeField] private float maxChargeTime =1.5f; // Time to reach max throw force

 [Header("Collision Settings (NEW)")]
 // Player collider reference used to temporarily disable collisions with carried item
 [SerializeField] private Collider playerCollider;
 // Layers considered obstacles for clipping prevention
 [SerializeField] private LayerMask wallLayers;

 [Header("References")]
 [SerializeField] private Transform playerCameraTransform; // Reference to player camera transform
 [SerializeField] private Transform holdPoint; // Local hold point used for placement

 [Header("Events")]
 [SerializeField] private FloatEventChannelSO onThrowChargeChanged; // UI event for throw charge

 // Runtime state
 private Rigidbody currentCarriedObject;
 private CarryableItem currentItemScript;
 private bool isCarrying = false; // Whether an object is currently carried

 // Charge state
 private bool isChargingThrow = false;
 private float currentCharge =0f; // Normalized [0,1]
 private Coroutine chargeCoroutine;

 [Header("Settings - Rotation")]
 // Stores rotation input used to rotate the carried object
 private float currentRotationInput =0f;

 private void OnEnable()
 {
 if (InputManager.Instance != null)
 {
 InputManager.Instance.OnRotateInput += HandleRotation; // Subscribe rotation input
 InputManager.Instance.OnScrollInput += HandleScrollChange; // Subscribe scroll input
 InputManager.Instance.OnThrowStarted += StartThrowCharge; // Subscribe throw start
 InputManager.Instance.OnThrowCanceled += ReleaseThrow; // Subscribe throw cancel/release
 }
 }

 private void OnDisable()
 {
 if (InputManager.Instance != null)
 {
 InputManager.Instance.OnRotateInput -= HandleRotation; // Unsubscribe
 InputManager.Instance.OnScrollInput -= HandleScrollChange; // Unsubscribe
 InputManager.Instance.OnThrowStarted -= StartThrowCharge; // Unsubscribe
 InputManager.Instance.OnThrowCanceled -= ReleaseThrow; // Unsubscribe
 }
 }

 private void FixedUpdate()
 {
 if (isCarrying && currentCarriedObject != null)
 {
 MoveObjectToHoldPoint(); // Physically move carried object toward target
 RotateObject(); // Apply continuous rotation input
 }
 }

 /// <summary>
 /// Move the carried object toward the hold point while preventing clipping into walls.
 /// Uses physics MovePosition for stable behavior.
 /// </summary>
 private void MoveObjectToHoldPoint()
 {
 if (holdPoint == null || playerCameraTransform == null) return; // Preconditions

 // Default target is the world position of the hold point
 Vector3 targetPos = holdPoint.position;

 // --- WALL CHECK ---
 // Cast from camera to the target; if an obstacle intersects, clamp the target position
 RaycastHit hit;
 Vector3 direction = (targetPos - playerCameraTransform.position).normalized;
 float distanceToTarget = Vector3.Distance(playerCameraTransform.position, targetPos);

 if (Physics.Raycast(playerCameraTransform.position, direction, out hit, distanceToTarget, wallLayers))
 {
 // Keep the object a small offset in front of the wall to avoid penetration
 targetPos = hit.point - (direction *0.5f);
 }
 // -----------------------------------

 // Apply smoothed physics movement using MovePosition
 Vector3 smoothedPos = Vector3.Lerp(currentCarriedObject.position, targetPos, moveSmoothing * Time.fixedDeltaTime);
 currentCarriedObject.MovePosition(smoothedPos);

 // Zero velocities to avoid jitter
 currentCarriedObject.linearVelocity = Vector3.zero;
 currentCarriedObject.angularVelocity = Vector3.zero;
 }

 /// <summary>
 /// Rotate the carried object based on stored rotation input.
 /// </summary>
 private void RotateObject()
 {
 if (currentRotationInput ==0) return; // Nothing to do

 // Calculate rotation amount and apply via MoveRotation for physics stability
 float rotationAmount = currentRotationInput *100f * Time.fixedDeltaTime;
 Quaternion deltaRotation = Quaternion.Euler(Vector3.up * rotationAmount);
 currentCarriedObject.MoveRotation(currentCarriedObject.rotation * deltaRotation);
 }

 /// <summary>
 /// Adjust hold distance via scroll input while clamping inside allowed range.
 /// </summary>
 private void HandleScrollChange(float value)
 {
 if (!isCarrying) return; // Only valid while carrying

 holdDistance += value * scrollSensitivity;
 holdDistance = Mathf.Clamp(holdDistance, minHoldDistance, maxHoldDistance);

 if (holdPoint != null)
 {
 holdPoint.localPosition = new Vector3(0,0,holdDistance); // Update local hold point
 }
 }

 /// <summary>
 /// Begin charging a throw. Starts a coroutine that updates UI while charging.
 /// </summary>
 private void StartThrowCharge()
 {
 if (!isCarrying || currentCarriedObject == null) return;
 if (currentItemScript != null && !currentItemScript.CanBeThrown()) return; // Item-specific check

 if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
 chargeCoroutine = StartCoroutine(ChargeRoutine());
 }

 /// <summary>
 /// Release the charged throw, compute final force and apply impulse to the dropped object.
 /// </summary>
 private void ReleaseThrow()
 {
 if (!isChargingThrow) return; // Not charging

 if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
 isChargingThrow = false;

 float finalForce = Mathf.Lerp(minThrowForce, maxThrowForce, currentCharge); // Interpolate force

 Rigidbody objToThrow = currentCarriedObject;
 DropObjectLogic(false); // Drop without resetting UI (we do it manually)

 if (objToThrow != null && playerCameraTransform != null)
 {
 objToThrow.AddForce(playerCameraTransform.forward * finalForce, ForceMode.Impulse); // Apply impulse
 }

 if (onThrowChargeChanged != null) onThrowChargeChanged.Raise(0f); // Reset UI
 currentCharge =0f;
 }

 private IEnumerator ChargeRoutine()
 {
 isChargingThrow = true;
 currentCharge =0f;

 while (isChargingThrow)
 {
 currentCharge += Time.deltaTime / maxChargeTime; // Normalize charge over time
 currentCharge = Mathf.Clamp01(currentCharge);
 if (onThrowChargeChanged != null) onThrowChargeChanged.Raise(currentCharge); // UI update
 yield return null;
 }
 }

 /// <summary>
 /// Request pickup handler invoked by other systems. Sets up physics and ignores collisions
 /// with player collider while carrying.
 /// </summary>
 public void RequestPickup(Rigidbody targetRb)
 {
 if (isCarrying || targetRb == null) return; // Already carrying or invalid target

 isCarrying = true;
 currentCarriedObject = targetRb;
 currentItemScript = targetRb.GetComponent<CarryableItem>();

 currentCarriedObject.useGravity = false; // Disable gravity while carried
 currentCarriedObject.linearDamping =10f; // Dampen motion
 currentCarriedObject.constraints = RigidbodyConstraints.FreezeRotation; // Allow rotation only via script

 // FIX: Prevent collisions between player and carried item
 if (playerCollider != null)
 {
 Collider itemCol = targetRb.GetComponent<Collider>();
 if (itemCol != null) Physics.IgnoreCollision(playerCollider, itemCol, true);
 }

 HandleScrollChange(0); // Initialize holdPoint distance
 }

 /// <summary>
 /// Public API to drop the currently carried object.
 /// </summary>
 public void DropObject()
 {
 DropObjectLogic(true); // Reset UI on drop
 }

 private void DropObjectLogic(bool resetChargeUI)
 {
 if (!isCarrying || currentCarriedObject == null) return;

 if (isChargingThrow)
 {
 isChargingThrow = false;
 if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
 if (resetChargeUI && onThrowChargeChanged != null) onThrowChargeChanged.Raise(0f);
 }

 ResetObjectPhysics(); // Restore physics settings
 isCarrying = false;
 currentCarriedObject = null;
 currentItemScript = null;
 }

 private void ResetObjectPhysics()
 {
 if (currentCarriedObject == null) return;

 currentCarriedObject.useGravity = true;
 currentCarriedObject.linearDamping =1f; // Restore default damping
 currentCarriedObject.constraints = RigidbodyConstraints.None; // Allow natural rotation

 // Re-enable collision between player and item
 if (playerCollider != null)
 {
 Collider itemCol = currentCarriedObject.GetComponent<Collider>();
 if (itemCol != null) Physics.IgnoreCollision(playerCollider, itemCol, false);
 }
 }

 /// <summary>
 /// Store rotation input for physics update loop to consume.
 /// </summary>
 private void HandleRotation(float direction)
 {
 currentRotationInput = direction; // Cached and applied in FixedUpdate
 }

 public bool IsCarrying() => isCarrying; // Query whether carrying
 public bool IsObjectBeingCarried(Rigidbody targetRb) => isCarrying && currentCarriedObject == targetRb; // Query specific object
}