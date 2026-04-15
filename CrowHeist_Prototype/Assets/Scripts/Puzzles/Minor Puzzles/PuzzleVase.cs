using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PuzzleVase : MonoBehaviour
{
    // Script by Mark D. 2/7/26
    // break the vases puzzle: vase will break with a certain force and spawn broken prefab
    // the broken peices will explode and will not effect player movement

    public float breakSpeed = 10f;
    public GameObject brokenVasePrefab;
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;

    public float explodeForce = 2f;
    [SerializeField] private EventReference breakSFX;

    private bool broken;

    void OnCollisionEnter(Collision collision)
    {
        if (broken) return;

        if (collision.relativeVelocity.magnitude >= breakSpeed)
        {
            Break();
        }
    }

    void Break()
    {
        broken = true;
        AudioManager.Instance?.PlayOneShot3D(breakSFX, transform.position);

        GameObject brokenInstance = Instantiate(brokenVasePrefab, coinSpawnPoint.position, transform.rotation);

        // gets the broken peices from the broken vase prefab right after instantiated
        foreach (Rigidbody rb in brokenInstance.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(Random.onUnitSphere * explodeForce, ForceMode.Impulse);
        }

        // If this is the last PuzzleVase in the scene
        if (FindObjectsOfType<PuzzleVase>().Length == 1 && coinPrefab)
        {
            GameObject spawnedCoin = Instantiate(coinPrefab, transform.position, Quaternion.identity);
            spawnedCoin.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
