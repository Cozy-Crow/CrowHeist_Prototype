using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_ThingsFallApart : MonoBehaviour
{

    // Script created by Mark D. 2/3/26
    // When things fall apart - this tracks the max height of the book when thrown
    // if it hits the "Ground" from a breakDropHeight it will drop a coin for now
    // it will eventually break once we have a broken book asset

    [SerializeField] private Collider frontCollider;
    [SerializeField] private Collider backCollider;

    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float breakDropHeight = 15f; // 15 is a solid number, it looks like crowley can throw it to a hieght of 11.2 from the ground

    private Rigidbody rb;
    private Pickable pickableUp;

    private bool _pickedUp;
    private bool hasBroken;

    private float maxHeight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pickableUp = GetComponent<Pickable>();
    }

    void Update()
    {
        if (rb == null || hasBroken) return;

        // While being held, keep resetting max height
        if (pickableUp != null && pickableUp.pickedUp)
        {
            //disable cover colliders if holding
            if (frontCollider) frontCollider.enabled = false;
            if (backCollider) backCollider.enabled = false;

            _pickedUp = true;
            maxHeight = transform.position.y;
            return;
        }
        else
        {
            //reenable cover colliders if not holding
            if (frontCollider) frontCollider.enabled = true;
            if (backCollider) backCollider.enabled = true;
        }

        // Only track after it has been picked up at least once
        if (!_pickedUp) return;

        float currentHeight = transform.position.y;

        // Track highest point after release
        if (currentHeight > maxHeight)
            maxHeight = currentHeight;

            Debug.Log("Max: " + maxHeight + "Cur: " + currentHeight);
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
        rb.isKinematic = true;
        SpawnItem();
        //Instantiante broken book prefab here
    }

    void SpawnItem()
    {
        if (spawnObject && spawnPoint)
        {
            GameObject spawned = Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
            spawned.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
    }
}
