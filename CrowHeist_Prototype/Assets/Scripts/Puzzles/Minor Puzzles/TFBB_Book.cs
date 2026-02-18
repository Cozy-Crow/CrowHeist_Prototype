using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class TFBB_Book : MonoBehaviour
{
    // Script created by Mark D. 2/13/26
    // Throwing For Bird Brains - new prefab version (closed -> open swap)

    [SerializeField] private GameObject openBookPrefab;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EventReference bookOpen;

    private Rigidbody rb;
    private Pickable pickable;

    private bool hasLanded;
    private bool wasThrown;
    private bool wasPickedUp;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickable = GetComponent<Pickable>();
    }

    void Update()
    {
        if (rb == null) return;

        if (pickable.pickedUp)
        {
            wasPickedUp = true;
            return;
        }

        if (!wasPickedUp || hasLanded) return;

        float speed = rb.velocity.magnitude;

        if (speed > 1f)
            wasThrown = true;

        if (wasThrown && speed < 0.1f)
        {
            hasLanded = true;
            rb.isKinematic = true;

            OpenBook();
        }
    }

    void OpenBook()
    {
        AudioManager.Instance?.PlayOneShot3D(bookOpen, transform.position);

        if (openBookPrefab)
        {
            Instantiate(openBookPrefab, transform.position, transform.rotation);
        }

        SpawnItem();

        Destroy(gameObject); // remove closed version
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
