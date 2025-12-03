// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author
using UnityEngine;

/// <summary>
/// Central game state manager used as the authoritative source for global game state
/// (playing, paused, cutscene). Handles pause toggling and time scale updates. // short
/// </summary>
public class GameManager : SingletonManager<GameManager>
{
    [Header("State")] // Inspector grouping
    // Authoritative game state variable
    [SerializeField] private GameState currentState = GameState.Playing; // Current global state - serialized

    private void Start()
    {
        // Subscribe to the input manager's pause action
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPausePressed += TogglePauseState; // subscribe - short
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnPausePressed -= TogglePauseState; // unsubscribe - short
        }
    }

    /// <summary>
    /// Toggle between Playing and Paused states. Respects other non-toggleable states such as Cutscene. // short
    /// </summary>
    private void TogglePauseState()
    {
        // If in a special state (e.g. cutscene) ignore pause toggles
        if (currentState != GameState.Playing && currentState != GameState.Paused) return;

        // Toggle play/pause
        GameState newState = (currentState == GameState.Playing) ? GameState.Paused : GameState.Playing;

        SetGameState(newState); // delegate state change handling - short
    }

    /// <summary>
    /// Set the authoritative game state and apply associated side effects (timeScale, UI). // short
    /// </summary>
    public void SetGameState(GameState newState)
    {
        currentState = newState; // persist new state

        switch (currentState)
        {
            case GameState.Paused:
                Time.timeScale = 0f; // pause game time - short
                UIManager.Instance.TogglePauseMenu(true); // instruct UI to show pause menu - short
                break;

            case GameState.Playing:
                Time.timeScale = 1f; // resume game time - short
                UIManager.Instance.TogglePauseMenu(false); // instruct UI to hide pause menu - short
                break;
        }

        Debug.Log($"Game State Changed to: {currentState}"); // debug - short
    }

    // Read-only accessor for other scripts
    public GameState GetCurrentState() => currentState; // access current state - short
}