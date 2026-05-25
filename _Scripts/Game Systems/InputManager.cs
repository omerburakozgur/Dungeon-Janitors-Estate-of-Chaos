/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
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
    private InputActions controls;

    /// <summary>Movement vector event (x,z)</summary>
    public event Action<Vector2> OnMoveInput;
    /// <summary>Look delta event (mouse/gamepad)</summary>
    public event Action<Vector2> OnLookInput;
    /// <summary>Event raised explicitly when jump input is pressed</summary>
    public event Action OnJumpPressed;
    /// <summary>Event raised carrying sprint active boolean status</summary>
    public event Action<bool> OnSprintStatus;

    /// <summary>Standard short press interact started</summary>
    public event Action OnInteractPressed;
    /// <summary>Standard short press interact released</summary>
    public event Action OnInteractReleased;
    /// <summary>Hold interact explicitly started</summary>
    public event Action OnHoldInteractPressed;
    /// <summary>Hold interact explicitly canceled</summary>
    public event Action OnHoldInteractCanceled;

    /// <summary>Attack input explicitly triggered</summary>
    public event Action OnAttackPressed;
    /// <summary>Attack input released</summary>
    public event Action OnAttackReleased;
    /// <summary>Continuous track dictating sustained attack holding state</summary>
    public event Action<bool> OnAttackHeldStatus;

    /// <summary>Rotational input event</summary>
    public event Action<float> OnRotateInput;

    /// <summary>Throw charge initialization event</summary>
    public event Action OnThrowStarted;
    /// <summary>Throw release execution event</summary>
    public event Action OnThrowCanceled;

    /// <summary>Event carrying normalized mouse wheel scrolling axis delta</summary>
    public event Action<float> OnScrollInput;

    /// <summary>Global pause menu invocation event</summary>
    public event Action OnPausePressed;

    /// <summary>Hotbar specific key bindings events</summary>
    public event Action OnHotbar1Pressed;
    public event Action OnHotbar2Pressed;
    public event Action OnHotbar3Pressed;
    public event Action OnHotbar4Pressed;
    public event Action OnHotbar5Pressed;
    public event Action OnHotbar6Pressed;
    public event Action OnHolsterPressed;

    protected override void Awake()
    {
        base.Awake();
        controls = new InputActions();
    }

    private void OnEnable()
    {
        if (controls == null) return;

        controls.Player.Enable();

        controls.Player.Move.performed += HandleMove;
        controls.Player.Move.canceled += HandleMove;
        controls.Player.Look.performed += HandleLook;
        controls.Player.Look.canceled += HandleLook;

        controls.Player.Jump.performed += HandleJumpPerformed;
        controls.Player.Sprint.performed += HandleSprintPerformed;
        controls.Player.Sprint.canceled += HandleSprintCanceled;

        controls.Player.Interact.started += HandleInteractEvent;
        controls.Player.Interact.performed += HandleInteractEvent;
        controls.Player.Interact.canceled += HandleInteractEvent;

        controls.Player.Attack.performed += HandleAttackPerformed;
        controls.Player.Attack.canceled += HandleAttackCanceled;

        controls.Player.Rotate.performed += HandleRotate;
        controls.Player.Rotate.canceled += HandleRotate;
        controls.Player.Throw.started += HandleThrowStarted;
        controls.Player.Throw.canceled += HandleThrowCanceled;
        controls.Player.Scroll.performed += HandleScroll;

        controls.Player.Pause.performed += HandlePausePerformed;

        controls.Player.Hotbar1.performed += ctx => OnHotbar1Pressed?.Invoke();
        controls.Player.Hotbar2.performed += ctx => OnHotbar2Pressed?.Invoke();
        controls.Player.Hotbar3.performed += ctx => OnHotbar3Pressed?.Invoke();
        controls.Player.Hotbar4.performed += ctx => OnHotbar4Pressed?.Invoke();
        controls.Player.Hotbar5.performed += ctx => OnHotbar5Pressed?.Invoke();
        controls.Player.Hotbar6.performed += ctx => OnHotbar6Pressed?.Invoke();
        controls.Player.Holster.performed += HandleHolsterPerformed;
        controls.Player.Scroll.performed += HandleScroll;
    }

    private void OnDisable()
    {
        if (controls == null) return;

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

        controls.Player.Hotbar1.performed -= ctx => OnHotbar1Pressed?.Invoke();
        controls.Player.Hotbar2.performed -= ctx => OnHotbar2Pressed?.Invoke();
        controls.Player.Holster.performed -= HandleHolsterPerformed;

        controls.Player.Disable();
    }

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

    private void HandleThrowStarted(InputAction.CallbackContext context)
    {
        OnThrowStarted?.Invoke();
    }

    private void HandleThrowCanceled(InputAction.CallbackContext context)
    {
        OnThrowCanceled?.Invoke();
    }

    private void HandleScroll(InputAction.CallbackContext context)
    {
        float scrollValue = Mathf.Clamp(context.ReadValue<float>(), -1f, 1f);
        OnScrollInput?.Invoke(scrollValue);
    }

    private void HandlePausePerformed(InputAction.CallbackContext context)
    {
        OnPausePressed?.Invoke();
    }

    private void HandleHolsterPerformed(InputAction.CallbackContext context)
    {
        OnHolsterPressed?.Invoke();
    }
}