using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleWand : MonoBehaviour
{
    public GameObject bubblePrefab;
    public float spawnDistance = 0.5f;

    private Pickable pickable; // reference to Pickable script

    void Awake()
    {
        pickable = GetComponent<Pickable>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnBubble();
        }
    }

    void SpawnBubble()
    {
        if (bubblePrefab != null && pickable != null && pickable.pickedUp)
        {
            Vector3 spawnPosition = transform.position + transform.forward * spawnDistance;
            Quaternion spawnRotation = transform.rotation;

            Instantiate(bubblePrefab, spawnPosition, spawnRotation);
        }
        else if (bubblePrefab == null)
        {
            Debug.LogWarning("BubblePrefab not assigned!");
        }
        else if (pickable != null && !pickable.pickedUp)
        {
            Debug.Log("No Bubble");
        }
    }
}
