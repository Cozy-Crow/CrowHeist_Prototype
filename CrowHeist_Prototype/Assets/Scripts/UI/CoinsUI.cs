using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> _coins = new List<Sprite>();

    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    private void Start()
    {
        UpdateCoins(0);
    }

    public void UpdateCoins(int coins)
    {
        if (coins > _coins.Count)
        {
            Debug.LogError("Not enough coin sprites in the list");
            return;
        }

        _image.sprite = _coins[GameManager.Score];
    }
}
