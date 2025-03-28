using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIEventManager : MonoBehaviour
{
    public static AIEventManager instance;

    [SerializeField] public UnityEvent e_pickup = new UnityEvent();
    [SerializeField] public UnityEvent e_playerdirty = new UnityEvent();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
