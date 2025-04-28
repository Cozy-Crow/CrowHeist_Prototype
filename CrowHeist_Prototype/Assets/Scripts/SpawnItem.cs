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
    }

    public void Spawn()
    {
        if (spawnItems.Count <= 0 || currentSpawnCount >= maxSpawnCount) return;

        Transform freePoint = null;
        foreach (Transform point in spawnPoints)
        {
            if (!activeSpawns.ContainsKey(point) || activeSpawns[point] == null)
            {
                freePoint = point;
                break;
            }
        }
        if (freePoint == null)
        {
            Debug.LogWarning("No Free Spawn Point");
            return;
        }
        currentSpawnCount++;
        Vector3 position = freePoint.position;
        Quaternion rotation = freePoint.rotation; 
        GameObject spawned = Instantiate(spawnItems[0], position, rotation);
        activeSpawns[freePoint] = spawned;
        
    }
}
