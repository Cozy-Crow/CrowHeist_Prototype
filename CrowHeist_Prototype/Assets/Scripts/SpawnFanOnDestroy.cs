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

        Transform armature = transform.Find("Armature");
        Transform socle = transform.Find("Socle");

        if (armature != null)
        {
            armature.SetParent(null); // Detach from parent
            armature.tag = "FloorFan";
            Debug.Log("FloorFan");
        }

        if (socle != null)
        {
            socle.SetParent(null); // Detach from parent
        }
    }


}
