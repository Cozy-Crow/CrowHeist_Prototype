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
    private int currentSpawnCount = 0;
    private int currentSpawnPoint = 0;

    private void Start()
    {
        itemEventManager = FindObjectOfType<ItemEventManager>();
        if (itemEventManager != null)
        {
            itemEventManager.e_spawnObj.AddListener(Spawn);
        }
        if(this.CompareTag("Cupboard"))
        {
            Spawn();
        }
    }
    
    public void NotifyIfRemoved(GameObject item)
    {
        foreach(var pair in activeSpawns)
        {
            if (pair.Value == item)
            {
                activeSpawns[pair.Key] = null;
                currentSpawnCount--;
                if(currentSpawnCount < maxSpawnCount)
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

        //Transform freePoint = null;
        foreach (Transform point in spawnPoints)
        {
            if (!activeSpawns.ContainsKey(point) || activeSpawns[point] == null)
            {
                if (currentSpawnCount >= maxSpawnCount) return;
            }
            Vector3 position = point.position;
            Quaternion rotation = point.rotation; 
            GameObject spawned = Instantiate(spawnItems[0], position, rotation);
            activeSpawns[point] = spawned;
            currentSpawnCount++;

        }
        
    }

    public void SpawnAtPoint(Transform point)
    {
        if (spawnItems.Count <= 0 || currentSpawnCount >= maxSpawnCount)
        {
            if(activeSpawns.ContainsKey(point) && activeSpawns[point] != null)
            {
                return;
            }
        }
        Vector3 position = point.position;
        Quaternion rotation = point.rotation; 
        GameObject spawned = Instantiate(spawnItems[0], position, rotation);
        activeSpawns[point] = spawned;
        currentSpawnCount++;
    }
}
