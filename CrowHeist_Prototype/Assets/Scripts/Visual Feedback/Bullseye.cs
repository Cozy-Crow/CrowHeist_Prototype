using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullseye : MonoBehaviour
{
    public ParticleSystem confetti;
    public GameObject coin;
    public Transform spawnPoint;

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && other.CompareTag("Dart"))
        {
            hasSpawned = true;

            if (confetti != null)
                confetti.Play();

            Instantiate(coin, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
