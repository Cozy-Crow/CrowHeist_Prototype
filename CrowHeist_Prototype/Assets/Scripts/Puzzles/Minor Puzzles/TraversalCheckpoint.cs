using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversalCheckpoint : MonoBehaviour
{
    [SerializeField] private string keyTag = "";
    [SerializeField] private int numberOfObjects = 1;
    [SerializeField] private GameObject[] targetObjects;
    [SerializeField] private Vector3[] endPositions;
    [SerializeField] private Vector3[] endRotations;
    [SerializeField] private float moveSpeed = 1f;

    private bool isOpen = false;

    private void OnValidate()
    {
        if (targetObjects == null || targetObjects.Length != numberOfObjects)
        {
            System.Array.Resize(ref targetObjects, numberOfObjects);
        }
        if (endPositions == null || endPositions.Length != numberOfObjects)
        {
            System.Array.Resize(ref endPositions, numberOfObjects);
        }
        if (endRotations == null || endRotations.Length != numberOfObjects)
        {
            System.Array.Resize(ref endRotations, numberOfObjects);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(keyTag))
        {
            MoveObjectsToEndPositions();
            isOpen = true;
        }
    }

    public void MoveObjectsToEndPositions()
    {
        if (isOpen) return;
        StartCoroutine(MoveObjects());
    }

    private IEnumerator MoveObjects()
    {
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<BoxCollider>().isTrigger = false;

        Vector3[] startPositions = new Vector3[targetObjects.Length];
        Quaternion[] startRotations = new Quaternion[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            if (targetObjects[i] != null)
            {
                startPositions[i] = targetObjects[i].transform.localPosition;
                startRotations[i] = targetObjects[i].transform.localRotation;
            }
        }

        float t = 0;
        while (t < 1)
        {
            t += moveSpeed * Time.deltaTime;
            for (int i = 0; i < targetObjects.Length; i++)
            {
                if (targetObjects[i] != null)
                {
                    targetObjects[i].transform.localPosition = Vector3.Lerp(startPositions[i], endPositions[i], t);
                    targetObjects[i].transform.localRotation = Quaternion.Lerp(startRotations[i], Quaternion.Euler(endRotations[i]), t);
                }
            }
            yield return null;
        }

        
    }
}
