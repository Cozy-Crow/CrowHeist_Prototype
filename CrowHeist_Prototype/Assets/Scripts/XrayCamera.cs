using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XrayCamera : MonoBehaviour
{
    [SerializeField, Tooltip("Mask object for the X-ray effect")]
    MaskObjects maskObject;
    [SerializeField, Tooltip("Layer mask for raycasting")] 
    LayerMask maskLayer;
    [SerializeField, Tooltip("Time taken to expand the mask")]
    float expandTime = .3f;
    [SerializeField, Tooltip("Time taken to retract the mask")] 
    float retractTime = 0.1f;
    private Transform player;
    private Coroutine currentCoroutine;
    private Transform raycastTransform;
    private bool seePlayer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        maskObject.transform.localScale = new Vector3(0, 0, 0);
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        Vector3 direction = player.position - transform.position;
        Physics.Raycast(transform.position, direction.normalized, out hit, Mathf.Infinity, maskLayer);
        Debug.DrawRay(transform.position, direction.normalized * 1000, Color.red);

        if (hit.collider != null && !hit.collider.CompareTag("Player"))
        {
            seePlayer = false;
            currentCoroutine = StartCoroutine(ApplyMask(maskObject));
        }
        else
        {
            seePlayer = true;
            currentCoroutine = StartCoroutine(RemoveMask());
        }
        raycastTransform = hit.transform;
    }

    /// <summary>
    /// Applies the mask by expanding it to the target scale.
    /// </summary>
    /// <param name="mask"> The mask object to apply the effect on. </param>
    private IEnumerator ApplyMask(MaskObjects mask)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        float elapsedTime = 0f;
        Vector3 startingScale = maskObject.transform.localScale;
        Vector3 targetScale = maskObject.targetScale;

        while (elapsedTime < expandTime)
        {
            maskObject.transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / expandTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        maskObject.transform.localScale = maskObject.targetScale; 
    }

    /// <summary>
    /// Removes the current mask by retracting it to zero scale.
    /// </summary>
    private IEnumerator RemoveMask()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        float elapsedTime = 0f;

        Vector3 startingScale = maskObject.transform.localScale;
        Vector3 targetScale = new Vector3(0, 0, 0);

        while (elapsedTime < retractTime)
        {
            maskObject.transform.localScale = Vector3.Lerp(startingScale, targetScale, elapsedTime / retractTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        maskObject.transform.localScale = targetScale;
    }
}

[Serializable]
public class MaskObjects
{
    public Transform transform;
    public Vector3 targetScale;
}
