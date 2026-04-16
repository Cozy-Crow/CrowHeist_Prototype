using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform panel;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    [Header("Animation")]
    [SerializeField] private float bounceDuration = 0.4f;

    private Vector3 originalScale;
    private bool isAnimatingIn = false;
    private bool isAnimatingOut = false;
    private float animTime = 0f;

    private void Awake()
    {
        originalScale = panel.localScale == Vector3.zero ? Vector3.one : panel.localScale;

        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        // StartCoroutine(DisableAfterStart());
        panel.gameObject.SetActive(false);
    }

    // private IEnumerator DisableAfterStart()
    // {
    //     yield return new WaitForEndOfFrame();
    //     yield return new WaitForEndOfFrame();
    //     panel.gameObject.SetActive(false);
    // }

    private void Update()
    {
        if (isAnimatingIn)
        {
            animTime += Time.deltaTime;
            float normalized = animTime / bounceDuration;

            if (normalized >= 1f)
            {
                panel.localScale = originalScale;
                isAnimatingIn = false;
                return;
            }

            panel.localScale = originalScale * BounceScale(normalized);
        }
        else if (isAnimatingOut)
        {
            animTime += Time.deltaTime;
            float normalized = animTime / bounceDuration;

            if (normalized >= 1f)
            {
                panel.localScale = originalScale;
                panel.gameObject.SetActive(false);
                isAnimatingOut = false;
                return;
            }

            panel.localScale = originalScale * Mathf.Lerp(1f, 0f, normalized);
        }
    }

    public void Show()
    {
        Transform t = panel.transform;
        while (t != null)
        {
            Debug.Log(t.gameObject.name + " activeSelf: " + t.gameObject.activeSelf + " activeInHierarchy: " + t.gameObject.activeInHierarchy);
            t = t.parent;
        }

        panel.gameObject.SetActive(true);
        Debug.Log("Is active after set: " + panel.gameObject.activeSelf);
        Debug.Log("Is active in hierarchy: " + panel.gameObject.activeInHierarchy);
        panel.localScale = Vector3.zero;
        animTime = 0f;
        isAnimatingIn = true;
        isAnimatingOut = false;
    }

    public void Hide()
    {
        animTime = 0f;
        isAnimatingOut = true;
        isAnimatingIn = false;
    }

    private float BounceScale(float t)
    {
        if (t < 0.7f)
            return Mathf.Lerp(0f, 1.15f, t / 0.7f);
        else
            return Mathf.Lerp(1.15f, 1f, (t - 0.7f) / 0.3f);
    }
}