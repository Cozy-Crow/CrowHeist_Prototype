using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Bullseye : MonoBehaviour
{
    public ParticleSystem confetti;
    public GameObject coin;
    public Transform spawnPoint;

    [SerializeField] private EventReference coinSparkle;

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && other.CompareTag("Dart"))
        {
            hasSpawned = true;
            AudioManager.Instance?.PlayOneShot3D(coinSparkle, transform.position);

            if (confetti != null)
                confetti.Play();

            Instantiate(coin, spawnPoint.position, spawnPoint.rotation);
        }
    }
}
