using UnityEngine;
using FMODUnity;

/// <summary>
/// Persistent singleton that stores and applies player settings:
/// master audio volume, mouse sensitivity, and left-hand mode toggle.
/// Settings are persisted via PlayerPrefs across sessions.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    // PlayerPrefs keys
    private const string KEY_MASTER_VOLUME      = "Settings_MasterVolume";
    private const string KEY_MOUSE_SENSITIVITY  = "Settings_MouseSensitivity";
    private const string KEY_LEFT_HAND_MODE     = "Settings_LeftHandMode";

    // Default values
    private const float DEFAULT_MASTER_VOLUME     = 1f;
    private const float DEFAULT_MOUSE_SENSITIVITY = 1f;
    private const bool  DEFAULT_LEFT_HAND_MODE    = false;

    public float MasterVolume     { get; private set; }
    public float MouseSensitivity { get; private set; }
    public bool  LeftHandMode     { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ──────────────────────────────────────────────
    //  Public Setters
    // ──────────────────────────────────────────────

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(KEY_MASTER_VOLUME, MasterVolume);
        ApplyMasterVolume();
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        MouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 3f);
        PlayerPrefs.SetFloat(KEY_MOUSE_SENSITIVITY, MouseSensitivity);
    }

    public void SetLeftHandMode(bool enabled)
    {
        LeftHandMode = enabled;
        PlayerPrefs.SetInt(KEY_LEFT_HAND_MODE, enabled ? 1 : 0);
    }

    // ──────────────────────────────────────────────
    //  Internal Helpers
    // ──────────────────────────────────────────────

    private void LoadSettings()
    {
        MasterVolume     = PlayerPrefs.GetFloat(KEY_MASTER_VOLUME, DEFAULT_MASTER_VOLUME);
        MouseSensitivity = PlayerPrefs.GetFloat(KEY_MOUSE_SENSITIVITY, DEFAULT_MOUSE_SENSITIVITY);
        LeftHandMode     = PlayerPrefs.GetInt(KEY_LEFT_HAND_MODE, DEFAULT_LEFT_HAND_MODE ? 1 : 0) == 1;
        ApplyMasterVolume();
    }

    private void ApplyMasterVolume()
    {
        FMOD.Studio.Bus masterBus;
        FMOD.RESULT result = RuntimeManager.StudioSystem.getBus("bus:/", out masterBus);
        if (result == FMOD.RESULT.OK)
        {
            masterBus.setVolume(MasterVolume);
        }
        else
        {
            Debug.LogWarning($"SettingsManager: Failed to get FMOD master bus (result: {result}).");
        }
    }
}
