using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    [SerializeField] private GameObject Blocker;
    [SerializeField] private CanvasGroup FadeCanvasGroup; // Assign in inspector

    void Update()
    {
        if(GameManager.Score >= 5 || GameManager.AltCoinsScore >= 5)
        {
            Blocker.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (GameManager.Score >= 5 || GameManager.AltCoinsScore >= 5))
        {
            StartCoroutine(GameEndDelay());
        }
    }

    private IEnumerator GameEndDelay()
    {
        yield return StartCoroutine(FadeToBlack(3f));
        GameManager.Instance.TriggerGameEnd();
    }

    private IEnumerator FadeToBlack(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            FadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        FadeCanvasGroup.alpha = 1f;
    }
}