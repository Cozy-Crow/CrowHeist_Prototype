using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Settings")]
    [SerializeField] private int _maxCoins = 25;

    public CanvasGroup CanvasGroup => _canvasGroup;

    private void Start()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(false); 
        UpdateCoins(0);
    }

    public void UpdateCoins(int coins)
    {
        _coinText.text = $"{coins}/{_maxCoins}";
    }
}
