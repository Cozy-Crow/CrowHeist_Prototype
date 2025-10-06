using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private List<CameraObject> _cameras;

    private static Dictionary<string, CinemachineVirtualCamera> _cameraDictionary = new Dictionary<string, CinemachineVirtualCamera>();
    private static int _score = 0;
    private static int altCoinsScore = 0;
    public static int Score { get => _score; set => _score = value; }
    public static int AltCoinsScore { get => altCoinsScore; set => altCoinsScore = value; }
    [SerializeField] private string endCutsceneScene = "EndCutsceneScene";
    [SerializeField] private string gameSceneName = "IntroScene"; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetGameData();
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var camera in _cameras)
        {
            _cameraDictionary.Add(camera.Name, camera.Camera);
        }
    }

    public static void ChangeCamera(string cameraName)
    {
        switch (cameraName)
        {
            case "Player":
                _cameraDictionary.TryGetValue("Player", out CinemachineVirtualCamera camera1);
                var playerCam = camera1;
                playerCam.enabled = false;
                playerCam.enabled = true;
                break;
            case "Exit":
                _cameraDictionary.TryGetValue("Exit", out CinemachineVirtualCamera camera2);
                var exitCam = camera2;
                exitCam.enabled = false;
                exitCam.enabled = true;
                break;
        }
    }
    
    public void TriggerGameEnd()
    {
        SceneManager.LoadScene(endCutsceneScene);
    }

    private void Update()
    {
        // Check for R key to restart
        if (Input.GetKeyDown(KeyCode.F))
        {
            RestartGame();
        }
        
        if (_score >= 5 || altCoinsScore >= 5)
        {
            TriggerGameEnd();
        }
    }
    
    private void RestartGame()
    {
        ResetGameData();
        SceneManager.LoadScene(gameSceneName);
    }
    
    public static void ResetGameData()
    {
        _score = 0;
        _cameraDictionary.Clear();
    }
}

[System.Serializable]
public class CameraObject
{
    [SerializeField] private string _name;
    [SerializeField] private CinemachineVirtualCamera _camera;

    public string Name { get => _name; }
    public CinemachineVirtualCamera Camera { get => _camera; }
}