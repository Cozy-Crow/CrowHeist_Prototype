using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFanOnDestroy : MonoBehaviour
{
    public GameObject[] prefabsToSpawn;
    private Transform fanSpawnLocation;
    private Transform baseSpawnLocation;

    void Start()
    {
        fanSpawnLocation = transform.Find("FloorFan");
        baseSpawnLocation = transform.Find("Socle");
    }


    public void Disassemble()
    {
        if (!Application.isPlaying) return;

        Transform floorFan = transform.Find("FloorFan");
        Transform socle = transform.Find("Socle");

        if (floorFan != null)
        {
            floorFan.SetParent(null); // Detach
            floorFan.tag = "FloorFan"; // Set tag

            // Enable gravity
            Rigidbody rb = floorFan.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
            }

            // Enable Pickable script
            var pickable = floorFan.GetComponent<Pickable>();
            if (pickable != null)
            {
                pickable.enabled = true;
            }

            // Enable Outline script
            var outline = floorFan.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = true;
            }

            // Unhide InteractionTrigger
            Transform interactionTrigger = floorFan.Find("Bone/Bone.001/Bone.002/InteractionTrigger");
            if (interactionTrigger != null)
            {
                interactionTrigger.gameObject.SetActive(true);
            }
        }

        if (socle != null)
        {
            socle.SetParent(null);
            socle.tag = "FloorFanBase";
        }
    }



}
