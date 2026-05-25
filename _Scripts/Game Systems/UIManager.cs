/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

/// <summary>
/// Oversees canvas interfaces and menu transitions dynamically across play states,
/// delegating interactions globally to manage game time and input handling.
/// </summary>
public class UIManager : SingletonManager<UIManager>, IUIManager
{
    [Header("Event Listening")]
    [SerializeField] private VoidEventChannelSO onPlayerDeath;

    [Header("Minecart Transition Events")]
    [SerializeField] private VoidEventChannelSO onMinecartArrivalEvent;
    [SerializeField] private VoidEventChannelSO onMinecartExtractionEvent;
    [SerializeField] private VoidEventChannelSO onShowLoadingEvent;

    [Header("Panels")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject interactionPromptPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject holdToStartPanel;

    [Header("Global UI")]
    [SerializeField] private GameObject loadingPanel;

    [Header("Transition UI (Minecart)")]
    [SerializeField] private GameObject fadeInUI;
    [SerializeField] private GameObject fadeOutUI;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button returnToHubButton;
    [SerializeField] private Button returnToMainMenuButton;

    [Header("Settings Menu Buttons")]
    [SerializeField] private Button closeSettingsButton;

    [Header("Game Over Buttons")]
    [SerializeField] private Button go_hubButton;
    [SerializeField] private Button go_mainMenuButton;
    [SerializeField] private Button go_quitButton;
    [SerializeField] private string hubSceneName = "HubScene";

    [Header("Tutorial Menu")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button pause_tutorialButton;
    [SerializeField] private Button closeTutorialButton;

    private bool isCursorLocked = true;
    private bool isGamePaused = false;
    private bool isGameOver = false;

    private float lastEscTime = 0f;
    private const float ESC_COOLDOWN = 0.2f;

    private void OnEnable()
    {
        if (onPlayerDeath != null) onPlayerDeath.OnEventRaised += HandleGameOver;
        if (onMinecartArrivalEvent != null) onMinecartArrivalEvent.OnEventRaised += PlayFadeIn;
        if (onMinecartExtractionEvent != null) onMinecartExtractionEvent.OnEventRaised += PlayFadeOut;
        if (onShowLoadingEvent != null) onShowLoadingEvent.OnEventRaised += () => ShowLoading(true);
    }

    private void OnDisable()
    {
        if (onPlayerDeath != null) onPlayerDeath.OnEventRaised -= HandleGameOver;
        if (onMinecartArrivalEvent != null) onMinecartArrivalEvent.OnEventRaised -= PlayFadeIn;
        if (onMinecartExtractionEvent != null) onMinecartExtractionEvent.OnEventRaised -= PlayFadeOut;
        if (onShowLoadingEvent != null) onShowLoadingEvent.OnEventRaised -= () => ShowLoading(true);
    }

    private void Start()
    {
        ShowHUD();
        SetCursorState(true);

        UnityEngine.UI.Button[] allButtons = FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (UnityEngine.UI.Button btn in allButtons)
        {
            btn.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayUIClick();
            });
        }

        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (loadingPanel) loadingPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (fadeInUI) fadeInUI.SetActive(false);
        if (fadeOutUI) fadeOutUI.SetActive(false);

        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        if (resumeButton) { resumeButton.onClick.RemoveAllListeners(); resumeButton.onClick.AddListener(ForceResumeGame); }
        if (settingsButton) { settingsButton.onClick.RemoveAllListeners(); settingsButton.onClick.AddListener(() => ToggleSettings(true)); }
        if (closeSettingsButton) { closeSettingsButton.onClick.RemoveAllListeners(); closeSettingsButton.onClick.AddListener(() => ToggleSettings(false)); }

        if (returnToHubButton) returnToHubButton.onClick.AddListener(LoadHubNormally);
        if (returnToMainMenuButton) returnToMainMenuButton.onClick.AddListener(LoadMainMenuState);

        if (go_hubButton) go_hubButton.onClick.AddListener(LoadHubNormally);
        if (go_mainMenuButton) go_mainMenuButton.onClick.AddListener(LoadMainMenuState);
        if (go_quitButton) go_quitButton.onClick.AddListener(Application.Quit);

        if (pause_tutorialButton) { pause_tutorialButton.onClick.RemoveAllListeners(); pause_tutorialButton.onClick.AddListener(() => ToggleTutorial(true)); }
        if (closeTutorialButton) { closeTutorialButton.onClick.RemoveAllListeners(); closeTutorialButton.onClick.AddListener(() => ToggleTutorial(false)); }
    }

    private void Update()
    {
        if (isGameOver) return;
        if (loadingPanel != null && loadingPanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.unscaledTime - lastEscTime < ESC_COOLDOWN) return;
            lastEscTime = Time.unscaledTime;

            if (tutorialPanel != null && tutorialPanel.activeSelf)
            {
                ToggleTutorial(false);
                return;
            }

            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                ToggleSettings(false);
                return;
            }

            if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
            {
                ForceResumeGame();
            }
            else
            {
                ForcePauseGame();
            }
        }

#if UNITY_EDITOR
        if (isCursorLocked && !isGamePaused && !isGameOver && Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
#endif
    }

    /// <summary>
    /// Displays or hides the UI representing a hold-to-start action interaction prompt.
    /// </summary>
    /// <param name="value">Target state for visibility.</param>
    public void ToggleHoldProgressBarUI(bool value)
    {
        if (holdToStartPanel != null)
            holdToStartPanel.SetActive(value);
    }

    /// <summary>
    /// Strictly forces the game into a paused state, shutting down all other UI.
    /// </summary>
    public void ForcePauseGame()
    {
        if (GameManager.Instance != null) GameManager.Instance.SetGameState(GameState.Paused);
        Time.timeScale = 0f;
        SetCursorState(false);

        if (hudPanel) hudPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Strictly forces the game to resume, clearing all pause-related UI.
    /// </summary>
    public void ForceResumeGame()
    {
        if (GameManager.Instance != null) GameManager.Instance.SetGameState(GameState.Playing);
        Time.timeScale = 1f;
        SetCursorState(true);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (hudPanel) hudPanel.SetActive(true);

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Alternative method to initiate the game pause logic.
    /// </summary>
    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        SetCursorState(false);

        if (hudPanel) hudPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Alternative method to resume the game logic.
    /// </summary>
    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
        SetCursorState(true);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (hudPanel) hudPanel.SetActive(true);

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Safely toggles the settings panel. Forces HUD to turn off when opening,
    /// and routes directly to PauseGame when closing to prevent state desync.
    /// </summary>
    /// <param name="isOpen">True to open the settings, false to close.</param>
    public void ToggleSettings(bool isOpen)
    {
        if (settingsPanel) settingsPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (hudPanel) hudPanel.SetActive(false);
        }
        else
        {
            ForcePauseGame();
        }
    }

    /// <summary>
    /// Manages the toggling of the standard pause menu and associated cursor states.
    /// </summary>
    /// <param name="isPaused">Expected target pause state.</param>
    public void TogglePauseMenu(bool isPaused)
    {
        if (hudPanel) hudPanel.SetActive(!isPaused);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(isPaused);

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
        isCursorLocked = !isPaused;
    }

    private void HandleGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        StartCoroutine(ShowGameOverRoutine());
    }

    private System.Collections.IEnumerator ShowGameOverRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        if (hudPanel) hudPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(true);
        SetCursorState(false);
    }

    /// <summary>
    /// Re-enables the main Heads Up Display interface explicitly, masking menus.
    /// </summary>
    public void ShowHUD()
    {
        if (hudPanel) hudPanel.SetActive(true);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Modulates visibility for dynamic context interaction prompts dynamically.
    /// </summary>
    /// <param name="isActive">Boolean flag asserting target interaction prompt active status.</param>
    public void SetInteractionPromptState(bool isActive)
    {
        if (interactionPromptPanel) interactionPromptPanel.SetActive(isActive);
    }

    /// <summary>
    /// Centralized modifier forcing cursor locking bounds dynamically updating visibility appropriately.
    /// </summary>
    /// <param name="lockCursor">True locks and hides the cursor, false releases and shows it.</param>
    public void SetCursorState(bool lockCursor)
    {
        isCursorLocked = lockCursor;
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// Transitions logic rendering opaque blocking elements preventing actions between networking steps.
    /// </summary>
    /// <param name="show">Flag declaring intent to transition to a loading visual state.</param>
    public void ShowLoading(bool show)
    {
        if (loadingPanel)
        {
            loadingPanel.SetActive(show);
            if (show) SetCursorState(false);
        }
    }

    /// <summary>
    /// Safely toggles the tutorial panel using the same authority as settings.
    /// </summary>
    /// <param name="isOpen">True to open, false to close.</param>
    public void ToggleTutorial(bool isOpen)
    {
        if (tutorialPanel) tutorialPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (hudPanel) hudPanel.SetActive(false);
        }
        else
        {
            ForcePauseGame();
        }
    }

    /// <summary>
    /// Bypasses logical routing transitioning fully loaded states straight onto standard hub scene environments.
    /// </summary>
    public void LoadHub()
    {
        Time.timeScale = 1f;
        ShowLoading(true);
        SceneManager.LoadScene(hubSceneName);
    }

    /// <summary>
    /// Kills the executable application gracefully.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Navigates the active scene back into the core Hub map correctly, clearing first boot tracking flags.
    /// </summary>
    public void LoadHubNormally()
    {
        Time.timeScale = 1f;
        HubUIManager.isFirstBoot = false;
        ShowLoading(true);
        if (LevelManager.Instance) LevelManager.Instance.ReturnToHub();
        else SceneManager.LoadScene(hubSceneName);
    }

    /// <summary>
    /// Hard resets game environment flags executing initialization flows replicating fresh launch states.
    /// </summary>
    public void LoadMainMenuState()
    {
        Time.timeScale = 1f;
        HubUIManager.isFirstBoot = true;
        ShowLoading(true);
        SceneManager.LoadScene(hubSceneName);
    }

    private void PlayFadeIn() { if (fadeInUI) fadeInUI.SetActive(true); }
    private void PlayFadeOut() { if (fadeOutUI) fadeOutUI.SetActive(true); }
}