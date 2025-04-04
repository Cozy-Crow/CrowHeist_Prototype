using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    
    public bool isGamePaused = false; // Variable to track pause state
    public GameObject pauseMenu; // UI element to show/hide

    // Update is called once per frame
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
}
