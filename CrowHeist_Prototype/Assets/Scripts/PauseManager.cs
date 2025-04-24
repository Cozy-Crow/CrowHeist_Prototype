using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    
    public bool isGamePaused = false; // Variable to track pause state
    public GameObject pauseMenu; // UI element to show/hide
    public Button quitButton; // Reference to the "Quit Button"
    public Button resumeButton; // Reference to the "Resume Button"
    public Button restartButton; // Reference to the "Restart Button"

    void Start() 
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.Log("Resume Button is not assigned!");
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        else 
        {
            Debug.Log("Quit Button is not assigned!");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.Log("Restart Button is not assigned!");
        }
    }

    void Update()
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

            Debug.Log("Escape Pressed");
        }
    }

    void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f; // Freeze time
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true); // Show pause menu
        }
    }

    void ResumeGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f; // Resume time
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false); // Hide pause menu
        }
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game is restarting...");
        isGamePaused = false;
        Time.timeScale = 1f; // Resume Time
    }
}
