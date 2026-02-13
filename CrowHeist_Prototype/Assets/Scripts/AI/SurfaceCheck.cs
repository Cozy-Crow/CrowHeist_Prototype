using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Created by Mark D on 9/10/25
// This script goes on objects in the Abuela level so the roomba knows when to activate
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

