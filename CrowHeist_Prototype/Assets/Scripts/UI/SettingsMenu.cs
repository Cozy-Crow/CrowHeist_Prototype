using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI controller for the in-game settings panel.
/// Exposes controls for master audio volume, mouse sensitivity, and left-hand mode.
/// Wire up the serialized fields in the Inspector and call OpenSettings() from a button.
/// </summary>
public class SettingsMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Controls")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Toggle leftHandModeToggle;

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    [SerializeField] private EventReference close;

    private VCA MasterVolumeControl;
    private bool _openedBeforeStart = false;


    private void Start()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);

        if (leftHandModeToggle != null)
            leftHandModeToggle.onValueChanged.AddListener(OnLeftHandModeChanged);

        if (backButton != null)
            backButton.onClick.AddListener(CloseSettings);

        if (settingsPanel != null && !_openedBeforeStart)
            settingsPanel.SetActive(false);

        MasterVolumeControl = FMODUnity.RuntimeManager.GetVCA("vca:/MasterVolumeControl");
    }

    // ──────────────────────────────────────────────
    //  Open / Close
    // ──────────────────────────────────────────────

    public void OpenSettings()
    {
        _openedBeforeStart = true;
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        RefreshUI();
        EventSystem.current?.SetSelectedGameObject(null);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        AudioManager.Instance?.PlayOneShot(close);
    }

    public bool IsOpen => settingsPanel != null && settingsPanel.activeSelf;

    // ──────────────────────────────────────────────
    //  UI Sync
    // ──────────────────────────────────────────────

    /// <summary>
    /// Syncs slider/toggle values to the current SettingsManager state without firing listeners.
    /// </summary>
    private void RefreshUI()
    {
        if (SettingsManager.Instance == null) return;

        if (masterVolumeSlider != null)
            masterVolumeSlider.SetValueWithoutNotify(SettingsManager.Instance.MasterVolume);

        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.SetValueWithoutNotify(SettingsManager.Instance.MouseSensitivity);

        if (leftHandModeToggle != null)
            leftHandModeToggle.SetIsOnWithoutNotify(SettingsManager.Instance.LeftHandMode);
    }

    // ──────────────────────────────────────────────
    //  Listeners
    // ──────────────────────────────────────────────

    private void OnMasterVolumeChanged(float value)
    {
        SettingsManager.Instance?.SetMasterVolume(value);
        MasterVolumeControl.setVolume(value);
    }

    private void OnMouseSensitivityChanged(float value)
    {
        SettingsManager.Instance?.SetMouseSensitivity(value);
    }

    private void OnLeftHandModeChanged(bool value)
    {
        SettingsManager.Instance?.SetLeftHandMode(value);
    }
}
