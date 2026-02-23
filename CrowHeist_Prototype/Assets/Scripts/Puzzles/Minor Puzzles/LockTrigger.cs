using UnityEngine;
using System.Collections;
using FMODUnity;

public class LockTrigger : MonoBehaviour
{
    [SerializeField] private string keyTag = "";
    [SerializeField] private TraversalCheckpoint checkpoint;
    [SerializeField] private GameObject lockInteractionTrigger;
    [SerializeField] private float minVelocity = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float keyTipOffset = 0.5f;
    [SerializeField] private float keyBackOffset = 0.1f;
    [Header("Audio")]
    [SerializeField] private EventReference keyUnlock;
    [SerializeField] private EventReference openLock;

    private void Start()
    {
        if (TryGetComponent<Pickable>(out var pickable)) pickable.enabled = false;
        if (lockInteractionTrigger) lockInteractionTrigger.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(keyTag) || checkpoint == null) return;
        
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null || rb.velocity.magnitude < minVelocity) return;

        if (other.TryGetComponent<Pickable>(out var keyPickable)) Destroy(keyPickable);
        if (other.TryGetComponent<Outline>(out var keyOutline)) Destroy(keyOutline);
        
        Transform keyInteractionTrigger = other.transform.Find("InteractionTrigger");
        if (keyInteractionTrigger) Destroy(keyInteractionTrigger.gameObject);
        
        StartCoroutine(UnlockMotion(other.transform));
    }

    private IEnumerator UnlockMotion(Transform key)
    {
        Rigidbody keyRb = key.GetComponent<Rigidbody>();
        if (keyRb)
        {
            keyRb.useGravity = false;
            keyRb.velocity = Vector3.zero;
            keyRb.isKinematic = true;
        }

        Quaternion targetRot = transform.rotation * Quaternion.Euler(0, 90, 90);
        Vector3 targetPos = transform.position + transform.forward * keyTipOffset - transform.up * keyBackOffset;
       
        while (Vector3.Distance(key.position, targetPos) > 0.01f)
        {
            key.position = Vector3.MoveTowards(key.position, targetPos, moveSpeed * Time.deltaTime);
            key.rotation = Quaternion.RotateTowards(key.rotation, targetRot, rotateSpeed * 2f * Time.deltaTime);
            if (lockInteractionTrigger && lockInteractionTrigger.activeSelf) 
                lockInteractionTrigger.SetActive(false);
            yield return null;
        }
        key.position = targetPos;
        key.rotation = targetRot;
        AudioManager.Instance?.PlayOneShot3D(keyUnlock, transform.position);

        Quaternion endRot = key.rotation * Quaternion.Euler(90, 0, 0);
        float rotated = 0f;
        while (rotated < 90f)
        {
            float step = rotateSpeed * Time.deltaTime;
            rotated += step;
            key.rotation = Quaternion.RotateTowards(key.rotation, endRot, step);
            if (lockInteractionTrigger && lockInteractionTrigger.activeSelf) 
                lockInteractionTrigger.SetActive(false);
            yield return null;
        }
        key.rotation = endRot;

        key.SetParent(transform);
        if (keyRb) keyRb.isKinematic = true;

        Collider keyCollider = key.GetComponent<Collider>();
        Collider lockCollider = GetComponent<Collider>();
        if (keyCollider && lockCollider)
            Physics.IgnoreCollision(keyCollider, lockCollider);
        
        if (lockCollider) lockCollider.isTrigger = false;

        if (TryGetComponent<Pickable>(out var pickable)) pickable.enabled = true;
        if (lockInteractionTrigger) lockInteractionTrigger.SetActive(true);

        Rigidbody lockRb = GetComponent<Rigidbody>();
        if (!lockRb) lockRb = gameObject.AddComponent<Rigidbody>();
        AudioManager.Instance?.PlayOneShot3D(openLock, transform.position);
        lockRb.useGravity = true;

        checkpoint.MoveObjectsToEndPositions();
    }
}