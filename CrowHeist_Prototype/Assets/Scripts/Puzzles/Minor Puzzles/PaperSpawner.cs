using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using Unity.VisualScripting;
using UnityEngine;

public class PaperSpawner : Interactable
{

    [SerializeField] private Transform paperSpawnPoint;
    [SerializeField] private GameObject paperPrefab;
    [SerializeField] private Controller2Point5D characterController;
    private GameObject paper;
    
    // Start is called before the first frame update
    void Start()
    {
        //paper = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    public override void TriggerInteraction(Pickable item)
    {
        if (characterController.GetHeldItems().Count > 0)
        {
            characterController.Drop();
        }

        if (paper == null)
        {
            paper = Instantiate(paperPrefab, paperSpawnPoint.position, Quaternion.identity);
            characterController.Pickup(paper.GetComponentInChildren<Interactable>(), paper.GetComponent<IPickupable>());
        }
        else
        {
            Destroy(paper);
            paper = Instantiate(paperPrefab, paperSpawnPoint.position, Quaternion.identity);
            characterController.Pickup(paper.GetComponentInChildren<Interactable>(), paper.GetComponent<IPickupable>());
        }
        //base.TriggerInteraction(item);
    }
}
