using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    private ItemEventManager itemEventManager;
    private Dictionary<Transform, GameObject> activeSpawns = new Dictionary<Transform, GameObject>();

    public List<GameObject> spawnItems = new List<GameObject>();
    public List<Transform> spawnPoints = new List<Transform>();
    public Transform spawnPoint;

    [SerializeField] private int maxSpawnCount = 1;
    private int currentSpawnPoint = 0;
    private int itemCount = 0;

    private void Start()
    {
        itemEventManager = FindObjectOfType<ItemEventManager>();
        if (itemEventManager != null)
        {
            itemEventManager.e_spawnObj.AddListener(Spawn);
        }
        if (this.CompareTag("Cupboard"))
        {
            Spawn();
        }
    }

    private void Update()
    {
        if (spawnItems.Count > 0)
        {
            string targetTag = spawnItems[0].tag;
            itemCount = GameObject.FindGameObjectsWithTag(targetTag).Length;
        }
        
        
    }

    
    public void NotifyIfRemoved(GameObject item)
    {
        foreach (var pair in activeSpawns)
        {
            if (pair.Value == item)
            {
                activeSpawns[pair.Key] = null;
                if (itemCount < maxSpawnCount)
                {
                    StartCoroutine(SpawnAfterDelay(pair.Key));
                }
                break;
            }
        }
    }

    public IEnumerator SpawnAfterDelay(Transform point)
    {
        Debug.Log(point);
        yield return new WaitForSeconds(2f);
        SpawnAtPoint(point);
    }

    public void Spawn()
{
    if (spawnItems.Count <= 0) return;

    foreach (Transform point in spawnPoints)
    {
        // Only spawn if the point is free AND we haven't hit max count
        if ((!activeSpawns.ContainsKey(point) || activeSpawns[point] == null) && itemCount < maxSpawnCount)
        {
            Vector3 position = point.position;
            Quaternion rotation = point.rotation; 
            GameObject spawned = Instantiate(spawnItems[0], position, rotation);
            activeSpawns[point] = spawned;
        }
    }
}


    public void SpawnAtPoint(Transform point)
    {
        if (spawnItems.Count <= 0 || itemCount >= maxSpawnCount)
        {
            if (activeSpawns.ContainsKey(point) && activeSpawns[point] != null)
            {
                return;
            }
        }
        Vector3 position = point.position;
        Quaternion rotation = point.rotation;
        GameObject spawned = Instantiate(spawnItems[0], position, rotation);
        activeSpawns[point] = spawned;
    }
}
