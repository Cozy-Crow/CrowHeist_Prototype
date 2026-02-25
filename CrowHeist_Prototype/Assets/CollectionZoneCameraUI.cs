using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class CollectionZoneCameraUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RawImage cameraDisplay;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform displayContainer;
    [SerializeField] private Image recordingDot;

    [Header("Narrative Item Popup")]
    [SerializeField] private Image narrativePopup;
    [SerializeField] private float narrativePopupDelay = 0.5f;

    [Header("Camera Setup")]
    [SerializeField] private Camera collectionCamera;
    [SerializeField] private CinemachineVirtualCamera collectionVirtualCamera;
    [SerializeField] private RenderTexture renderTexture;

    [Header("Animation Settings")]
    [SerializeField] private float appearDuration = 0.5f;
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Vector2 hiddenScale = new Vector2(0.5f, 0.5f);
    [SerializeField] private Vector2 visibleScale = Vector2.one;

    [Header("Recording Dot Settings")]
    [SerializeField] private float blinkSpeed = 0.3f;

    private Coroutine currentAnimation;
    private Coroutine blinkCoroutine;
    private Coroutine coinFadeCoroutine;
    private int originalCameraPriority;

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        displayContainer.localScale = hiddenScale;

        if (recordingDot != null)
            recordingDot.enabled = false;

        if (narrativePopup != null)
            narrativePopup.enabled = false;

        if (collectionCamera != null && renderTexture != null)
        {
            collectionCamera.targetTexture = renderTexture;
            cameraDisplay.texture = renderTexture;
            collectionCamera.enabled = false;
        }

        if (collectionVirtualCamera != null)
        {
            originalCameraPriority = collectionVirtualCamera.Priority;
            collectionVirtualCamera.Priority = 0;
        }
    }

    public void ShowCollectionZone(bool isNarrativeItem = false, Sprite itemSprite = null)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(AnimateCameraView(isNarrativeItem, itemSprite));
    }

    // Call this from Coins.cs instead of UIManager.Instance.CoinsUI.UpdateCoins()
    public void UpdateCoinCounter(int currentScore)
    {
        if (UIManager.Instance == null || UIManager.Instance.CoinsUI == null) return;

        // Update display text
        UIManager.Instance.CoinsUI.UpdateCoins(currentScore);

        // Cancel any ongoing fade and fade coin UI in
        if (coinFadeCoroutine != null)
            StopCoroutine(coinFadeCoroutine);

        coinFadeCoroutine = StartCoroutine(FadeCoinUI(1f));
    }

    private IEnumerator AnimateCameraView(bool isNarrativeItem, Sprite itemSprite)
    {
        if (collectionCamera != null)
            collectionCamera.enabled = true;

        if (recordingDot != null)
        {
            recordingDot.enabled = true;
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkRecordingDot());
        }

        if (collectionVirtualCamera != null)
            collectionVirtualCamera.Priority = 15;

        displayContainer.localScale = hiddenScale;
        canvasGroup.alpha = 0f;

        // Pop in and fade in
        float elapsed = 0f;
        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / appearDuration;

            displayContainer.localScale = Vector2.Lerp(hiddenScale, visibleScale, EaseOutBack(t));
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        displayContainer.localScale = visibleScale;
        canvasGroup.alpha = 1f;

        // Show narrative popup if applicable
        if (isNarrativeItem && narrativePopup != null)
        {
            yield return new WaitForSeconds(narrativePopupDelay);

            if (itemSprite != null)
                narrativePopup.sprite = itemSprite;

            narrativePopup.enabled = true;
        }

        // Hold
        yield return new WaitForSeconds(displayDuration);

        // Hide narrative popup
        if (narrativePopup != null)
            narrativePopup.enabled = false;

        // Fade out coin counter in sync with this UI fading out
        if (coinFadeCoroutine != null)
            StopCoroutine(coinFadeCoroutine);
        coinFadeCoroutine = StartCoroutine(FadeCoinUI(0f));

        // Fade out this UI
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;

            displayContainer.localScale = Vector2.Lerp(visibleScale, hiddenScale, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        canvasGroup.alpha = 0f;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        if (recordingDot != null)
            recordingDot.enabled = false;

        if (collectionCamera != null)
            collectionCamera.enabled = false;

        if (collectionVirtualCamera != null)
            collectionVirtualCamera.Priority = originalCameraPriority;

        currentAnimation = null;
    }

    private IEnumerator FadeCoinUI(float targetAlpha)
    {
        if (UIManager.Instance == null || UIManager.Instance.CoinsUI == null) yield break;

        CanvasGroup coinCanvasGroup = UIManager.Instance.CoinsUI.CanvasGroup;
        if (coinCanvasGroup == null) yield break;

        // Re-enable the GameObject before fading in
        if (targetAlpha == 1f)
            UIManager.Instance.CoinsUI.gameObject.SetActive(true);

        float startAlpha = coinCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            coinCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        coinCanvasGroup.alpha = targetAlpha;

        // Disable the GameObject after fading out
        if (targetAlpha == 0f)
        {
            coinCanvasGroup.interactable = false;
            coinCanvasGroup.blocksRaycasts = false;
            UIManager.Instance.CoinsUI.gameObject.SetActive(false);
        }

        coinFadeCoroutine = null;
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private IEnumerator BlinkRecordingDot()
    {
        while (true)
        {
            float elapsed = 0f;
            while (elapsed < blinkSpeed / 2f)
            {
                elapsed += Time.deltaTime;
                if (recordingDot != null)
                {
                    Color color = recordingDot.color;
                    color.a = Mathf.Lerp(0.2f, 1f, elapsed / (blinkSpeed / 2f));
                    recordingDot.color = color;
                }
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < blinkSpeed / 2f)
            {
                elapsed += Time.deltaTime;
                if (recordingDot != null)
                {
                    Color color = recordingDot.color;
                    color.a = Mathf.Lerp(1f, 0.2f, elapsed / (blinkSpeed / 2f));
                    recordingDot.color = color;
                }
                yield return null;
            }
        }
    }
}