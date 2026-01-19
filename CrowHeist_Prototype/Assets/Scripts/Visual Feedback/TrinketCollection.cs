using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrinketCollection : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectEffect;

    void OnTriggerEnter(Collider other)
    {
        other.TryGetComponent(out Trinket trinket);
        
        if(trinket != null)
        {
            TrinketMenu.instance.UpdateItem(trinket.TrinketData);
            collectEffect?.Play();
            Destroy(other.gameObject);
        }
    }
}
