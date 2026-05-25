/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages settings menu interactions including audio, gameplay, and graphics options.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private TextMeshProUGUI masterText;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI sfxText;
    [SerializeField] private Slider ambientSlider;
    [SerializeField] private TextMeshProUGUI ambientText;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private TextMeshProUGUI uiText;

    [Header("Gameplay Settings")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI sensitivityText;

    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullscreenDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown fpsLimitDropdown;

    private Resolution[] resolutions;

    private void Start()
    {
        SetupAudio();
        SetupGameplay();
        SetupGraphics();
    }

    private void SetupAudio()
    {
        SetupVolume(masterSlider, masterText, PlayerPrefsKeys.MIXER_MASTER, AudioManager.Instance.SetMasterVolume);
        SetupVolume(musicSlider, musicText, PlayerPrefsKeys.MIXER_MUSIC, AudioManager.Instance.SetMusicVolume);
        SetupVolume(sfxSlider, sfxText, PlayerPrefsKeys.MIXER_SFX, AudioManager.Instance.SetSFXVolume);
        SetupVolume(ambientSlider, ambientText, PlayerPrefsKeys.MIXER_AMBIENT, AudioManager.Instance.SetAmbientVolume);
        SetupVolume(uiSlider, uiText, PlayerPrefsKeys.MIXER_UI, AudioManager.Instance.SetUIVolume);
    }

    private void SetupVolume(Slider slider, TextMeshProUGUI label, string key, System.Action<float> action)
    {
        if (slider == null) return;
        float val = PlayerPrefs.GetFloat(key, 0.75f);
        slider.value = val;
        UpdateText(label, Mathf.RoundToInt(val * 100).ToString());
        if (action != null) action(val);

        slider.onValueChanged.AddListener((v) =>
        {
            if (action != null) action(v);
            UpdateText(label, Mathf.RoundToInt(v * 100).ToString());
            PlayerPrefs.SetFloat(key, v);
            PlayerPrefs.Save();
        });
    }

    private void SetupGameplay()
    {
        if (sensitivitySlider == null) return;

        float sens = PlayerPrefs.GetFloat(PlayerPrefsKeys.SENSITIVITY, 1.0f);
        sensitivitySlider.value = sens;

        UpdateText(sensitivityText, sens.ToString("F2"));

        sensitivitySlider.onValueChanged.AddListener((v) =>
        {
            float roundedValue = Mathf.Round(v * 100f) / 100f;
            UpdateText(sensitivityText, roundedValue.ToString("F2"));

            PlayerPrefs.SetFloat(PlayerPrefsKeys.SENSITIVITY, roundedValue);
            PlayerPrefs.Save();

            var playerAim = FindFirstObjectByType<PlayerAim>();

            if (playerAim != null)
            {
                playerAim.UpdateSensitivity(roundedValue);
            }
        });
    }

    private void SetupGraphics()
    {
        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentResolutionIndex = 0;

            HashSet<string> uniqueRes = new HashSet<string>();
            List<Resolution> filteredRes = new List<Resolution>();

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                if (!uniqueRes.Contains(option))
                {
                    uniqueRes.Add(option);
                    filteredRes.Add(resolutions[i]);
                    options.Add(option);

                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    {
                        currentResolutionIndex = filteredRes.Count - 1;
                    }
                }
            }
            resolutions = filteredRes.ToArray();
            resolutionDropdown.AddOptions(options);

            int savedWidth = PlayerPrefs.GetInt(PlayerPrefsKeys.RES_WIDTH, Screen.currentResolution.width);
            int savedHeight = PlayerPrefs.GetInt(PlayerPrefsKeys.RES_HEIGHT, Screen.currentResolution.height);

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == savedWidth && resolutions[i].height == savedHeight)
                {
                    currentResolutionIndex = i;
                    break;
                }
            }

            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            resolutionDropdown.onValueChanged.AddListener(SetResolution);
        }

        if (fullscreenDropdown != null)
        {
            fullscreenDropdown.ClearOptions();
            fullscreenDropdown.AddOptions(new List<string> { "Exclusive Fullscreen", "Borderless Window", "Windowed" });

            int savedMode = PlayerPrefs.GetInt(PlayerPrefsKeys.FULLSCREEN, 0);
            fullscreenDropdown.value = savedMode;
            fullscreenDropdown.RefreshShownValue();
            fullscreenDropdown.onValueChanged.AddListener(SetFullscreen);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string>(QualitySettings.names));

            int savedQuality = PlayerPrefs.GetInt(PlayerPrefsKeys.QUALITY, QualitySettings.GetQualityLevel());
            qualityDropdown.value = savedQuality;
            qualityDropdown.RefreshShownValue();
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        }

        if (fpsLimitDropdown != null)
        {
            fpsLimitDropdown.ClearOptions();
            fpsLimitDropdown.AddOptions(new List<string> { "30 FPS", "60 FPS", "120 FPS", "144 FPS", "165 FPS", "240 FPS", "Unlimited" });

            int savedFpsIndex = PlayerPrefs.GetInt(PlayerPrefsKeys.FPS, 4);
            fpsLimitDropdown.value = savedFpsIndex;
            fpsLimitDropdown.RefreshShownValue();
            SetFpsLimit(savedFpsIndex);
            fpsLimitDropdown.onValueChanged.AddListener(SetFpsLimit);
        }
    }

    /// <summary>
    /// Updates game resolution settings.
    /// </summary>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);

        PlayerPrefs.SetInt(PlayerPrefsKeys.RES_WIDTH, resolution.width);
        PlayerPrefs.SetInt(PlayerPrefsKeys.RES_HEIGHT, resolution.height);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the fullscreen mode.
    /// </summary>
    public void SetFullscreen(int index)
    {
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        switch (index)
        {
            case 0: mode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: mode = FullScreenMode.FullScreenWindow; break;
            case 2: mode = FullScreenMode.Windowed; break;
        }

        Screen.fullScreenMode = mode;
        PlayerPrefs.SetInt(PlayerPrefsKeys.FULLSCREEN, index);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Adjusts graphics quality settings.
    /// </summary>
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt(PlayerPrefsKeys.QUALITY, index);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Sets the frame rate limit.
    /// </summary>
    public void SetFpsLimit(int index)
    {
        int targetFps = -1;
        switch (index)
        {
            case 0: targetFps = 30; break;
            case 1: targetFps = 60; break;
            case 2: targetFps = 120; break;
            case 3: targetFps = 144; break;
            case 4: targetFps = 165; break;
            case 5: targetFps = 240; break;
            case 6: targetFps = -1; break;
        }

        Application.targetFrameRate = targetFps;
        PlayerPrefs.SetInt(PlayerPrefsKeys.FPS, index);
        PlayerPrefs.Save();
    }

    private void UpdateText(TextMeshProUGUI label, string text)
    {
        if (label != null) label.text = text;
    }

    /// <summary>
    /// Applies saved settings on application startup.
    /// </summary>
    public static void ApplyStartupSettings()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MIXER_MASTER, 0.75f));
            AudioManager.Instance.SetMusicVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MIXER_MUSIC, 0.75f));
            AudioManager.Instance.SetSFXVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MIXER_SFX, 0.75f));
            AudioManager.Instance.SetAmbientVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MIXER_AMBIENT, 0.75f));
            AudioManager.Instance.SetUIVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MIXER_UI, 0.75f));
        }

        int savedWidth = PlayerPrefs.GetInt(PlayerPrefsKeys.RES_WIDTH, Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt(PlayerPrefsKeys.RES_HEIGHT, Screen.currentResolution.height);
        int savedMode = PlayerPrefs.GetInt(PlayerPrefsKeys.FULLSCREEN, 0);

        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        if (savedMode == 1) mode = FullScreenMode.FullScreenWindow;
        else if (savedMode == 2) mode = FullScreenMode.Windowed;

        Screen.SetResolution(savedWidth, savedHeight, mode);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt(PlayerPrefsKeys.QUALITY, QualitySettings.GetQualityLevel()));

        int fpsIndex = PlayerPrefs.GetInt(PlayerPrefsKeys.FPS, 4);
        int targetFps = -1;
        switch (fpsIndex)
        {
            case 0: targetFps = 30; break;
            case 1: targetFps = 60; break;
            case 2: targetFps = 120; break;
            case 3: targetFps = 144; break;
            case 4: targetFps = 165; break;
            case 5: targetFps = 240; break;
            case 6: targetFps = -1; break;
        }
        Application.targetFrameRate = targetFps;
    }
}