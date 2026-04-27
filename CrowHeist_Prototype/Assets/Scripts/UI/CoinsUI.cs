using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private CanvasGroup localCanvasGroup;

    [Header("Settings")]
    [SerializeField] private int _maxCoins = 25;

    [Header("Animation")]
    [SerializeField] private float holdTime = 1.5f;
    [SerializeField] private float animDuration = 0.4f;

    public CanvasGroup CanvasGroup => _canvasGroup;

    private Coroutine uiCoroutine;
    private Vector3 originalScale;
    private bool _initialized = false;

    private void Start()
    {
        localCanvasGroup.alpha = 0f;
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        originalScale = GetComponent<RectTransform>().localScale;
        _coinText.text = $"0/{_maxCoins}";
        _initialized = true;
    }

    public void UpdateCoins(int coins)
    {
        _coinText.text = $"{coins}/{_maxCoins}";

        if (!_initialized) return;

        if (uiCoroutine != null)
            StopCoroutine(uiCoroutine);

        uiCoroutine = StartCoroutine(AnimateUI());
    }

    public void UpdateCoinsSilent(int coins)
    {
        _coinText.text = $"{coins}/{_maxCoins}";
    }

    private IEnumerator AnimateUI()
    {
        RectTransform rect = GetComponent<RectTransform>();
        Vector3 targetScale = rect.localScale; // capture current scale
        localCanvasGroup.alpha = 1f;
        rect.localScale = Vector3.zero;

        // Scale pop in
        float t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / animDuration);
            rect.localScale = targetScale * RubberBandScale(normalized);
            yield return null;
        }
        rect.localScale = targetScale;

        yield return new WaitForSeconds(holdTime);

        // Fade out
        t = 0f;
        while (t < animDuration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / animDuration);
            localCanvasGroup.alpha = Mathf.Lerp(1f, 0f, normalized);
            yield return null;
        }

        localCanvasGroup.alpha = 0f;
        rect.localScale = targetScale;
    }


    private float RubberBandScale(float t)
    {
        if (t < 0.6f)
            return Mathf.Lerp(0f, 1.2f, t / 0.6f);
        else if (t < 0.8f)
            return Mathf.Lerp(1.2f, 0.9f, (t - 0.6f) / 0.2f);
        else
            return Mathf.Lerp(0.9f, 1.0f, (t - 0.8f) / 0.2f);
    }
}