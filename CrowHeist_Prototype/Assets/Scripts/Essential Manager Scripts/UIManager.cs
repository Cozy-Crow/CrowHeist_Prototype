using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private CoinsUI _coinsUI;
    [SerializeField] private AltWinCoinsUI _altWinCoinsUI;
    public CollectionZoneCameraUI CollectionZoneCameraUI; // ADD THIS LINE

    public CoinsUI CoinsUI { get => _coinsUI;}
    public AltWinCoinsUI AltWinCoinsUI { get => _altWinCoinsUI;}



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        _coinsUI = GameObject.Find("CoinsUI").GetComponent<CoinsUI>();
        _altWinCoinsUI = GameObject.Find("AltWinCoinsUI").GetComponent<AltWinCoinsUI>();
        CollectionZoneCameraUI = GameObject.Find("CollectionZoneCameraUI").GetComponent<CollectionZoneCameraUI>();
    }
}
