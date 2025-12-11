using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        if (ResumeButton != null)
        {
            ResumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.Log("Resume Button is not assigned!");
        }

        if (QuitButton != null)
        {
            QuitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.Log("Quit Button is not assigned!");
        }

        if (RestartButton != null)
        {
            RestartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.Log("Restart Button is not assigned!");
        }

        if (SaveButton != null)
        {
            SaveButton.onClick.AddListener(SaveGame);
        }
        else
        {
            Debug.Log("Save Button is not assigned!");
        }

        if (PlayButton != null)
        {
            PlayButton.onClick.AddListener(PlayGame);
        }

        if (LoadButton != null)
        {
            LoadButton.onClick.AddListener(LoadGame);
        }

        if (BackButton != null)
        {
            BackButton.onClick.AddListener(GoBack);
        }
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
        isGamePaused = false;
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game is restarting...");
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
            // Resume time before loading (in case we're paused)
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
}