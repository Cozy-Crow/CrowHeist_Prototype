using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private int maxCoins = 30;

    void Awake()
    {
        fillBar.type = Image.Type.Filled;
        fillBar.fillMethod = Image.FillMethod.Horizontal;
        fillBar.fillOrigin = (int)Image.OriginHorizontal.Left;
    }


    void OnEnable()
    {
        UpdateProgress(GameManager.Score);
    }

    public void UpdateProgress(int coins)
    {
        fillBar.fillAmount = Mathf.Clamp01((float)coins / maxCoins);

        if (label != null)
            label.text = $"Total Trinkets Collected: {coins}/{maxCoins}";
    }
}