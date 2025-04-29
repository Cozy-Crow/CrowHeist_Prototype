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
                float randomScale = Random.Range(0.1f, 0.6f);
                Vector3 finalScale = new Vector3(randomScale, 1f, randomScale);
                StartCoroutine(AnimateScale(puddleInstance, 0.5f, finalScale)); // 0.5f is duration in seconds

            }
        }
    }
    private IEnumerator AnimateScale(GameObject puddle, float duration, Vector3 targetScale)
    {
        Vector3 startScale = Vector3.zero;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            puddle.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        puddle.transform.localScale = targetScale;
    }

}
