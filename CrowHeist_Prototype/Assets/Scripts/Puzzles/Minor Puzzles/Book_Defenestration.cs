using System.Collections;
using UnityEngine;

public class Book_Defenestration : MonoBehaviour
{
    [SerializeField] private Mesh openBookMesh;
    [SerializeField] private GameObject coin;
    [SerializeField] private string windowTag = "HeistZone";
    [SerializeField] private Transform spawnPoint;
    
    private bool puzzleSolved = false;
    private Rigidbody rb;
    private MeshFilter meshFilter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!puzzleSolved && other.CompareTag(windowTag))
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;
        SpawnItem();
        if (meshFilter != null && openBookMesh != null) meshFilter.mesh = openBookMesh;
    }

    void SpawnItem()
    {
        if (coin && spawnPoint)
        {
            GameObject spawned = Instantiate(coin, spawnPoint.position, spawnPoint.rotation);

            Rigidbody spawnedRb = spawned.GetComponent<Rigidbody>();
            if (spawnedRb != null)
                spawnedRb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
        }
    }
}
