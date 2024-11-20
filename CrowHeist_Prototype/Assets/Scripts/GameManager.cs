using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private List<CameraObject> _cameras;

    private static Dictionary<string, CinemachineVirtualCamera> _cameraDictionary = new Dictionary<string, CinemachineVirtualCamera>();
    private static int _score = 0;
    public static int Score { get => _score;   set => _score = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }else
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
}

[System.Serializable]
public class CameraObject
{
    [SerializeField] private string _name;
    [SerializeField] private CinemachineVirtualCamera _camera;

    public string Name { get => _name; }
    public CinemachineVirtualCamera Camera { get => _camera; }
}
