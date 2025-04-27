using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    private ItemEventManager itemEventManager;

    public List<GameObject> spawnItems = new List<GameObject>();
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
        if (spawnItems.Count <= 0 || currentSpawnCount >= maxSpawnCount) return;
        currentSpawnCount++;
        Vector3 position = spawnPoint ? spawnPoint.position : transform.position + Vector3.up;
        Quaternion rotation = spawnPoint ? spawnPoint.rotation : Quaternion.identity;
        if(this.CompareTag("VendingMachineDropper"))
        {
            Instantiate(spawnItems[0], position, rotation);

        }
    }
}
