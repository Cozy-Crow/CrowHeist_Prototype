using UnityEngine;

public class RespawnObject : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Transform spawnLocation;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("InvalidArea"))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (spawnLocation != null)
        {
            transform.SetParent(null);
            transform.root.position = spawnLocation.position;
            transform.root.rotation = spawnLocation.rotation;
            
            Rigidbody rb = transform.root.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
