using System.Collections;
using UnityEngine;
using TMPro;

public class NotEnoughCoinsUI : MonoBehaviour
{
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private TextMeshProUGUI label;
    private const int MAX_COINS = 20;
    private Coroutine hideCoroutine;

    public void Show()
    {
        if (label != null)
            label.text = $"Not Enough Trinkets!\n{GameManager.Score}/{MAX_COINS}";

        gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        gameObject.SetActive(false);
    }
}