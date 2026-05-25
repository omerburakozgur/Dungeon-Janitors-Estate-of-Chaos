/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Central scene and level transition authority. Evaluates mission results, caches
/// ongoing session loot data, and bridges UI states during background environment loading.
/// </summary>
public class LevelManager : SingletonManager<LevelManager>
{
    [Header("Scene Configuration")]
    [Tooltip("Hub sahnesinin Build Settings'deki tam adi")]
    [SerializeField] private string hubSceneName = "HubScene";
    [Tooltip("Varsayilan test gorevi")]
    [SerializeField] private string defaultMissionScene = "Level_Barracks_01";

    [Header("Mission Settings")]
    [Tooltip("Gorevi basarmak icin temizlenmesi gereken oran (0.0 - 1.0)")]
    [SerializeField] private float successThreshold = 0.85f;
    [Tooltip("Sahnede temizlenmesi gereken toplam leke sayisi.")]
    [SerializeField] private int totalStainsInLevel = 20;

    [Header("Transit Settings")]
    [Tooltip("Yukleme ekraninda / Tunelde gececek saniye cinsinden sure")]
    [SerializeField] public float transitDuration = 8f;

    [Header("Listening To")]
    [SerializeField] private VoidEventChannelSO onDecalFullyCleanedEvent;

    private int sessionGold = 0;
    private int sessionSalvage = 0;
    private int currentCleanedCount = 0;

    private bool isLoading = false;
    public System.Action<int> OnSessionGoldChanged;

    private IUIManager CurrentUI
    {
        get
        {
            if (UIManager.Instance != null) return UIManager.Instance;
            if (HubUIManager.Instance != null) return HubUIManager.Instance;
            return null;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        if (onDecalFullyCleanedEvent != null)
            onDecalFullyCleanedEvent.OnEventRaised += OnStainCleaned;
    }

    private void OnDisable()
    {
        if (onDecalFullyCleanedEvent != null)
            onDecalFullyCleanedEvent.OnEventRaised -= OnStainCleaned;
    }

    private void Start()
    {
        int totalGold = PlayerPrefs.GetInt("TotalGold", 0);
    }

    /// <summary>
    /// Evaluates requested target strings routing environment transition routines actively.
    /// </summary>
    public void LoadDungeonScene(string sceneName)
    {
        if (isLoading) return;

        sessionGold = 0;
        sessionSalvage = 0;
        currentCleanedCount = 0;
        NotifySessionUI();

        StartCoroutine(LoadSceneRoutine(sceneName, true));
    }

    /// <summary>
    /// Processes extracted temporary session loot pushing to centralized persistent datastores safely.
    /// </summary>
    public void SecureLoot()
    {
        int currentTotalGold = PlayerPrefs.GetInt("TotalGold", 0);
        int currentTotalSalvage = PlayerPrefs.GetInt("TotalSalvage", 0);

        int newTotalGold = currentTotalGold + sessionGold;
        int newTotalSalvage = currentTotalSalvage + sessionSalvage;

        PlayerPrefs.SetInt("TotalGold", newTotalGold);
        PlayerPrefs.SetInt("TotalSalvage", newTotalSalvage);
        PlayerPrefs.Save();

        Debug.Log($"[LevelManager] Loot Saved. Total Gold: {newTotalGold}");

        sessionGold = 0;
        sessionSalvage = 0;
        NotifySessionUI();
    }

    /// <summary>
    /// Validates and captures the current success state writing metadata evaluating run performance passively.
    /// </summary>
    public void SaveLastRunDataToPrefs()
    {
        bool isSuccess = IsMissionSuccess();

        PlayerPrefs.SetInt("HasLastRunData", 1);
        PlayerPrefs.SetInt("LastRunSuccess", isSuccess ? 1 : 0);
        PlayerPrefs.SetInt("LastRunGold", sessionGold);
        PlayerPrefs.SetInt("LastRunSalvage", sessionSalvage);
        PlayerPrefs.Save();

        Debug.Log($"[LevelManager] Fatura kesildi: Basari={isSuccess}, Altin={sessionGold}, Hurda={sessionSalvage}");
    }

    /// <summary>
    /// Short-circuits to central hub immediately explicitly avoiding level transit delays natively.
    /// </summary>
    public void ReturnToHub()
    {
        if (isLoading) return;
        StartCoroutine(LoadSceneRoutine(hubSceneName, false));
    }

    /// <summary>
    /// Helper functionality explicitly triggering loaded specific testing scenes.
    /// </summary>
    public void LoadMission()
    {
        LoadDungeonScene(defaultMissionScene);
    }

    private void OnStainCleaned()
    {
        currentCleanedCount++;
    }

    /// <summary>
    /// Examines linked ActiveMissionManager states routing structural win conditions properly.
    /// </summary>
    public bool IsMissionSuccess()
    {
        if (ActiveMissionManager.Instance != null)
            return ActiveMissionManager.Instance.isMissionComplete;

        return false;
    }

    /// <summary>
    /// Increments tracked local unbanked currencies appropriately alerting connected visual arrays.
    /// </summary>
    public void AddSessionLoot(int gold, int salvage)
    {
        sessionGold += gold;
        sessionSalvage += salvage;
        NotifySessionUI();
    }

    private IEnumerator LoadSceneRoutine(string sceneName, bool useTransitDelay = false)
    {
        isLoading = true;

        if (CurrentUI != null)
        {
            CurrentUI.ShowLoading(true);
        }

        if (useTransitDelay)
        {
            yield return new WaitForSeconds(transitDuration);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone) yield return null;

        if (CurrentUI != null)
        {
            CurrentUI.ShowLoading(false);
            CurrentUI.SetCursorState(true);
        }

        Time.timeScale = 1f;
        isLoading = false;
    }

    private void NotifySessionUI() => OnSessionGoldChanged?.Invoke(sessionGold);

    /// <summary>
    /// Provides simple access to current temporary session unbanked gold cache directly.
    /// </summary>
    public int GetSessionGold() => sessionGold;
}