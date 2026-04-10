using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PreloadScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float minimumHoldTime = 1f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private string nextScene = "Main"; // your main scene name
    [SerializeField] private bool isIntro = true;

    private void Start()
    {
        canvasGroup.alpha = 1f;
        StartCoroutine(LoadMainScene());
        if (isIntro)
        {
            nextScene = "IntroScene";
        }
        else
        {
            nextScene = "EndingScene";
        }
    }

    private IEnumerator LoadMainScene()
    {
        // Start loading the main scene in the background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextScene);
        asyncLoad.allowSceneActivation = false; // don't switch yet

        // Hold for minimum time while scene loads in background
        float elapsed = 0f;
        while (elapsed < minimumHoldTime || !asyncLoad.isDone)
        {
            elapsed += Time.deltaTime;

            // asyncLoad.progress caps at 0.9 until allowSceneActivation is true
            if (elapsed >= minimumHoldTime && asyncLoad.progress >= 0.9f)
                break;

            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeDuration);
            yield return null;
        }

        // Switch to the loaded scene
        asyncLoad.allowSceneActivation = true;
    }
}