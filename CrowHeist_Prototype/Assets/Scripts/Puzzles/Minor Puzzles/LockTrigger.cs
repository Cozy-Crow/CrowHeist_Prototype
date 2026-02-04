using UnityEngine;

public class LockTrigger : MonoBehaviour
{
    [SerializeField] private string keyTag = "";
    [SerializeField] private TraversalCheckpoint checkpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(keyTag) && checkpoint != null)
        {
            gameObject.AddComponent<Rigidbody>();
            gameObject.GetComponent<BoxCollider>().isTrigger = false;
            checkpoint.MoveObjectsToEndPositions();
        }
    }
}