/* ==========================================================
 * Project: Dungeon Janitors: Estate of Chaos
 * Developer: Omer Burak Ozgur
 * GitHub: https://github.com/omerburakozgur
 * ========================================================== */

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

/// <summary>
/// Centralized system for managing background music, UI sounds, and spatial sound effects.
/// Handles scene transitions and audio mixer volume routing.
/// </summary>
public class AudioManager : SingletonManager<AudioManager>
{
    [Header("Data")]
    [Tooltip("Reference to the AudioData ScriptableObject containing audio clips.")]
    public AudioDataSO data;

    [Header("2D Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource uiSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mainMixer;

    [Header("Mixer Groups")]
    [SerializeField] private AudioMixerGroup sfxMixerGroup;
    [SerializeField] private AudioMixerGroup uiMixerGroup;

    public AudioMixerGroup SFXMixerGroup => sfxMixerGroup;

    private const string MIXER_MASTER = "MasterVol";
    private const string MIXER_MUSIC = "MusicVol";
    private const string MIXER_SFX = "SFXVol";
    private const string MIXER_AMBIENT = "AmbientVol";
    private const string MIXER_UI = "UIVol";

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        SettingsMenu.ApplyStartupSettings();

        string currentScene = SceneManager.GetActiveScene().name;
        CheckAndPlaySceneMusic(currentScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckAndPlaySceneMusic(scene.name);
    }

    private void CheckAndPlaySceneMusic(string sceneName)
    {
        if (data == null) return;

        if (sceneName == "HubScene")
        {
            PlayMusic(data.menuMusic);
        }
        else if (sceneName == "MainScene")
        {
            PlayMusic(data.mainSceneMusic);
        }
        else
        {
            Debug.LogWarning($"[AudioManager] Undefined scene name ({sceneName}). Defaulting to menu music.");
            PlayMusic(data.menuMusic);
        }
    }

    public void SetMasterVolume(float value) => SetMixerVolume(MIXER_MASTER, value);
    public void SetMusicVolume(float value) => SetMixerVolume(MIXER_MUSIC, value);
    public void SetSFXVolume(float value) => SetMixerVolume(MIXER_SFX, value);
    public void SetAmbientVolume(float value) => SetMixerVolume(MIXER_AMBIENT, value);
    public void SetUIVolume(float value) => SetMixerVolume(MIXER_UI, value);

    private void SetMixerVolume(string paramName, float sliderValue)
    {
        if (mainMixer == null) return;

        // Convert linear 0-1 slider value to logarithmic dB scale
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        mainMixer.SetFloat(paramName, dbValue);
    }

    /// <summary>
    /// Plays the configured UI click sound.
    /// </summary>
    public void PlayClickSound()
    {
        if (data != null && data.uiClick != null)
        {
            PlayUI(data.uiClick);
        }
    }

    /// <summary>
    /// Starts playing background music. If the requested clip is already playing, the request is ignored.
    /// </summary>
    /// <param name="clip">The music track to play.</param>
    /// <param name="volume">Playback volume.</param>
    public void PlayMusic(AudioClip clip, float volume = 0.4f)
    {
        if (musicSource == null || clip == null)
        {
            Debug.LogWarning("[AudioManager] MusicSource or AudioClip is missing.");
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.volume = volume;
        musicSource.Play();
    }

    /// <summary>
    /// Plays a UI sound effect as a one-shot.
    /// </summary>
    /// <param name="clip">The sound clip to play.</param>
    /// <param name="volume">Playback volume.</param>
    public void PlayUI(AudioClip clip, float volume = 1f)
    {
        if (uiSource == null || clip == null) return;
        uiSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// Plays a spatialized sound effect at a specific location.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    /// <param name="position">World position for the sound.</param>
    /// <param name="volume">Playback volume.</param>
    /// <param name="pitchRandom">Range of random pitch variation.</param>
    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1f, float pitchRandom = 0.1f)
    {
        if (clip == null) return;

        GameObject go = new GameObject("SFX_" + clip.name);
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.pitch = 1f + Random.Range(-pitchRandom, pitchRandom);
        source.spatialBlend = 0.6f;
        source.minDistance = 2f;
        source.maxDistance = 20f;
        source.rolloffMode = AudioRolloffMode.Linear;

        source.Play();
        Destroy(go, clip.length + 0.1f);
    }

    /// <summary>
    /// Alias for PlayClickSound.
    /// </summary>
    public void PlayUIClick()
    {
        if (uiSource != null && data != null && data.uiClick != null)
        {
            uiSource.PlayOneShot(data.uiClick);
        }
    }

    /// <summary>
    /// Selects a random clip from an array and plays it at the specified location.
    /// </summary>
    /// <param name="clips">Array of possible audio clips.</param>
    /// <param name="position">World position for the sound.</param>
    /// <param name="volume">Playback volume.</param>
    /// <param name="pitchRandom">Range of random pitch variation.</param>
    public void PlayRandomSFX(AudioClip[] clips, Vector3 position, float volume = 1f, float pitchRandom = 0.1f)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        AudioClip selectedClip = clips[index];
        if (selectedClip == null) return;

        GameObject go = new GameObject("SFX_" + selectedClip.name);
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = selectedClip;
        source.volume = volume;
        source.pitch = 1f + Random.Range(-pitchRandom, pitchRandom);
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.spatialBlend = 0.6f;
        source.minDistance = 2f;
        source.maxDistance = 20f;

        source.Play();
        Destroy(go, selectedClip.length + 0.1f);
    }
}