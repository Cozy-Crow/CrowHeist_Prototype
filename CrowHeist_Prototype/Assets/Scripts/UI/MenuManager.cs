using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KinematicCharacterController.Examples;

public class PauseManager : MonoBehaviour
{
    public bool isGamePaused = false;

    [Header("Panels")]
    public GameObject PauseMenu;
    public GameObject RestartConfirmPanel;

    [Header("Save/Load Slot Panel")]
    [SerializeField] private SaveSlotPanel saveSlotPanel;

    [Header("Settings")]
    [SerializeField] private SettingsMenu settingsMenu;

    [Header("Buttons")]
    public Button QuitButton;
    public Button ResumeButton;
    public Button RestartButton;
    public Button SaveButton;
    public Button LoadButton;
    public Button BackButton;
    public Button PlayButton;
    public Button SettingsButton;
    public Button ConfirmRestartYesButton;
    public Button ConfirmRestartNoButton;

    private Controller2Point5D player;

    void Start()
    {
        player = FindObjectOfType<Controller2Point5D>();

        // Fall back to finding buttons by name if serialized references are null
        if (ResumeButton  == null) ResumeButton  = FindButtonInChildren("ResumeButton");
        if (QuitButton    == null) QuitButton    = FindButtonInChildren("QuitButton");
        if (RestartButton == null) RestartButton = FindButtonInChildren("RestartButton");
        if (SaveButton    == null) SaveButton    = FindButtonInChildren("SaveButton");
        if (LoadButton    == null) LoadButton    = FindButtonInChildren("LoadButton");
        if (BackButton    == null) BackButton    = FindButtonInChildren("BackButton");
        if (PlayButton    == null) PlayButton    = FindButtonInChildren("PlayButton");
        if (SettingsButton == null) SettingsButton = FindButtonInChildren("SettingsButton");
        if (ConfirmRestartYesButton == null) ConfirmRestartYesButton = FindButtonInChildren("ConfirmYesButton");
        if (ConfirmRestartNoButton  == null) ConfirmRestartNoButton  = FindButtonInChildren("ConfirmNoButton");

        if (RestartConfirmPanel == null)
        {
            foreach (Transform t in GetComponentsInChildren<Transform>(true))
            {
                if (t.gameObject.name == "RestartConfirmPanel")
                {
                    RestartConfirmPanel = t.gameObject;
                    break;
                }
            }
        }

        // Auto-find SaveSlotPanel and SettingsMenu if not set in Inspector
        if (saveSlotPanel == null)
            saveSlotPanel = GetComponentInChildren<SaveSlotPanel>(true);

        if (settingsMenu == null)
            settingsMenu = GetComponentInChildren<SettingsMenu>(true);

        // Wire listeners
        if (ResumeButton  != null) ResumeButton.onClick.AddListener(ResumeGame);
        if (QuitButton    != null) QuitButton.onClick.AddListener(QuitGame);
        if (RestartButton != null) RestartButton.onClick.AddListener(ShowRestartConfirm);
        if (PlayButton    != null) PlayButton.onClick.AddListener(PlayGame);
        if (SaveButton    != null) SaveButton.onClick.AddListener(OnSaveButtonClicked);
        if (LoadButton    != null) LoadButton.onClick.AddListener(OnLoadButtonClicked);
        if (BackButton    != null) BackButton.onClick.AddListener(GoBack);
        if (SettingsButton != null) SettingsButton.onClick.AddListener(OpenSettings);
        if (ConfirmRestartYesButton != null) ConfirmRestartYesButton.onClick.AddListener(RestartGame);
        if (ConfirmRestartNoButton  != null) ConfirmRestartNoButton.onClick.AddListener(CancelRestart);

        // Wire slot panel callbacks
        if (saveSlotPanel != null)
        {
            saveSlotPanel.OnBack          += OnSlotPanelBack;
            saveSlotPanel.OnSlotSelected  += OnSlotSelected;
        }

        if (RestartConfirmPanel != null)
            RestartConfirmPanel.SetActive(false);

        UpdateButtonStates();
    }

    // ──────────────────────────────────────────────
    //  Update
    // ──────────────────────────────────────────────

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If a sub-panel is open, close it first instead of toggling the whole menu
            if (saveSlotPanel != null && saveSlotPanel.IsOpen)
            {
                saveSlotPanel.Close();
                return;
            }
            if (settingsMenu != null && settingsMenu.IsOpen)
            {
                settingsMenu.CloseSettings();
                return;
            }
            if (RestartConfirmPanel != null && RestartConfirmPanel.activeSelf)
            {
                CancelRestart();
                return;
            }

            if (isGamePaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    // ──────────────────────────────────────────────
    //  Pause / Resume
    // ──────────────────────────────────────────────

    public void PauseGame()
    {
        // Close narrative menu if it is open so both can't be visible simultaneously
        if (NarrativeMenu.Instance != null)
            NarrativeMenu.Instance.ForceClose();

        isGamePaused = true;
        Time.timeScale = 0f;

        if (PauseMenu != null)
            PauseMenu.SetActive(true);

        UpdateButtonStates();
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;

        if (PauseMenu != null)
            PauseMenu.SetActive(false);

        // Also close any sub-panels that may still be open
        if (saveSlotPanel != null) saveSlotPanel.Close();
        if (settingsMenu  != null) settingsMenu.CloseSettings();
        if (RestartConfirmPanel != null) RestartConfirmPanel.SetActive(false);
    }

    // ──────────────────────────────────────────────
    //  Scene Transitions
    // ──────────────────────────────────────────────

    public void PlayGame()
    {
        MusicManager.Instance.StopMusic();
        SceneManager.LoadSceneAsync(1);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ──────────────────────────────────────────────
    //  Restart
    // ──────────────────────────────────────────────

    public void RestartGame()
    {
        if (SaveLoadSystem.Instance != null)
        {
            SaveLoadSystem.Instance.DeleteAllSaveData();
            Debug.Log("All save files deleted on restart.");
        }
        if (PickupRegistry.Instance != null)
            PickupRegistry.Instance.ResetAllStates();

        isGamePaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game is restarting...");
    }

    public void ShowRestartConfirm()
    {
        if (RestartConfirmPanel != null)
            RestartConfirmPanel.SetActive(true);
    }

    public void CancelRestart()
    {
        if (RestartConfirmPanel != null)
            RestartConfirmPanel.SetActive(false);
    }

    // ──────────────────────────────────────────────
    //  Save / Load (slot-based)
    // ──────────────────────────────────────────────

    private void OnSaveButtonClicked()
    {
        Debug.Log("Save button pressed");
        if (IsPlayerHoldingItem())
        {
            Debug.LogWarning("Cannot save while holding an item!");
            return;
        }
        OpenSaveSlotPanel(SaveSlotPanel.Mode.Save);
    }

    private void OnLoadButtonClicked()
    {
        Debug.Log("Load button pressed");
        OpenSaveSlotPanel(SaveSlotPanel.Mode.Load);
    }

    private void OpenSaveSlotPanel(SaveSlotPanel.Mode mode)
    {
        if (saveSlotPanel == null)
        {
            // Fallback: save/load current slot directly if no slot panel is set up
            if (mode == SaveSlotPanel.Mode.Save)
                SaveLoadSystem.Instance?.SaveGame();
            else
                LoadGameDirect(SaveLoadSystem.Instance != null ? SaveLoadSystem.Instance.CurrentSlot : 1);
            return;
        }

        if (mode == SaveSlotPanel.Mode.Save)
            saveSlotPanel.OpenForSave();
        else
            saveSlotPanel.OpenForLoad();
    }

    private void OnSlotSelected(int slot)
    {
        UpdateButtonStates();
    }

    private void OnSlotPanelBack()
    {
        // Nothing extra needed; slot panel closes itself
    }

    public void LoadGameDirect(int slot = 1)
    {
        if (SaveLoadSystem.Instance == null) return;
        if (!SaveLoadSystem.Instance.SaveExists(slot))
        {
            Debug.LogWarning($"No save file found for slot {slot}!");
            return;
        }

        Time.timeScale = 1f;
        isGamePaused = false;
        SaveLoadSystem.Instance.LoadGame(slot);
        Debug.Log($"Game loaded from slot {slot} via UI.");
    }

    // Legacy no-arg version kept for any external callers
    public void LoadGameDirect() => LoadGameDirect(
        SaveLoadSystem.Instance != null ? SaveLoadSystem.Instance.CurrentSlot : 1);

    public void GoBack()
    {
        if (saveSlotPanel != null && saveSlotPanel.IsOpen)
            saveSlotPanel.Close();
    }

    // ──────────────────────────────────────────────
    //  Settings
    // ──────────────────────────────────────────────

    public void OpenSettings()
    {
        if (settingsMenu != null)
            settingsMenu.OpenSettings();
    }

    // ──────────────────────────────────────────────
    //  Button State Helpers
    // ──────────────────────────────────────────────

    private void UpdateButtonStates()
    {
        if (SaveButton != null)
            SaveButton.interactable = !IsPlayerHoldingItem();

        if (LoadButton != null && SaveLoadSystem.Instance != null)
        {
            // Enable load button if at least one slot has a save
            bool anySlotExists = false;
            for (int i = 1; i <= SaveLoadSystem.SlotCount; i++)
            {
                if (SaveLoadSystem.Instance.SaveExists(i))
                {
                    anySlotExists = true;
                    break;
                }
            }
            LoadButton.interactable = anySlotExists;
        }
    }

    private bool IsPlayerHoldingItem()
    {
        if (player == null)
            player = FindObjectOfType<Controller2Point5D>();
        return player != null && player.heldObject != null;
    }

    private Button FindButtonInChildren(string buttonName)
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn.gameObject.name == buttonName)
                return btn;
        }
        return null;
    }
}
