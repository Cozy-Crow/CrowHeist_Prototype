using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Crowley SFX")]
    [field: SerializeField] public EventReference crowCaw { get; private set; }
    [field: SerializeField] public EventReference footsteps { get; private set; }

    [field: Header("UI SFX")]
    [field: SerializeField] public EventReference itemPickup { get; private set; }
    public static FMODEvents instance { get; private set; }
   
    public void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in this scene");
        }
        instance = this;
    }
}

