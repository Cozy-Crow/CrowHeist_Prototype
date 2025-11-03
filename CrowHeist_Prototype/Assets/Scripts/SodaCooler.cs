using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;

public class SodaCooler : Interactable
{


    [SerializeField] private Transform sodaSpawnPoint;
    [SerializeField] private GameObject sodaPrefab;
    
    private bool _canBeUnlocked = false;
    private bool _isUnlocked = false;
    
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
        

        if (!_isUnlocked && item != null && item.gameObject.CompareTag("SodaKey"))
        {
            _isUnlocked = true;
            item.Consume();
        }
        else if(_isUnlocked)
        {
            Instantiate(sodaPrefab, sodaSpawnPoint.position, Quaternion.identity);
        }


    }
}
