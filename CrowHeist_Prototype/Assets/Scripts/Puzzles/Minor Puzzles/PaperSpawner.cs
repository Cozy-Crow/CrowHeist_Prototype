using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaperSpawner : Interactable
{

    [SerializeField] private Transform paperSpawnPoint;
    [SerializeField] private GameObject paperPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    public override void TriggerInteraction(Pickable item)
    {
        Instantiate(paperPrefab, paperSpawnPoint.position, Quaternion.identity);
        //base.TriggerInteraction(item);
    }
}
