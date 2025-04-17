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
        fanSpawnLocation = transform.Find("Armature");
        baseSpawnLocation = transform.Find("Socle");
    }

    // void OnDestroy()
    // {
    //     if (!Application.isPlaying) return;

    //     foreach (GameObject prefab in prefabsToSpawn)
    //     {
    //         if (prefab == null) continue;

    //         if (prefab.CompareTag("FloorFan") && fanSpawnLocation != null)
    //         {
    //             Instantiate(prefab, fanSpawnLocation.position, fanSpawnLocation.rotation);
    //         }
    //         else if (prefab.CompareTag("FloorFanBase") && baseSpawnLocation != null)
    //         {
    //             Instantiate(prefab, baseSpawnLocation.position, baseSpawnLocation.rotation);
    //         }
    //     }
    // }

    void OnDestroy()
    {
        if (!Application.isPlaying) return;

        Transform floorFan = transform.Find("Armature/Bone/Bone.001/Bone.002/FloorFan");
        Transform socle = transform.Find("Socle");

        if (floorFan != null)
        {
            floorFan.SetParent(null); // Detach
            floorFan.tag = "FloorFan"; // Set tag
            if (floorFan.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = floorFan.gameObject.AddComponent<Rigidbody>();
                rb.mass = 1f; // Customize as needed
            }

        }

        if (socle != null)
        {
            socle.SetParent(null);
            socle.tag = "FloorFanBase";
        }
    }


}
