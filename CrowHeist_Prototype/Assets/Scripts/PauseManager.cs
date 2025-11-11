using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    
    public bool isGamePaused = false; // Variable to track pause state
    public GameObject PauseMenu; // UI element to show/hide
    public Button QuitButton; // Reference to the "Quit Button"
    public Button ResumeButton; // Reference to the "Resume Button"
    public Button RestartButton; // Reference to the "Restart Button"
    public Button SaveButton; // Reference to the "Save Button"

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
}
