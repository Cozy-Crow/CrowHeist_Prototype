using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteSwap : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject interactableCrowley;
    [SerializeField] private GameObject[] funnyCrowleys;

    [Header("SFX")]
    public FMODUnity.EventReference clickSFX;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        TrinketMenu.instance.StartCoroutine(SwapRoutine());
    }

    private IEnumerator SwapRoutine()
    {
        interactableCrowley.SetActive(false);

        GameObject randomCrowley = funnyCrowleys[Random.Range(0, funnyCrowleys.Length)];
        randomCrowley.SetActive(true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShot(clickSFX);

        float elapsed = 0f;
        while (elapsed < 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        randomCrowley.SetActive(false);
        interactableCrowley.SetActive(true);
    }
}