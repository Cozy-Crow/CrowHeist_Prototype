using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleWand : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnDistance = 0.5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnBubble();
        }
    }

    void SpawnBubble()
    {
        if (bubblePrefab != null)
        {
            Vector3 spawnPosition = transform.position + transform.forward * spawnDistance;
            Quaternion spawnRotation = transform.rotation;

            Instantiate(bubblePrefab, spawnPosition, spawnRotation);
        }
        else
        {
            Debug.LogWarning("BubblePrefab not assigned!");
        }
    }
}
