using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Bullseye : MonoBehaviour
{
    public GameObject confetti;
    public Transform confettiSpawnPoint;

    public GameObject coin;
    public Transform coinSpawnPoint;

    [SerializeField] private EventReference coinSparkle;

    private bool hasSpawned = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasSpawned && other.CompareTag("Dart"))
        {
            hasSpawned = true;
            StartCoroutine(SpawnSequence());
        }
    }

    private IEnumerator SpawnSequence()
    {
        AudioManager.Instance?.PlayOneShot3D(coinSparkle, transform.position);

        Instantiate(confetti, confettiSpawnPoint.position, confettiSpawnPoint.rotation);

        yield return new WaitForSeconds(1f); // delay here

        Instantiate(coin, coinSpawnPoint.position, coinSpawnPoint.rotation);
    }
}
