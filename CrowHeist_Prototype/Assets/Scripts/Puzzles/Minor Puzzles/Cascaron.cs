using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cascaron : MonoBehaviour
{
    public float breakSpeed = 5f;
    public GameObject confettiEffect;
    public GameObject coinPrefab;

    private static int remainingCascarones;
    private static bool initialized;

    void Start()
    {
        if (!initialized)
        {
            remainingCascarones = FindObjectsOfType<Cascaron>().Length;
            initialized = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            return;
        }
        else
        {
            // Debug.Log("Force: "+collision.relativeVelocity.magnitude);
            if (collision.relativeVelocity.magnitude >= breakSpeed)
            {
                Break();
            }
        }
    }

    void Break()
    {
        Debug.Log("Remaining: "+remainingCascarones);
        if (confettiEffect)
            Instantiate(confettiEffect, transform.position, Quaternion.identity);

        remainingCascarones--;

        if (remainingCascarones <= 0 && coinPrefab)
            Instantiate(coinPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
