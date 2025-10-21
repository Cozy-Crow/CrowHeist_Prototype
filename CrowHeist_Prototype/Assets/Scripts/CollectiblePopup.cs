using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CollectiblePopup : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Animation Settings")]
    [SerializeField] private float _floatSpeed = 50f;
    [SerializeField] private float _fadeSpeed = 1f;
    [SerializeField] private float _lifetime = 1.5f;
    [SerializeField] private Vector2 _randomOffset = new Vector2(30f, 30f);

    private RectTransform _rectTransform;
    private Vector2 _velocity;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        if (_text == null)
            _text = GetComponentInChildren<TextMeshProUGUI>();

        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Initialize(Vector3 screenPosition, int pointValue)
    {
        // Set position with random offset
        Vector2 randomizedPos = new Vector2(
            screenPosition.x + Random.Range(-_randomOffset.x, _randomOffset.x),
            screenPosition.y + Random.Range(-_randomOffset.y, _randomOffset.y)
        );
        _rectTransform.position = randomizedPos;

        // Set text
        _text.text = $"+{pointValue}";

        // Set initial velocity
        _velocity = new Vector2(
            Random.Range(-20f, 20f),
            _floatSpeed
        );

        // Start animation
        StartCoroutine(AnimatePopup());
    }

    private IEnumerator AnimatePopup()
    {
        float elapsed = 0f;

        while (elapsed < _lifetime)
        {
            elapsed += Time.deltaTime;

            // Move upward
            _rectTransform.position += (Vector3)_velocity * Time.deltaTime;

            // Fade out in the last portion of lifetime
            if (elapsed > _lifetime * 0.5f)
            {
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, (elapsed - _lifetime * 0.5f) / (_lifetime * 0.5f));
            }

            // Slow down velocity
            _velocity *= 0.98f;

            yield return null;
        }

        Destroy(gameObject);
    }
}
