using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillation : MonoBehaviour
{
    [SerializeField] private float rotationAngle = 45f; // Maximum angle to oscillate to each side
    [SerializeField] private float rotationSpeed = 1f;  // Speed of oscillation

    private float initialYRotation;

    void Start()
    {
        // Store the starting Y rotation
        initialYRotation = transform.eulerAngles.y;
    }

    void Update()
    {
        // Calculate the new Y rotation using Mathf.Sin
        float angle = Mathf.Sin(Time.time * rotationSpeed) * rotationAngle;
        transform.rotation = Quaternion.Euler(0f, initialYRotation + angle, 0f);
    }
}
