using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPuzzle : MonoBehaviour
{
    [SerializeField] private float timeToGrow = 5f;
    [SerializeField] private float growthScale = 2f;
    [SerializeField] private GameObject plantPrefab;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fertilizer"))
        {
            StartCoroutine(GrowPlant());
        }
    }

    private IEnumerator GrowPlant()
    {
        GameObject plant = Instantiate(plantPrefab, transform.position, Quaternion.identity);
        Vector3 originalScale = plant.transform.localScale;
        Vector3 targetScale = Vector3.one * growthScale;
        Vector3 originalPos = plant.transform.position;
        Vector3 targetPos = originalPos + new Vector3(0, 4f, 0);

        float elapsedTime = 0f;

        while (elapsedTime < timeToGrow)
        {
            float t = elapsedTime / timeToGrow;
            plant.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            plant.transform.position = Vector3.Lerp(originalPos, targetPos, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plant.transform.localScale = targetScale;
        plant.transform.position = targetPos;
    }
}
