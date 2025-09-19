using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class EndCutsceneManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string gameSceneName = "GameScene"; // or whatever your main game scene is called
    [SerializeField] private bool skipOnInput = true;
    [SerializeField] private bool restartFromIntro = false; // Set to true if you want to go back to intro scene
    [SerializeField] private string introSceneName = "IntroScene";
    
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
            RestartGame();
        }
    }
    
    private void OnVideoFinished(VideoPlayer vp)
    {
        RestartGame();
    }
    
    private void RestartGame()
    {
        // Reset any static/persistent data
        ResetGameData();
        
        // Choose where to restart
        if (restartFromIntro)
        {
            SceneManager.LoadScene(introSceneName);
        }
        else
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
    
    private void ResetGameData()
    {
        // Reset your game's static data here
        // For example, if you have a GameManager with static score:
        if (GameManager.Instance != null)
        {
            GameManager.Score = 0;
            // Reset any other game state variables
        }
        
        // You might also want to reset other managers
        // UIManager, AudioManager, etc.
    }
}