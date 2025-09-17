using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string gameSceneName = "GameScene";
    [SerializeField] private bool skipOnInput = true;
    
    void Start()
    {
        // Set up video player
        videoPlayer.loopPointReached += OnVideoFinished;
        
        // Start playing the video
        videoPlayer.Play();
    }
    
    void Update()
    {
        // Allow skipping with any key/click
        if (skipOnInput && Input.anyKeyDown)
        {
            LoadGameScene();
        }
    }
    
    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadGameScene();
    }
    
    private void LoadGameScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}