using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paint : MonoBehaviour
{
    public GameObject paintSpillPrefab;
    public Transform spillSpawnPoint; // Where the paint spills from
    RaycastHit hit;
    


    public void Spill()
    {    
        if (paintSpillPrefab != null && spillSpawnPoint != null)
        {
            int groundMask = LayerMask.GetMask("Ground"); // Create a "Ground" layer in Unity

            if (Physics.Raycast(spillSpawnPoint.position, Vector3.down, out hit, 5f, groundMask))
            {
                Instantiate(
                    paintSpillPrefab,
                    hit.point + Vector3.up * 0.01f,
                    Quaternion.FromToRotation(Vector3.up, hit.normal)
                );
            }
        }

    }
}
