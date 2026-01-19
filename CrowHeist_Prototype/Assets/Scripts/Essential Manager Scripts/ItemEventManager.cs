using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemEventManager : MonoBehaviour
{
    public static ItemEventManager instance;
    [SerializeField] public UnityEvent e_spawnObj = new UnityEvent();
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
    public void SpawnItemTrigger()
    {
        // Trigger the event
        e_spawnObj?.Invoke();
    }


}
