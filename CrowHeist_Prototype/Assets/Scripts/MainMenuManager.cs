using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button exitButton; // Reference to the "Exit Button"
    public Button startButton; // Reference to the "Start Button"
    public Button creditsButton; // Reference to the "Credits Button"

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.Log("Start Button is not assigned!");
        }

        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.Log("Exit Button is not assigned!");
        }

        if (creditsButton != null)
        {
            creditsButton.onClick.AddListener(Credits);
        }
        else
        {
            Debug.Log("Credits Button is not assigned!");
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void Credits()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
}
