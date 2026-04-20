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
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        GameObject coinsUIObj = GameObject.Find("CoinsUI");
        if (coinsUIObj != null) _coinsUI = coinsUIObj.GetComponent<CoinsUI>();

        GameObject altWinCoinsUIObj = GameObject.Find("AltWinCoinsUI");
        if (altWinCoinsUIObj != null) _altWinCoinsUI = altWinCoinsUIObj.GetComponent<AltWinCoinsUI>();

        GameObject collectionZoneObj = GameObject.Find("CollectionZoneCameraUI");
        if (collectionZoneObj != null) CollectionZoneCameraUI = collectionZoneObj.GetComponent<CollectionZoneCameraUI>();
    }

}
