// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;
using System; // Required for C# events (Action)

/// <summary>
/// Handles player movement using a CharacterController. Listens to central InputManager
/// events for movement, sprint and jump.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float defaultStandingSpeed =1f; // Speed when idle
    [SerializeField] private float defaultMoveSpeed =5f; // Normal move speed
    [SerializeField] private float defaultSprintSpeed =8f; // Sprint speed
    [SerializeField] private float acceleration =1f; // Lerp acceleration
    [SerializeField] private float gravity =-9.81f; // Gravity acceleration

    [Header("Jumping (MVP Out of Scope)")]
    [SerializeField] private float jumpVelocity =10f; // Jump initial velocity

    // Runtime state
    private float currentMoveSpeed =1f;
    private bool isRunning; // Sprint flag
    private CharacterController characterController;
    private Vector2 currentMovementInput; // Cached movement input
    private float verticalVelocity; // Y-axis velocity

    public void Awake()
    {
        characterController = GetComponent<CharacterController>(); // Cache controller
    }

    private void OnEnable()
    {
        // Ensure InputManager is available
        if (InputManager.Instance == null)
        {
            Debug.LogError("PlayerMovement: InputManager.Instance not found. Is boot scene loaded?");
            this.enabled = false; // Disable to avoid null references
            return;
        }

        // Subscribe to input events
        InputManager.Instance.OnMoveInput += HandleMoveInput;
        InputManager.Instance.OnSprintStatus += HandleSprintStatus;
        InputManager.Instance.OnJumpPressed += HandleJumpPressed;
    }

    private void OnDisable()
    {
        // Unsubscribe safely (guard for shutdown ordering)
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= HandleMoveInput;
            InputManager.Instance.OnSprintStatus -= HandleSprintStatus;
            InputManager.Instance.OnJumpPressed -= HandleJumpPressed;
        }
    }

    // --- Event Handlers ---
    private void HandleMoveInput(Vector2 input) { currentMovementInput = input; }
    private void HandleSprintStatus(bool sprintActive) { isRunning = sprintActive; }
    private void HandleJumpPressed() { Jump(); }

    // --- Main Loop ---
    void Update()
    {
        CalculateMoveSpeed();
        ApplyGravity();
        ApplyMovement();
    }

    private void CalculateMoveSpeed()
    {
        if (IsStandingStill())
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, defaultStandingSpeed, Time.deltaTime * acceleration);
        }
        else if (isRunning)
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, defaultSprintSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, defaultMoveSpeed, Time.deltaTime * acceleration);
        }
    }

    private void ApplyMovement()
    {
        Vector3 horizontalMove = new Vector3(currentMovementInput.x,0f, currentMovementInput.y);
        if (horizontalMove.magnitude >1f) horizontalMove.Normalize();

        horizontalMove = transform.TransformDirection(horizontalMove);
        horizontalMove *= currentMoveSpeed;

        Vector3 finalMove = horizontalMove;
        finalMove.y = verticalVelocity;

        characterController.Move(finalMove * Time.deltaTime);
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpVelocity; // Apply jump impulse
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity <0f)
        {
            verticalVelocity = -2f; // Keep player grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Integrate gravity
        }
    }

    private bool IsStandingStill() => currentMovementInput.magnitude <0.1f; // Helper
}