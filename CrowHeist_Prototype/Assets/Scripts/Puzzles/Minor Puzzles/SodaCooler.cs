using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;

public class SodaCooler : Interactable
{


    [SerializeField] private Transform sodaSpawnPoint;
    [SerializeField] private GameObject sodaPrefab;
    [SerializeField] private GameObject lockSprite;
    
    private bool _canBeUnlocked = false;
    private bool _isUnlocked = false;

    public override void TriggerInteraction(Pickable item)
    {
        

        if (!_isUnlocked && item != null && item.gameObject.CompareTag("SodaKey"))
        {
            _isUnlocked = true;
            lockSprite.SetActive(false);
            item.Consume();
        }
        else if(_isUnlocked)
        {
            Instantiate(sodaPrefab, sodaSpawnPoint.position, Quaternion.identity);
        }


    }
}
