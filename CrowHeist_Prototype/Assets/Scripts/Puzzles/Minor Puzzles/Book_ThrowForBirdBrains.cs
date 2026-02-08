using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_ThrowForBirdBrains : MonoBehaviour
{
    // Script created by Mark D. 2/3/26
    // When things fall apart - this tracks the max height of the book when thrown
    // if it hits the "Ground" from a breakDropHeight it will drop a coin for now
    // it will eventually break once we have a broken book asset

    [SerializeField] private Collider frontCollider;
    [SerializeField] private Collider backCollider;

    [SerializeField] private GameObject bookCover;
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;

    private Rigidbody rb;
    private Pickable pickableUp;

    private bool hasLanded;
    private bool wasThrown;
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
        if (rb == null) return;

        if (pickableUp.pickedUp)
        {
            //disable cover colliders if holding
            if (frontCollider) frontCollider.enabled = false;
            if (backCollider) backCollider.enabled = false;

            _pickedUp = true;
            return; // don't evaluate throw/landing while held
        }
        else
        {
            //reenable cover colliders if not holding
            if (frontCollider) frontCollider.enabled = true;
            if (backCollider) backCollider.enabled = true;
        }

        // Only care about landing after it has been picked up and released
        if (!_pickedUp || hasLanded) return;

        float speed = rb.velocity.magnitude;

        if (speed > 1f)
            wasThrown = true;

        if (wasThrown && speed < 0.1f)
        {
            hasLanded = true;
            rb.isKinematic = true;

            if (bookCover)
                StartCoroutine(SmoothOpen());
        }
    }

    IEnumerator SmoothOpen()
    {
        var startPos = bookCover.transform.localPosition;
        var startRot = bookCover.transform.localEulerAngles;
        var targetPos = startPos + new Vector3(-1f, 0, 0.238f);
        var targetRot = startRot + new Vector3(0, 180f, 0);

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            float p = t;
            bookCover.transform.localPosition = Vector3.Lerp(startPos, targetPos, p) + Vector3.up * (Mathf.Sin(p * Mathf.PI) * 0.2f);
            bookCover.transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, p);
            yield return null;
        }

        bookCover.transform.localPosition = targetPos;
        bookCover.transform.localEulerAngles = targetRot;

        SpawnItem();
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
