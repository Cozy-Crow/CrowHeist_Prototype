using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Mark D on 9/10/25 - Updated 2/13/26
// This script goes one object in the Abuela level to activate the roomba and trigger the cutscene
// This will go on every object that has a surface that crowley can climb on to activate roomba

public class SurfaceCheck : MonoBehaviour
{
    public CutsceneManager cutsceneManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RoombAi roomba = FindObjectOfType<RoombAi>();
            if (roomba != null)
            {
                roomba.Activate();
            }
            cutsceneManager.RoombaActivationCutscene();
        }
    }
}

