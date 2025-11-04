using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyableTV : MonoBehaviour
{
    public GameObject brokenTVPrefab;   
    public GameObject explosionEffect;  
    public AudioClip smashSound;        
    public bool canBeDestroyed = true;

    public void DestroyTV()
    {
        if (!canBeDestroyed) return;

        canBeDestroyed = false;

        if (explosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        if (smashSound)
            AudioSource.PlayClipAtPoint(smashSound, transform.position);

        if (brokenTVPrefab)
            Instantiate(brokenTVPrefab, transform.position, transform.rotation);

        Destroy(gameObject); 
    }
}
