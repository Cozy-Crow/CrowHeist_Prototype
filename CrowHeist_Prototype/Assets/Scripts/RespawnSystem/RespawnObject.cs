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
            Destroy(transform.root.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
            Respawn();
    }

    private void Respawn()
    {
        if (spawnLocation != null && prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);
        }
    }
}
