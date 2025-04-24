using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{

    public bool isGameWon = false; // Variable to track win state
    public GameObject WinMenu; // UI element to show/hide
    public Button quitButton; // Reference to the "Quit Button"
    public Button restartButton; // Reference to the "Restart Button"

    void start()
    {
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(quitGame);
        }
        else
        {
            Debug.Log("Quit Button is not assigned!");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(restartGame);
        }
        else
        {
            Debug.Log("Restart Button is not assigned!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            WonGame();
        }
    }

    void WonGame()
    {
        isGameWon = true;
        Time.timeScale = 0f; // Pause Time
        if (WinMenu != null)
        {
            WinMenu.SetActive(true); // Show win menu
        }
    }

    void quitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void restartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Debug.Log("Game is restarting...");
        isGameWon = false;
        Time.timeScale = 1f; // Resume Time
    }
}