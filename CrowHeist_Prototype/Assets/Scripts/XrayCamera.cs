using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xray : MonoBehaviour
{
    [SerializeField] MaskObjects[] maskObjects;
    private Coroutine currentCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < maskObjects.Length; i++)
        {
            maskObjects[i].transform.localScale = new Vector3(0, 0, 0);
        }
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(ApplyMask(maskObjects[0]));
        }
        else
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            RemoveMask();
        }
    }

    private IEnumerator ApplyMask(MaskObjects mask)
    {
        for (int i = 0; i < maskObjects.Length; i++)
        {
            float elapsedTime = 0f;
            float duration = 1f;
            Vector3 startingScale = maskObjects[i].transform.localScale;
            Vector3 targetScale = new Vector3(3, 3, 3);

            while (elapsedTime < duration)
            {
                maskObjects[i].transform.localScale = Vector3.Lerp(startingScale, targetScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            maskObjects[i].transform.localScale = maskObjects[i].targetScale;
        }
    }

    private void RemoveMask()
    {
        float elapsedTime = 0f;
        float duration = 1f;

        for (int i = 0; i < maskObjects.Length; i++)
        {
            Vector3 startingScale = maskObjects[i].transform.localScale;
            Vector3 targetScale = new Vector3(0, 0, 0);

            while (elapsedTime < duration)
            {
                maskObjects[i].transform.localScale = Vector3.Lerp(startingScale, targetScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
            }

            maskObjects[i].transform.localScale = targetScale;
        }
    }
}

[Serializable]
public class MaskObjects
{
    public Transform transform;
    public Vector3 targetScale;
}
