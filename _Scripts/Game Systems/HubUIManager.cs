/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Orchestrates all menu layers within the Hub environment enforcing precise traversal logic,
/// menu lockouts and first-time boot camera transition parameters.
/// </summary>
public class HubUIManager : SingletonManager<HubUIManager>, IUIManager
{
    public static bool isFirstBoot = true;

    [Header("--- MAIN MENU (INTRO) ---")]
    [SerializeField] private bool forceIntroForTesting = true;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject menuCameraObj;
    [SerializeField] private GameObject playerCameraObj;
    [SerializeField] private float cameraBlendTime = 2f;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button mm_settingsButton;
    [SerializeField] private Button mm_quitButton;

    [Header("--- HUB GAMEPLAY ---")]
    [SerializeField] private GameObject hubHudPanel;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI salvageText;

    [Header("Global UI & Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject holdToStartPanel;

    [Header("Mission Success (Result Screen)")]
    [SerializeField] private UI_MissionComplete missionSuccessPanel;

    [Header("Pause Menu Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeSettingsButton;
    [SerializeField] private Button quitButton;

    [Header("Tutorial Menu")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Button mm_tutorialButton;
    [SerializeField] private Button pause_tutorialButton;
    [SerializeField] private Button closeTutorialButton;

    private bool isPaused = false;
    private bool isShopOpen = false;

    private float lastEscTime = 0f;
    private const float ESC_COOLDOWN = 0.2f;

    protected override void Awake()
    {
        base.Awake();
        if (loadingPanel) loadingPanel.SetActive(false);
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (missionSuccessPanel) missionSuccessPanel.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetupButtonListeners();

        UnityEngine.UI.Button[] allButtons = FindObjectsByType<UnityEngine.UI.Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (UnityEngine.UI.Button btn in allButtons)
        {
            btn.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayUIClick();
            });
        }

        if (PlayerPrefs.GetInt("HasLastRunData", 0) == 1)
        {
            bool isSuccess = PlayerPrefs.GetInt("LastRunSuccess", 1) == 1;
            int gold = PlayerPrefs.GetInt("LastRunGold", 0);
            int salvage = PlayerPrefs.GetInt("LastRunSalvage", 0);

            PlayerPrefs.SetInt("HasLastRunData", 0);
            PlayerPrefs.Save();

            isFirstBoot = false;
            StartHubNormally();

            ShowMissionResult(isSuccess, gold, salvage);
        }
        else
        {
            if (isFirstBoot || forceIntroForTesting)
            {
                StartMainMenuMode();
            }
            else
            {
                StartHubNormally();
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.unscaledTime - lastEscTime < ESC_COOLDOWN) return;
            lastEscTime = Time.unscaledTime;

            if (isShopOpen) return;
            if (missionSuccessPanel != null && missionSuccessPanel.gameObject.activeSelf) return;

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

            if (mainMenuPanel != null && mainMenuPanel.activeSelf) return;

            if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
            {
                ForceResumeGame();
            }
            else
            {
                ForcePauseGame();
            }
        }
    }

    /// <summary>
    /// Strictly forces the hub into a paused state.
    /// </summary>
    public void ForcePauseGame()
    {
        isPaused = true;

        if (GameManager.Instance != null) GameManager.Instance.SetGameState(GameState.Paused);
        Time.timeScale = 0f;
        SetCursorState(false);

        if (hubHudPanel) hubHudPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(true);

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Strictly forces the hub to resume gameplay.
    /// </summary>
    public void ForceResumeGame()
    {
        isPaused = false;

        if (GameManager.Instance != null) GameManager.Instance.SetGameState(GameState.Playing);
        Time.timeScale = 1f;
        SetCursorState(true);

        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (tutorialPanel) tutorialPanel.SetActive(false);

        if (hubHudPanel) hubHudPanel.SetActive(true);

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Displays mission end results overlay and disables player controls.
    /// </summary>
    public void ShowMissionResult(bool isSuccess, int gold, int salvage)
    {
        if (hubHudPanel) hubHudPanel.SetActive(false);

        if (Player.Instance != null) Player.Instance.SetControlActive(false);

        if (missionSuccessPanel != null)
            missionSuccessPanel.Show(isSuccess, gold, salvage);
    }

    private void StartMainMenuMode()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(true);
        if (hubHudPanel) hubHudPanel.SetActive(false);

        if (menuCameraObj) menuCameraObj.SetActive(true);
        if (playerCameraObj) playerCameraObj.SetActive(false);

        if (Player.Instance != null)
        {
            Player.Instance.SetControlActive(false);

            PlayerVisualsManager visualsManager = Player.Instance.GetComponentInChildren<PlayerVisualsManager>();
            if (visualsManager != null) visualsManager.SetTPSView();
        }

        SetCursorState(false);
    }

    private void StartTransitionToHub()
    {
        StartCoroutine(TransitionRoutine());
    }

    /// <summary>
    /// Explicitly enables or disables UI overlays guiding player towards interactable targets.
    /// </summary>
    public void ToggleHoldProgressBarUI(bool value)
    {
        if (holdToStartPanel != null)
            holdToStartPanel.SetActive(value);
    }

    private IEnumerator TransitionRoutine()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);

        if (playerCameraObj) playerCameraObj.SetActive(true);
        if (menuCameraObj) menuCameraObj.SetActive(false);

        yield return new WaitForSeconds(cameraBlendTime);

        if (!forceIntroForTesting) isFirstBoot = false;

        if (Player.Instance != null)
        {
            PlayerVisualsManager visualsManager = Player.Instance.GetComponentInChildren<PlayerVisualsManager>();
            if (visualsManager != null) visualsManager.SetFPSView();
        }

        StartHubNormally();
    }

    private void StartHubNormally()
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(false);

        if (menuCameraObj) menuCameraObj.SetActive(false);
        if (playerCameraObj) playerCameraObj.SetActive(true);

        if (hubHudPanel) hubHudPanel.SetActive(true);

        if (Player.Instance != null) Player.Instance.SetControlActive(true);

        SetCursorState(true);
        UpdateEconomyUI();
    }

    /// <summary>
    /// Explicitly routes the activation of tutorial components based on global pause contexts.
    /// </summary>
    public void ToggleTutorial(bool isOpen)
    {
        if (tutorialPanel) tutorialPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (hubHudPanel) hubHudPanel.SetActive(false);
        }
        else
        {
            if (isPaused)
            {
                ForcePauseGame();
            }
            else
            {
                if (mainMenuPanel) mainMenuPanel.SetActive(true);
                if (hubHudPanel) hubHudPanel.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Explicitly triggers HUD restoration for use from post-processing external overlays.
    /// </summary>
    public void ShowHUD()
    {
        if (hubHudPanel) hubHudPanel.SetActive(true);
    }

    private void SetupButtonListeners()
    {
        if (playButton) { playButton.onClick.RemoveAllListeners(); playButton.onClick.AddListener(StartTransitionToHub); }
        if (mm_settingsButton) { mm_settingsButton.onClick.RemoveAllListeners(); mm_settingsButton.onClick.AddListener(() => ToggleSettings(true)); }
        if (mm_quitButton) { mm_quitButton.onClick.RemoveAllListeners(); mm_quitButton.onClick.AddListener(Application.Quit); }

        if (resumeButton) { resumeButton.onClick.RemoveAllListeners(); resumeButton.onClick.AddListener(ForceResumeGame); }
        if (settingsButton) { settingsButton.onClick.RemoveAllListeners(); settingsButton.onClick.AddListener(() => ToggleSettings(true)); }
        if (closeSettingsButton) { closeSettingsButton.onClick.RemoveAllListeners(); closeSettingsButton.onClick.AddListener(() => ToggleSettings(false)); }
        if (quitButton) { quitButton.onClick.RemoveAllListeners(); quitButton.onClick.AddListener(Application.Quit); }

        if (mm_tutorialButton) { mm_tutorialButton.onClick.RemoveAllListeners(); mm_tutorialButton.onClick.AddListener(() => ToggleTutorial(true)); }
        if (pause_tutorialButton) { pause_tutorialButton.onClick.RemoveAllListeners(); pause_tutorialButton.onClick.AddListener(() => ToggleTutorial(true)); }
        if (closeTutorialButton) { closeTutorialButton.onClick.RemoveAllListeners(); closeTutorialButton.onClick.AddListener(() => ToggleTutorial(false)); }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        SetCursorState(!isPaused);

        if (isPaused)
        {
            if (hubHudPanel) hubHudPanel.SetActive(false);
            if (settingsPanel) settingsPanel.SetActive(false);
            if (tutorialPanel) tutorialPanel.SetActive(false);

            if (pauseMenuPanel) pauseMenuPanel.SetActive(true);
        }
        else
        {
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (settingsPanel) settingsPanel.SetActive(false);
            if (tutorialPanel) tutorialPanel.SetActive(false);

            if (hubHudPanel) hubHudPanel.SetActive(true);
        }

        if (UnityEngine.EventSystems.EventSystem.current != null)
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    }

    /// <summary>
    /// Safely toggles the settings panel in the Hub.
    /// Explicitly shuts down the HUD to prevent overlap bugs when exiting via ESC.
    /// </summary>
    public void ToggleSettings(bool isOpen)
    {
        if (settingsPanel) settingsPanel.SetActive(isOpen);

        if (isOpen)
        {
            if (mainMenuPanel) mainMenuPanel.SetActive(false);
            if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
            if (hubHudPanel) hubHudPanel.SetActive(false);
        }
        else
        {
            if (isPaused)
            {
                ForcePauseGame();
            }
            else
            {
                if (mainMenuPanel) mainMenuPanel.SetActive(true);
                if (hubHudPanel) hubHudPanel.SetActive(false);
            }

            if (UnityEngine.EventSystems.EventSystem.current != null)
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Extracts cached global progression states pushing visual elements up-to-date.
    /// </summary>
    public void UpdateEconomyUI()
    {
        int g = PlayerPrefs.GetInt(PlayerPrefsKeys.TOTAL_GOLD, 0);
        int s = PlayerPrefs.GetInt(PlayerPrefsKeys.TOTAL_SALVAGE, 0);

        if (goldText) goldText.text = g.ToString();
        if (salvageText) salvageText.text = s.ToString();
    }

    /// <summary>
    /// Manages cursor locking state for appropriate interactive availability cleanly globally.
    /// </summary>
    public void SetCursorState(bool isLocked)
    {
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    /// <summary>
    /// Evaluates if the shop environment menu binds input rendering appropriately avoiding conflicts.
    /// </summary>
    public void SetShopState(bool isOpen)
    {
        isShopOpen = isOpen;
        SetCursorState(!isOpen);
    }

    /// <summary>
    /// Displays or hides transitional network delay screen blockers.
    /// </summary>
    public void ShowLoading(bool show)
    {
        if (loadingPanel) loadingPanel.SetActive(show);
    }
}