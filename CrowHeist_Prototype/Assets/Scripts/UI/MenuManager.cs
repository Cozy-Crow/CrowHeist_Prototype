using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using KinematicCharacterController.Examples;

public class PauseManager : MonoBehaviour
{
    public bool isGamePaused = false; // Variable to track pause state

    public GameObject PauseMenu; // UI element to show/hide Pause Menu
    public GameObject loadSaveSlots; // UI element to show/hide Save Slots

    public Button QuitButton; // Reference to the "Quit Button"
    public Button ResumeButton; // Reference to the "Resume Button"
    public Button RestartButton; // Reference to the "Restart Button"
    public Button SaveButton; // Reference to the "Save Button"
    public Button PlayButton; // Reference to the "Play Button"
    public Button LoadButton; // Reference to the "Load Button"
    public Button BackButton; // Reference to the "Back Button"

    private Controller2Point5D player; // Reference to player for checking held items

    void Start()
    {
        // Find the player in the scene
        player = FindObjectOfType<Controller2Point5D>();

        if (ResumeButton != null)
        {
            ResumeButton.onClick.AddListener(ResumeGame);
        }

        if (QuitButton != null)
        {
            QuitButton.onClick.AddListener(QuitGame);
        }

        if (RestartButton != null)
        {
            RestartButton.onClick.AddListener(RestartGame);
        }

        if (SaveButton != null)
        {
            SaveButton.onClick.AddListener(OnSaveButtonClicked);
        }

        if (PlayButton != null)
        {
            PlayButton.onClick.AddListener(PlayGame);
        }

        if (LoadButton != null)
        {
            LoadButton.onClick.AddListener(OnLoadButtonClicked);
        }

        if (BackButton != null)
        {
            BackButton.onClick.AddListener(GoBack);
        }

        // Update button states on start
        UpdateButtonStates();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Check for Escape key press
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PlayGame()
    {
        MusicManager.Instance.StopMusic();
        SceneManager.LoadSceneAsync(1);
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Freeze time
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(true); // Show pause menu
        }

        // Update button states when menu opens
        UpdateButtonStates();
    }

    public void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Resume time
        if (PauseMenu != null)
        {
            PauseMenu.SetActive(false); // Hide pause menu
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RestartGame()
    {
        // Delete the save file before restarting
        if (SaveLoadSystem.Instance != null)
        {
            SaveLoadSystem.Instance.DeleteSave();
            Debug.Log("Save file deleted on restart");
        }

        isGamePaused = false;
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game is restarting...");
    }

    // Called when save button is clicked
    private void OnSaveButtonClicked()
    {
        // Check if player is holding something
        if (IsPlayerHoldingItem())
        {
            Debug.LogWarning("Cannot save while holding an item!");
            return;
        }

        SaveGame();
        UpdateButtonStates(); // Update button states after saving
    }

    // Called when load button is clicked
    private void OnLoadButtonClicked()
    {
        // Check if save exists before attempting to load
        if (SaveLoadSystem.Instance != null && !SaveLoadSystem.Instance.SaveExists())
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        LoadGame();
    }

    public void SaveGame()
    {
        if (SaveLoadSystem.Instance != null)
        {
            SaveLoadSystem.Instance.SaveGame();
            Debug.Log("Game saved from UI button!");
        }
        else
        {
            Debug.LogWarning("SaveLoadSystem not found!");
        }
    }

    public void LoadGame()
    {
        if (loadSaveSlots != null)
        {
            loadSaveSlots.SetActive(true); // Show Save Slot selection
        }
        else
        {
            // If no save slots UI, load directly
            LoadGameDirect();
        }
    }

    public void LoadGameDirect()
    {
        if (SaveLoadSystem.Instance != null)
        {
            if (!SaveLoadSystem.Instance.SaveExists())
            {
                Debug.LogWarning("No save file to load!");
                return;
            }

            // Resume time before loading 
            Time.timeScale = 1f;
            isGamePaused = false;

            SaveLoadSystem.Instance.LoadGame();
            Debug.Log("Game loaded from UI button!");
        }
        else
        {
            Debug.LogWarning("SaveLoadSystem not found!");
        }
    }

    public void GoBack()
    {
        if (loadSaveSlots != null)
        {
            loadSaveSlots.SetActive(false); // Hide Save Slot selection
        }
    }

    // Updates both Save and Load button states
    private void UpdateButtonStates()
    {
        UpdateSaveButtonState();
        UpdateLoadButtonState();
    }

    // Updates the Save button's interactable state based on whether player is holding an item
    private void UpdateSaveButtonState()
    {
        if (SaveButton != null)
        {
            bool canSave = !IsPlayerHoldingItem();
            SaveButton.interactable = canSave;

            if (!canSave)
            {
                Debug.Log("Save button disabled - player is holding an item");
            }
            else
            {
                Debug.Log("Save button enabled - player is not holding anything");
            }
        }
    }

    // Updates the Load button's interactable state based on save file existence
    private void UpdateLoadButtonState()
    {
        if (LoadButton != null && SaveLoadSystem.Instance != null)
        {
            bool saveExists = SaveLoadSystem.Instance.SaveExists();
            LoadButton.interactable = saveExists;

            if (saveExists)
            {
                Debug.Log("Load button enabled - save file exists");
            }
            else
            {
                Debug.Log("Load button disabled - no save file");
            }
        }
    }

    // Checks if the player is currently holding an item
    private bool IsPlayerHoldingItem()
    {
        if (player == null)
        {
            player = FindObjectOfType<Controller2Point5D>();
        }

        if (player != null)
        {
            return player.heldObject != null;
        }

        return false;
    }
}