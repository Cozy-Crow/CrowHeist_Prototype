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
            Vector3 spawnPos = hit.point + hit.normal * 0.01f;

            // Rotate 90 degrees on X, and random rotation on Y
            Quaternion rot = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);

            GameObject puddleInstance = Instantiate(paintSpillPrefab, spawnPos, rot);

            // OPTIONAL: Random scale (if using Decal Projector set to "Inherit from Hierarchy")
            float randomScale = Random.Range(0.1f, 0.6f);
            Vector3 finalScale = new Vector3(randomScale, 1f, randomScale);
            StartCoroutine(AnimateScale(puddleInstance, 0.5f, finalScale));
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
