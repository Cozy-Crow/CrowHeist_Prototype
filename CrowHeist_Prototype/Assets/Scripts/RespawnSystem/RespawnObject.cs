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
        if (spawnLocation != null && prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);
            Destroy(transform.root.gameObject);
        }
    }
}