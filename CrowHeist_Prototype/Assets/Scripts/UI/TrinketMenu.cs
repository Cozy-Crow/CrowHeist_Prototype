using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrinketMenu : MonoBehaviour
{
    [SerializeField] private Transform container;
    private static TrinketMenu instance;
    void Awake()
    {
        if (instance != null )
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        container.gameObject.SetActive(false);
    }
}
