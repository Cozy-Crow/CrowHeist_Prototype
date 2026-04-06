using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPuzzle : MonoBehaviour
{
    [SerializeField] private float timeToGrow = 5f;
    [SerializeField] private float growthScale = 2f;
    [SerializeField] private float branchTargetScale = 0.5f;
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private GameObject flowerCenter;
    [SerializeField] private float flowerGrowDuration = 1f;

    private List<Transform> branches = new List<Transform>();
    private Vector3 originalPlantPos;

    void Start()
    {
        originalPlantPos = plantPrefab.transform.localPosition;
        foreach (Transform child in plantPrefab.GetComponentsInChildren<Transform>(true))
        {
            if (child.CompareTag("Branch"))
            {
                child.localScale = new Vector3(0f, 0.5f, 0f);
                branches.Add(child);
            }
        }

        if (flowerCenter != null) flowerCenter.transform.localScale = new Vector3(0f, 0f, flowerCenter.transform.localScale.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fertilizer"))
        {
            StartCoroutine(GrowPlant());
        }
    }

    private IEnumerator GrowPlant()
    {
        Vector3 originalScale = plantPrefab.transform.localScale;
        Vector3 targetScale = Vector3.one * growthScale;
        Vector3 targetPos = originalPlantPos + new Vector3(0, 4f, 0);

        float branchInterval = timeToGrow / branches.Count;
        int branchIndex = 0;
        float elapsedTime = 0f;

        while (elapsedTime < timeToGrow)
        {
            float t = elapsedTime / timeToGrow;
            plantPrefab.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            plantPrefab.transform.localPosition = Vector3.Lerp(originalPlantPos, targetPos, t);

            int targetBranchIndex = Mathf.FloorToInt(elapsedTime / branchInterval);
            if (targetBranchIndex > branchIndex && branchIndex < branches.Count)
            {
                StartCoroutine(GrowBranch(branches[branchIndex]));
                branchIndex++;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        plantPrefab.transform.localScale = targetScale;
        plantPrefab.transform.localPosition = targetPos;

        if (flowerCenter != null) yield return StartCoroutine(GrowFlower());
    }

    private IEnumerator GrowFlower()
    {
        float originalZ = flowerCenter.transform.localScale.z;
        Vector3 start = new Vector3(0f, 0f, originalZ);
        Vector3 target = new Vector3(0.4f, 0.4f, originalZ);
        float elapsed = 0f;

        while (elapsed < flowerGrowDuration)
        {
            flowerCenter.transform.localScale = Vector3.Lerp(start, target, elapsed / flowerGrowDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        flowerCenter.transform.localScale = target;
    }

    private IEnumerator GrowBranch(Transform branch)
    {
        Vector3 start = new Vector3(0f, 0.5f, 0f);
        Vector3 target = Vector3.one * branchTargetScale;
        float elapsed = 0f;
        float duration = timeToGrow / branches.Count;

        while (elapsed < duration)
        {
            branch.localScale = Vector3.Lerp(start, target, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        branch.localScale = target;
    }
}
