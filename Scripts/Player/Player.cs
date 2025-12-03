// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lightweight container that centralizes player-related components and input wrapper.
/// Responsible for instantiating input actions and exposing component references.
/// </summary>
public class Player : MonoBehaviour
{
    public InputActions controls { get; private set; }
    public PlayerMovement movement { get; private set; }
    // public PlayerAim aim { get; private set; }
    // public PlayerWeaponController weapon { get; private set; }
    // public PlayerWeaponVisuals weaponVisuals { get; private set; }
    // public PlayerInteraction interaction { get; private set; }

    private void Awake()
    {

        controls = new InputActions();
        movement = GetComponent<PlayerMovement>();


        // aim = GetComponent<PlayerAim>();
        // weapon = GetComponent<PlayerWeaponController>();
        // weaponVisuals = GetComponent<PlayerWeaponVisuals>();
        // interaction = GetComponent<PlayerInteraction>();

    }

    public void Start()
    {

        controls.Player.Enable();

    }

    private void OnEnable()
    {
        controls.Player.Enable();

    }

    private void OnDisable()
    {
        controls.Player.Disable();

    }
}
