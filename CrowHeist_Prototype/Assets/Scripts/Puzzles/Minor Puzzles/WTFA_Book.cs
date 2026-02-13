using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WTFA_Book : MonoBehaviour
{
    // Script created by Mark D. 2/13/26
    // When Things Fall Apart - refactored version
    // Spawns broken prefab when dropped from sufficient height

    [SerializeField] private GameObject brokenBookPrefab;
    [SerializeField] private Transform brokenBookSpawnPoint;

    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float breakDropHeight = 15f;

    private Rigidbody rb;
    private Pickable pickable;

    private bool wasPickedUp;
    private bool hasBroken;

    private float maxHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickable = GetComponent<Pickable>();
    }

    void Update()
    {
        if (rb == null || hasBroken) return;

        if (pickable != null && pickable.pickedUp)
        {
            wasPickedUp = true;
            maxHeight = transform.position.y;
            return;
        }

        if (!wasPickedUp) return;

        float currentHeight = transform.position.y;

        if (currentHeight > maxHeight)
            maxHeight = currentHeight;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasBroken || rb == null) return;
        if (!collision.gameObject.CompareTag("Ground")) return;

        float dropDistance = maxHeight - transform.position.y;

        if (dropDistance >= breakDropHeight)
        {
            BreakBook();
        }
    }

    void BreakBook()
    {
        hasBroken = true;

        GameObject broken = Instantiate(brokenBookPrefab, transform.position, transform.rotation);

        foreach (Rigidbody r in broken.GetComponentsInChildren<Rigidbody>())
        {
            r.velocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
        }

        SpawnItem();

        Destroy(gameObject);
    }

    void SpawnItem()
    {
        if (spawnObject && spawnPoint)
        {
            GameObject spawned = Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);

            Rigidbody spawnedRb = spawned.GetComponent<Rigidbody>();
            if (spawnedRb != null)
                spawnedRb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
    }
}

