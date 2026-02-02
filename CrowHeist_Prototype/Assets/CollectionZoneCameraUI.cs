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
    private int originalCameraPriority;

    private void Awake()
    {
        // Start hidden
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        
        displayContainer.localScale = hiddenScale;

        if (recordingDot != null)
        {
            recordingDot.enabled = false;
        }

        // Hide narrative popup initially
        if (narrativePopup != null)
        {
            narrativePopup.enabled = false;
        }
        
        // Set up render texture
        if (collectionCamera != null && renderTexture != null)
        {
            collectionCamera.targetTexture = renderTexture;
            cameraDisplay.texture = renderTexture;
            collectionCamera.enabled = false; // Start disabled
        }

        // Store original priority
        if (collectionVirtualCamera != null)
        {
            originalCameraPriority = collectionVirtualCamera.Priority;
            collectionVirtualCamera.Priority = 0; // Start disabled
        }
    }

    // Show the collection zone camera view
    public void ShowCollectionZone(bool isNarrativeItem = false, Sprite itemSprite = null)
    {
        // Stop any existing animation
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(AnimateCameraView(isNarrativeItem, itemSprite));
    }

    private IEnumerator AnimateCameraView(bool isNarrativeItem, Sprite itemSprite)
    {
        // Enable camera
        if (collectionCamera != null)
            collectionCamera.enabled = true;

        // Start blinking recording dot
        if (recordingDot != null)
        {
            recordingDot.enabled = true;
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkRecordingDot());
        }

        // Optionally activate virtual camera for better framing
        if (collectionVirtualCamera != null)
            collectionVirtualCamera.Priority = 15; // Higher than player cam

        // Reset
        displayContainer.localScale = hiddenScale;
        canvasGroup.alpha = 0f;

        // Pop in and fade in
        float elapsed = 0f;
        while (elapsed < appearDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / appearDuration;
            
            // Ease out back for a nice pop effect
            float easeT = EaseOutBack(t);
            displayContainer.localScale = Vector2.Lerp(hiddenScale, visibleScale, easeT);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
            
            yield return null;
        }

        displayContainer.localScale = visibleScale;
        canvasGroup.alpha = 1f;

        // Show narrative popup if this is a narrative item
        if (isNarrativeItem && narrativePopup != null)
        {
            yield return new WaitForSeconds(narrativePopupDelay);
            
            // Set the sprite if provided
            if (itemSprite != null)
            {
                narrativePopup.sprite = itemSprite;
            }
            
            narrativePopup.enabled = true;
        }

        // Hold for display duration
        yield return new WaitForSeconds(displayDuration);

        // Hide narrative popup before fading out
        if (narrativePopup != null)
        {
            narrativePopup.enabled = false;
        }

        // Fade out
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
        
        // Stop blinking and hide recording dot
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
        if (recordingDot != null)
            recordingDot.enabled = false;
        
        // Disable camera
        if (collectionCamera != null)
            collectionCamera.enabled = false;

        // Reset virtual camera priority
        if (collectionVirtualCamera != null)
            collectionVirtualCamera.Priority = originalCameraPriority;

        currentAnimation = null;
    }

    // Ease function for smooth animation
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
            // Fade in
            float elapsed = 0f;
            while (elapsed < blinkSpeed / 2f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0.2f, 1f, elapsed / (blinkSpeed / 2f));
                
                if (recordingDot != null)
                {
                    Color color = recordingDot.color;
                    color.a = alpha;
                    recordingDot.color = color;
                }
                
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            while (elapsed < blinkSpeed / 2f)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0.2f, elapsed / (blinkSpeed / 2f));
                
                if (recordingDot != null)
                {
                    Color color = recordingDot.color;
                    color.a = alpha;
                    recordingDot.color = color;
                }
                
                yield return null;
            }
        }
    }
}