using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpen : MonoBehaviour
{
    // Script created by Mark D. 12/06/2025
    // Handles the door opening part of the roomba break cutscene
    // DoorOpen is called from the RoombaCamManager Script
    // door rotates around pivot point at a constant speed

    public Transform pivot;
    public float openAngle = 90f;
    public float openSpeed = 30f;

    private bool isOpening = false;
    private float currentAngle = 0f;

    public RoombAi roomba;

    public void OpenDoor()
    {
        if (!isOpening)
        {
            isOpening = true;
            //roomba.SwitchPatrol();
        }
    }

    void Update()
    {
        if (!isOpening) return;

        float rotate = openSpeed * Time.deltaTime;
        float newAngle = Mathf.Min(currentAngle + rotate, openAngle);

        pivot.Rotate(0f, newAngle - currentAngle, 0f);
        currentAngle = newAngle;

        if (currentAngle >= openAngle)
            isOpening = false;
    }
}
