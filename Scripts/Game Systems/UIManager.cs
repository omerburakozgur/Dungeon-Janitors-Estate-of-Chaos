// Created by Ömer Burak ÖZGÜR - Dungeon Janitors: Estate of Chaos project author

using UnityEngine;

/// <summary>
/// Simple UI panel manager responsible for toggling HUD and pause panels and
/// managing cursor state. Does not contain game data.
/// </summary>
public class UIManager : SingletonManager<UIManager>
{
    [Header("Panels")]
    [SerializeField] private GameObject hudPanel; // Main HUD panel
    [SerializeField] private GameObject pauseMenuPanel; // Pause menu panel
    [SerializeField] private GameObject interactionPromptPanel; // Interaction prompt panel

    // Local cursor lock tracking
    private bool isCursorLocked = true;
    private void Start()
    {
        ShowHUD(); // Show HUD by default
        SetCursorState(true); // Lock cursor for gameplay
    }

    /// <summary>
    /// Show the main HUD and hide pause menu.
    /// </summary>
    public void ShowHUD()
    {
        hudPanel.SetActive(true);
        pauseMenuPanel.SetActive(false);
    }

    /// <summary>
    /// Toggle pause menu state and manage cursor visibility/locking.
    /// </summary>
    /// <param name="isPaused">True to open pause menu, false to close it.</param>
    public void TogglePauseMenu(bool isPaused)
    {
        hudPanel.SetActive(!isPaused);
        pauseMenuPanel.SetActive(isPaused);

        // Manage cursor state accordingly
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    /// <summary>
    /// Enable or disable the interaction prompt panel.
    /// </summary>
    public void SetInteractionPromptState(bool isActive)
    {
        interactionPromptPanel.SetActive(isActive);
    }

    /// <summary>
    /// Set cursor lock mode used for gameplay vs menus.
    /// </summary>
    /// <param name="lockCursor">True to lock/hide cursor, false to release and show it.</param>
    public void SetCursorState(bool lockCursor)
    {
        isCursorLocked = lockCursor;

        if (lockCursor)
        {
            // Gameplay mode: lock and hide cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Menu mode: show and release cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Editor convenience: ensure the cursor re-locks when play is resumed and user clicks the game view
    private void Update()
    {
#if UNITY_EDITOR
        if (isCursorLocked && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
#endif
    }
}