using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowForIdiots : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Animator bookAnimator;
    private bool hasLanded = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasLanded && collision.gameObject.CompareTag("Ground"))
        {
            hasLanded = true;
            OpenBook();
            SpawnObjectOnTop();
        }
    }

    void OpenBook()
    {
        // Trigger the book opening
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
