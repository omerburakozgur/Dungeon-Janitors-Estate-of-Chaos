// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Centralized input manager that reads raw input from the Unity Input System and
/// exposes it via C# events for other systems to subscribe to.
/// Handles movement, look, actions, interactions and extended features such as scrolling and throwing.
/// </summary>
public class InputManager : SingletonManager<InputManager>
{
    private InputActions controls; // Generated InputActions wrapper

    // --- Events exposed by this manager ---
    public event Action<Vector2> OnMoveInput; // Movement vector (x,z)
    public event Action<Vector2> OnLookInput; // Look delta (mouse/gamepad)

    public event Action OnJumpPressed; // Jump pressed
    public event Action<bool> OnSprintStatus; // Sprint start/stop status

    // Interaction events (short press / hold)
    public event Action OnInteractPressed;
    public event Action OnInteractReleased;
    public event Action OnHoldInteractPressed;
    public event Action OnHoldInteractCanceled;

    // Attack / cleaning input events
    public event Action OnAttackPressed;
    public event Action OnAttackReleased;
    public event Action<bool> OnAttackHeldStatus;

    // Rotation (Q/E axis) for quick rotation
    public event Action<float> OnRotateInput;

    // Throw start/cancel for charge-and-release mechanics
    public event Action OnThrowStarted;
    public event Action OnThrowCanceled;

    // Scroll input for zoom or selection (mouse wheel)
    public event Action<float> OnScrollInput;

    // UI events
    public event Action OnPausePressed;

    // Hotbar shortcuts
    public event Action OnHotbar1Pressed; // Hotbar slot1
    public event Action OnHotbar2Pressed; // Hotbar slot2

    protected override void Awake()
    {
        base.Awake();
        controls = new InputActions(); // Instantiate generated input actions
    }

    private void OnEnable()
    {
        if (controls == null) return;

        controls.Player.Enable(); // Enable input action map

        // Bind movement and look
        controls.Player.Move.performed += HandleMove;
        controls.Player.Move.canceled += HandleMove;
        controls.Player.Look.performed += HandleLook;
        controls.Player.Look.canceled += HandleLook;

        // Actions: Jump and Sprint
        controls.Player.Jump.performed += HandleJumpPerformed;
        controls.Player.Sprint.performed += HandleSprintPerformed;
        controls.Player.Sprint.canceled += HandleSprintCanceled;

        // Interaction with support for Tap and Hold interactions
        controls.Player.Interact.started += HandleInteractEvent;
        controls.Player.Interact.performed += HandleInteractEvent;
        controls.Player.Interact.canceled += HandleInteractEvent;

        // Attack input
        controls.Player.Attack.performed += HandleAttackPerformed;
        controls.Player.Attack.canceled += HandleAttackCanceled;

        // Extended inputs (rotation, throw, scroll)
        controls.Player.Rotate.performed += HandleRotate;
        controls.Player.Rotate.canceled += HandleRotate;
        controls.Player.Throw.started += HandleThrowStarted;
        controls.Player.Throw.canceled += HandleThrowCanceled;
        controls.Player.Scroll.performed += HandleScroll;

        // UI
        controls.Player.Pause.performed += HandlePausePerformed;

        // Hotbar shortcuts
        controls.Player.Hotbar1.performed += ctx => OnHotbar1Pressed?.Invoke();
        controls.Player.Hotbar2.performed += ctx => OnHotbar2Pressed?.Invoke();
    }

    private void OnDisable()
    {
        if (controls == null) return;

        // Unsubscribe all callbacks to avoid memory leaks
        controls.Player.Move.performed -= HandleMove;
        controls.Player.Move.canceled -= HandleMove;
        controls.Player.Look.performed -= HandleLook;
        controls.Player.Look.canceled -= HandleLook;
        controls.Player.Jump.performed -= HandleJumpPerformed;
        controls.Player.Sprint.performed -= HandleSprintPerformed;
        controls.Player.Sprint.canceled -= HandleSprintCanceled;

        controls.Player.Interact.started -= HandleInteractEvent;
        controls.Player.Interact.performed -= HandleInteractEvent;
        controls.Player.Interact.canceled -= HandleInteractEvent;

        controls.Player.Attack.performed -= HandleAttackPerformed;
        controls.Player.Attack.canceled -= HandleAttackCanceled;

        controls.Player.Rotate.performed -= HandleRotate;
        controls.Player.Rotate.canceled -= HandleRotate;
        controls.Player.Throw.started -= HandleThrowStarted;
        controls.Player.Throw.canceled -= HandleThrowCanceled;
        controls.Player.Scroll.performed -= HandleScroll;

        controls.Player.Pause.performed -= HandlePausePerformed;

        // Hotbar
        controls.Player.Hotbar1.performed -= ctx => OnHotbar1Pressed?.Invoke();
        controls.Player.Hotbar2.performed -= ctx => OnHotbar2Pressed?.Invoke();

        controls.Player.Disable(); // Disable action map
    }

    // --- Handlers that translate input system callbacks into manager events ---
    private void HandleMove(InputAction.CallbackContext context)
    {
        OnMoveInput?.Invoke(context.ReadValue<Vector2>());
    }

    private void HandleLook(InputAction.CallbackContext context)
    {
        OnLookInput?.Invoke(context.ReadValue<Vector2>());
    }

    private void HandleJumpPerformed(InputAction.CallbackContext context)
    {
        OnJumpPressed?.Invoke();
    }

    private void HandleSprintPerformed(InputAction.CallbackContext context)
    {
        OnSprintStatus?.Invoke(true);
    }

    private void HandleSprintCanceled(InputAction.CallbackContext context)
    {
        OnSprintStatus?.Invoke(false);
    }

    private void HandleInteractEvent(InputAction.CallbackContext context)
    {
        var interaction = context.interaction;

        if (interaction is UnityEngine.InputSystem.Interactions.TapInteraction)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                OnInteractPressed?.Invoke();
            }
        }
        else if (interaction is UnityEngine.InputSystem.Interactions.HoldInteraction)
        {
            if (context.phase == InputActionPhase.Started)
            {
                OnHoldInteractPressed?.Invoke();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                OnHoldInteractCanceled?.Invoke();
            }
        }
    }

    private void HandleAttackPerformed(InputAction.CallbackContext context)
    {
        OnAttackPressed?.Invoke();
        OnAttackHeldStatus?.Invoke(true);
    }

    private void HandleAttackCanceled(InputAction.CallbackContext context)
    {
        OnAttackReleased?.Invoke();
        OnAttackHeldStatus?.Invoke(false);
    }

    private void HandleRotate(InputAction.CallbackContext context)
    {
        OnRotateInput?.Invoke(context.ReadValue<float>());
    }

    // --- Throw and Scroll handlers ---
    private void HandleThrowStarted(InputAction.CallbackContext context)
    {
        OnThrowStarted?.Invoke(); // Begin charge
    }

    private void HandleThrowCanceled(InputAction.CallbackContext context)
    {
        OnThrowCanceled?.Invoke(); // Release/throw
    }

    private void HandleScroll(InputAction.CallbackContext context)
    {
        // Normalize scroll input between -1 and1 for downstream consumers
        float scrollValue = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
        OnScrollInput?.Invoke(scrollValue);
    }

    private void HandlePausePerformed(InputAction.CallbackContext context)
    {
        OnPausePressed?.Invoke();
    }
}