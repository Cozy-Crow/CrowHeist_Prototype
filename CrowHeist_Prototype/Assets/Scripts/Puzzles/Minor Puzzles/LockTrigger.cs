using UnityEngine;

public class LockTrigger : MonoBehaviour
{
    [SerializeField] private string keyTag = "";
    [SerializeField] private TraversalCheckpoint checkpoint;
    [SerializeField] private float minVelocity = 3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(keyTag) && checkpoint != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity.magnitude >= minVelocity)
            {
                UnlockMotion(other.transform);
                checkpoint.MoveObjectsToEndPositions();
            }
        }
    }

    private void UnlockMotion(Transform key)
    {
        key.position = transform.position;
        key.Rotate(0, 90, 0);
        key.SetParent(transform);
        
        Rigidbody keyRb = key.GetComponent<Rigidbody>();
        if (keyRb) Destroy(keyRb);
        
        Rigidbody lockRb = GetComponent<Rigidbody>();
        if (!lockRb) lockRb = gameObject.AddComponent<Rigidbody>();
        lockRb.useGravity = true;
    }
}