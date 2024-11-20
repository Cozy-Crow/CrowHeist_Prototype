using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    [SerializeField] private CoinsUI _coinsUI;

    public CoinsUI CoinsUI { get => _coinsUI;}

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
    }
}
