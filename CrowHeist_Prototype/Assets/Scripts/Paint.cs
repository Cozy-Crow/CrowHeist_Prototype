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
            int groundMask = LayerMask.GetMask("Ground");

            if (Physics.Raycast(spillSpawnPoint.position, Vector3.down, out hit, 5f, groundMask))
            {
                // Instantiate puddle decal
                GameObject puddleInstance = Instantiate(
                    paintSpillPrefab,
                    hit.point + Vector3.up * 0.01f,
                    Quaternion.FromToRotation(Vector3.up, hit.normal)
                );

                // OPTIONAL: Random rotation around Y axis
                float randomYRotation = Random.Range(0f, 360f);
                puddleInstance.transform.Rotate(Vector3.up, randomYRotation);

                // OPTIONAL: Random scale (URP Decal Projector must be set to "Inherit from Hierarchy")
                float randomScale = Random.Range(0.8f, 1.6f);
                puddleInstance.transform.localScale = new Vector3(randomScale, 1f, randomScale);
            }
        }
    }
}
