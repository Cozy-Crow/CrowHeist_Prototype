using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class ButtonSpriteSwap : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite clickedSprite;

    [Header("SFX")]
    public EventReference clickSFX;

    private Image buttonImage;
    private Sprite defaultSprite;
    private Coroutine swapRoutine;

    private void Awake()
    {
        buttonImage = GetComponent<Button>().targetGraphic as Image;
        defaultSprite = buttonImage.sprite;
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (swapRoutine != null)
            StopCoroutine(swapRoutine);

        swapRoutine = StartCoroutine(SpriteSwapRoutine());
    }

    private IEnumerator SpriteSwapRoutine()
    {

        buttonImage.sprite = clickedSprite;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayOneShot(clickSFX);

        yield return new WaitForSeconds(2f);

        buttonImage.sprite = defaultSprite;
    }
}