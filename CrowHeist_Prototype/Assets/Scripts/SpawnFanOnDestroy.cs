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

            // Enable Pickable
            var pickable = floorFan.GetComponent<Pickable>();
            if (pickable != null)
            {
                pickable.enabled = true;
            }

            // Enable Outline
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

            // Disable Oscillation script on Bone
            Transform bone = floorFan.Find("Bone");
            if (bone != null)
            {
                Oscillation osc = bone.GetComponent<Oscillation>();
                if (osc != null)
                {
                    osc.enabled = false;
                }
            }

            // Change tag on ForceArea
            Transform forceArea = floorFan.Find("Bone/Bone.001/Bone.002/BladeControl/ForceArea");
            if (forceArea != null)
            {
                forceArea.tag = "FloorFanForce";
            }

            // Check if FloorFan is grounded and set rotation
            Collider floorFanCollider = floorFan.GetComponent<Collider>();
            if (floorFanCollider != null)
            {
                float raycastDistance = floorFanCollider.bounds.extents.y + 0.1f;
                if (Physics.Raycast(floorFan.position, Vector3.down, raycastDistance))
                {
                    floorFan.eulerAngles = new Vector3(-80f, -90f, 0f);
                }
            }
        }

        if (socle != null)
        {
            socle.SetParent(null);
            socle.tag = "FloorFanBase";
        }
    }


}
