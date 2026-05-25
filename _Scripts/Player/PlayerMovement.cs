/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using System;

/// <summary>
/// Handles player movement using a CharacterController. Listens to central InputManager
/// events for movement, sprint, and jump. Triggers footstep events for camera shake and audio.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float defaultStandingSpeed = 1f;
    [SerializeField] private float defaultMoveSpeed = 5f;
    [SerializeField] private float defaultSprintSpeed = 8f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Jumping")]
    [SerializeField] private float jumpVelocity = 10f;

    [Header("Game Feel (Footsteps & Shake)")]
    [Tooltip("Event channel to trigger camera shake on each step.")]
    [SerializeField] private VoidEventChannelSO onFootstepShakeChannel;
    [Tooltip("Time between steps while walking.")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [Tooltip("Time between steps while sprinting.")]
    [SerializeField] private float sprintStepInterval = 0.3f;

    private float currentMoveSpeed;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float verticalVelocity;
    private bool isRunning;
    private float stepTimer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        currentMoveSpeed = defaultMoveSpeed;
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput += HandleMove;
            InputManager.Instance.OnSprintStatus += HandleSprint;
            InputManager.Instance.OnJumpPressed += Jump;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMoveInput -= HandleMove;
            InputManager.Instance.OnSprintStatus -= HandleSprint;
            InputManager.Instance.OnJumpPressed -= Jump;
        }
    }

    private void Update()
    {
        Move();
        ApplyGravity();
        HandleFootsteps();
    }

    private void Move()
    {
        Vector3 movement = (transform.right * moveDirection.x + transform.forward * moveDirection.y) * currentMoveSpeed;
        movement.y = verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }

    private void HandleMove(Vector2 input)
    {
        moveDirection = input;
    }

    private void HandleSprint(bool running)
    {
        isRunning = running;
        currentMoveSpeed = running ? defaultSprintSpeed : defaultMoveSpeed;
    }

    private void HandleFootsteps()
    {
        if (characterController.isGrounded && moveDirection.magnitude > 0.1f)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                TriggerFootstep();
                stepTimer = isRunning ? sprintStepInterval : walkStepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private void TriggerFootstep()
    {
        if (onFootstepShakeChannel != null)
        {
            onFootstepShakeChannel.Raise();
        }
    }

    private void Jump()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = jumpVelocity;

            if (AudioManager.Instance && AudioManager.Instance.data)
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.data.jump, transform.position, 0.8f, 0.1f);
            }
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}