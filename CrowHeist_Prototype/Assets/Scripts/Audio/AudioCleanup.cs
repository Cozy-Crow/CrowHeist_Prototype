using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioCleanup : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.CleanUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
