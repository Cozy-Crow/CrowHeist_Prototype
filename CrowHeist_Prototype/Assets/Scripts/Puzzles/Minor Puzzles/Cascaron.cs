using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cascaron : MonoBehaviour
{
    // Cascaron Puzzle implemented by Mark D. 2/6/26
    // This is the only script needed for the puzzle and is on the cascaron prefab
    // just drag in however many cascarones you want in the level
    // the last one to be destroyed will drop a coin

    public float breakSpeed = 5f;
    public GameObject confettiEffect;
    public GameObject coinPrefab;

    private Vector3 zero = new Vector3(0, 0, 0);

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        // Debug.Log("Force: "+collision.relativeVelocity.magnitude);
        if (collision.relativeVelocity.magnitude >= breakSpeed)
        {
            Break();   
        }
    }

    void Update()
    {
        if (GetComponent<Pickable>().pickedUp == true)
            {
              GetComponent<FMODUnity.StudioEventEmitter>().Stop(); //stops emitter audio when picked up   
            }
    }

    void Break()
    {
        if (confettiEffect)
        {
            Debug.Log(confettiEffect.GetComponent<ParticleSystem>());
            confettiEffect.GetComponent<ParticleSystem>().time = 0f;
            Instantiate(confettiEffect, transform.position, Quaternion.identity);
            confettiEffect.GetComponent<ParticleSystem>().Play();
            
           
            
        }

        if (FindObjectsOfType<Cascaron>().Length == 1 && coinPrefab)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
