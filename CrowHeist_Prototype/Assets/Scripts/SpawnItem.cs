using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    private ItemEventManager itemEventManager;

    public GameObject item;
    public Transform spawnPoint;

    [SerializeField] private int maxSpawnCount = 1;
    private int currentSpawnCount = 0;

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
        if (item == null || currentSpawnCount >= maxSpawnCount) return;
        currentSpawnCount++;
        Vector3 position = spawnPoint ? spawnPoint.position : transform.position + Vector3.up;
        Quaternion rotation = spawnPoint ? spawnPoint.rotation : Quaternion.identity;
        Instantiate(item, position, rotation);
    }
}
