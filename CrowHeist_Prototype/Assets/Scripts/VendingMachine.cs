using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingMachine : MonoBehaviour
{
    private ItemEventManager itemEventManager;
    private void Start()
    {
        itemEventManager = FindObjectOfType<ItemEventManager>();
        
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Quarter"))
        {
            Destroy(other.gameObject);
            Notify();
        }
    }

    // Update is called once per frame
    public void Notify()
    {
        Debug.Log("Notifying...");
        itemEventManager.SpawnItemTrigger();
    }
}
