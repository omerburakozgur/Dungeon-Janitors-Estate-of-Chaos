/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;

/// <summary>
/// Central game state manager used as the authoritative source for global game state.
/// Input and UI hierarchy are handled by the UIManager. This script only enforces the logical state (Time).
/// </summary>
public class GameManager : SingletonManager<GameManager>
{
    [Header("State")]
    [SerializeField] private GameState currentState = GameState.Playing;

    /// <summary>
    /// Set the authoritative game state and apply associated logical side effects (timeScale).
    /// Called explicitly by the UIManager when the pause hierarchy is resolved.
    /// </summary>
    public void SetGameState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Paused:
                Time.timeScale = 0f;
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }
    }

    /// <summary>
    /// Polls currently mapped State Enum validating internal architecture logic transitions passively.
    /// </summary>
    public GameState GetCurrentState() => currentState;
}