using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using TMPro;

public class Cascaron : MonoBehaviour
{
    // Cascaron Puzzle implemented by Mark D. 2/6/26
    // This is the only script needed for the puzzle and is on the cascaron prefab
    // just drag in however many cascarones you want in the level
    // the last one to be destroyed will drop a coin

    public float breakSpeed = 5f;
    public GameObject confettiEffect;
    public GameObject smokeEffect;
    public Color baseColor;
    public GameObject coinPrefab;

    [SerializeField] private EventReference breakSFX;
    [SerializeField] private GameObject cascaronUI;

    [Header("UI Animation")]
    [SerializeField] private float holdTime = 2f;
    [SerializeField] private float animDuration = 0.4f;

    private static int totalCascarons = 0;
    private static int collectedCascarons = 0;
    private const int MAX_CASCARONS = 4;
    private static Coroutine uiCoroutine;
    private static MonoBehaviour coroutineRunner;

    private Vector3 zero = new Vector3(0, 0, 0);

    void Start()
    {
        totalCascarons = FindObjectsOfType<Cascaron>().Length;
        collectedCascarons = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        if (collision.relativeVelocity.magnitude >= breakSpeed)
        {
            Break();
        }
    }

    void Update()
    {
        if (GetComponent<Pickable>().pickedUp == true)
        {
            GetComponent<FMODUnity.StudioEventEmitter>().Stop();
        }
    }

    void Break()
    {
        collectedCascarons++;

        if (confettiEffect)
        {
            Debug.Log(confettiEffect.GetComponent<ParticleSystem>());
            confettiEffect.GetComponent<ParticleSystem>().time = 0f;
            Instantiate(confettiEffect, transform.position, Quaternion.identity);
            confettiEffect.GetComponent<ParticleSystem>().Play();
            AudioManager.Instance?.PlayOneShot3D(breakSFX, transform.position);
        }

        if (FindObjectsOfType<Cascaron>().Length == 1 && coinPrefab)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        UpdateUI();
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        if (cascaronUI == null) return;

        TextMeshProUGUI label = cascaronUI.GetComponentInChildren<TextMeshProUGUI>(true);
        if (label != null)
        {
            label.text = $"{collectedCascarons}/{MAX_CASCARONS}";
        }

        if (label != null)
        {
            label.text = $"{collectedCascarons}/{MAX_CASCARONS}";
            label.color = collectedCascarons >= MAX_CASCARONS ? Color.green : Color.white;
        }


        // Activate before starting the coroutine
        cascaronUI.SetActive(true);

        if (uiCoroutine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(uiCoroutine);
        }

        CascaronUIRunner runner = cascaronUI.GetComponent<CascaronUIRunner>();
        if (runner == null)
            runner = cascaronUI.AddComponent<CascaronUIRunner>();

        coroutineRunner = runner;
        uiCoroutine = runner.StartCoroutine(AnimateUI(cascaronUI, animDuration, holdTime));
    }

    private IEnumerator AnimateUI(GameObject ui, float duration, float hold)
    {
        RectTransform rect = ui.GetComponent<RectTransform>();
        Vector3 targetScale = rect.localScale; // store the intended scale (0.25, 0.25, 1)

        rect.localScale = Vector3.zero;

        // Scale pop in with rubber band
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            rect.localScale = targetScale * RubberBandScale(normalized);
            yield return null;
        }
        rect.localScale = targetScale;

        yield return new WaitForSeconds(hold);

        // Scale out
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            rect.localScale = targetScale * Mathf.Lerp(1f, 0f, normalized);
            yield return null;
        }

        rect.localScale = targetScale;
        ui.SetActive(false);
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

// Lightweight runner so coroutines survive the Cascaron being destroyed
public class CascaronUIRunner : MonoBehaviour { }