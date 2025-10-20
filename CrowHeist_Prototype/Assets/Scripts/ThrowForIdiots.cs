using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowForIdiots : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private GameObject bookCover;
    private Rigidbody rb;
    private bool hasLanded = false;
    private bool wasThrown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (rb != null && !hasLanded)
        {
            if (rb.velocity.magnitude > 1f)
                wasThrown = true;
            
            if (wasThrown && rb.velocity.magnitude < 0.1f)
            {
                hasLanded = true;
                OpenBook();
                SpawnObjectOnTop();
            }
        }
    }

    void OpenBook()
    {
        if (bookCover != null)
            bookCover.transform.Rotate(Vector3.right, -120f);
    }

    void SpawnObjectOnTop()
    {
        if (spawnObject != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            Instantiate(spawnObject, spawnPos, Quaternion.identity);
        }
    }
}
