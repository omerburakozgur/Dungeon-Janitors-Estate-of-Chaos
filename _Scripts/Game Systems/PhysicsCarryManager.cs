/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System.Collections;

/// <summary>
/// Physics-based object carrying manager.
/// Uses velocity-based movement to prevent objects from clipping through walls
/// while maintaining a smooth and responsive hold feel.
/// </summary>
public class PhysicsCarryManager : SingletonManager<PhysicsCarryManager>
{
    [Header("Settings - Holding")]
    [Tooltip("Default hold distance from the camera.")]
    [SerializeField] private float holdDistance = 2.5f;

    [Tooltip("How fast the object moves to the target hold position. Higher is snappier.")]
    [SerializeField] private float moveSmoothing = 15f;

    [Tooltip("How fast the object rotates to match the player's rotation.")]
    [SerializeField] private float rotationSmoothing = 10f;

    [Header("Settings - Throwing")]
    [Tooltip("Minimum throw impulse applied immediately upon release.")]
    [SerializeField] private float minThrowForce = 5f;

    [Tooltip("Maximum throw impulse applied after a full charge.")]
    [SerializeField] private float maxThrowForce = 25f;

    [Tooltip("Time in seconds required to reach the maximum throw force.")]
    [SerializeField] private float maxChargeTime = 1.5f;

    [Header("Dependencies")]
    [Tooltip("Reference to the player's camera to determine hold position.")]
    [SerializeField] private Transform playerCameraTransform;

    [Tooltip("The player's collider to ignore collisions while carrying.")]
    [SerializeField] private Collider playerCollider;

    [Header("Event Broadcasting")]
    [Tooltip("Broadcasts the normalized charge amount [0..1] for UI elements.")]
    [SerializeField] private FloatEventChannelSO onThrowChargeChanged;

    private Rigidbody currentCarriedObject;
    private float currentCharge = 0f;
    private bool isChargingThrow = false;
    private Coroutine chargeCoroutine;

    protected override void Awake()
    {
        base.Awake();

        if (playerCameraTransform == null && Camera.main != null)
        {
            playerCameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnThrowStarted += StartThrowCharge;
            InputManager.Instance.OnThrowCanceled += ReleaseThrow;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnThrowStarted -= StartThrowCharge;
            InputManager.Instance.OnThrowCanceled -= ReleaseThrow;
        }
    }

    private void FixedUpdate()
    {
        if (currentCarriedObject != null && playerCameraTransform != null)
        {
            MoveObjectWithPhysics();
        }
    }

    private void MoveObjectWithPhysics()
    {
        Vector3 targetPos = playerCameraTransform.position + (playerCameraTransform.forward * holdDistance);
        Vector3 directionToTarget = targetPos - currentCarriedObject.position;

        float distance = directionToTarget.magnitude;

        float speedMultiplier = Mathf.Clamp(distance * moveSmoothing, 0f, 30f);
        currentCarriedObject.linearVelocity = directionToTarget.normalized * speedMultiplier;

        Quaternion targetRotation = Quaternion.LookRotation(playerCameraTransform.forward, Vector3.up);
        currentCarriedObject.MoveRotation(Quaternion.Slerp(currentCarriedObject.rotation, targetRotation, rotationSmoothing * Time.fixedDeltaTime));
    }

    /// <summary>
    /// Attaches the provided rigidbody to the player's carrying system, removing gravity and ignoring player collision.
    /// </summary>
    /// <param name="targetRigidbody">The rigidbody to be carried.</param>
    public void RequestPickup(Rigidbody targetRigidbody)
    {
        if (currentCarriedObject != null || targetRigidbody == null) return;

        currentCarriedObject = targetRigidbody;

        currentCarriedObject.useGravity = false;
        currentCarriedObject.linearVelocity = Vector3.zero;
        currentCarriedObject.angularVelocity = Vector3.zero;

        currentCarriedObject.linearDamping = 5f;
        currentCarriedObject.angularDamping = 5f;

        if (playerCollider != null)
        {
            Collider objCol = currentCarriedObject.GetComponent<Collider>();
            if (objCol != null) Physics.IgnoreCollision(playerCollider, objCol, true);
        }
    }

    /// <summary>
    /// Releases the currently carried object back into the world physics simulation.
    /// </summary>
    public void DropObject()
    {
        DropObjectLogic(true);
    }

    private void DropObjectLogic(bool resetUI)
    {
        if (currentCarriedObject == null) return;

        currentCarriedObject.useGravity = true;
        currentCarriedObject.linearDamping = 0f;
        currentCarriedObject.angularDamping = 0.05f;

        if (playerCollider != null)
        {
            Collider objCol = currentCarriedObject.GetComponent<Collider>();
            if (objCol != null) Physics.IgnoreCollision(playerCollider, objCol, false);
        }

        currentCarriedObject = null;

        if (resetUI && onThrowChargeChanged != null)
        {
            onThrowChargeChanged.Raise(0f);
        }
    }

    private void StartThrowCharge()
    {
        if (currentCarriedObject == null) return;

        if (currentCarriedObject.TryGetComponent<CarryableItem>(out var carryable) && !carryable.CanBeThrown())
        {
            DropObject();
            return;
        }

        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        chargeCoroutine = StartCoroutine(ChargeRoutine());
    }

    private void ReleaseThrow()
    {
        if (!isChargingThrow) return;

        if (chargeCoroutine != null) StopCoroutine(chargeCoroutine);
        isChargingThrow = false;

        float finalForce = Mathf.Lerp(minThrowForce, maxThrowForce, currentCharge);

        Rigidbody objToThrow = currentCarriedObject;
        DropObjectLogic(false);

        if (objToThrow != null && playerCameraTransform != null)
        {
            objToThrow.AddForce(playerCameraTransform.forward * finalForce, ForceMode.Impulse);
        }

        if (onThrowChargeChanged != null) onThrowChargeChanged.Raise(0f);
        currentCharge = 0f;
    }

    private IEnumerator ChargeRoutine()
    {
        isChargingThrow = true;
        currentCharge = 0f;

        while (isChargingThrow)
        {
            currentCharge += Time.deltaTime / maxChargeTime;
            currentCharge = Mathf.Clamp01(currentCharge);

            if (onThrowChargeChanged != null)
                onThrowChargeChanged.Raise(currentCharge);

            yield return null;
        }
    }

    /// <summary>
    /// Checks whether the player is currently carrying any object.
    /// </summary>
    /// <returns>True if an object is being carried, false otherwise.</returns>
    public bool IsCarrying() => currentCarriedObject != null;

    /// <summary>
    /// Verifies if a specific Rigidbody is the exact object currently being carried by the player.
    /// </summary>
    /// <param name="rb">The Rigidbody to check.</param>
    /// <returns>True if the specified Rigidbody is currently carried.</returns>
    public bool IsObjectBeingCarried(Rigidbody rb) => currentCarriedObject == rb;
}