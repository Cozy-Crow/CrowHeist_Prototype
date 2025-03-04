using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class Cawing : MonoBehaviour
{
    [SerializeField] private EventReference _cawingSfx;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            AudioManager.PlayOneShot(_cawingSfx);
            //RuntimeManager.PlayOneShot("event:/Crow");
        }
    }

    public void PlaySFX(string audioname)
    {
        switch (audioname)
        {
            case "caw":
                AudioManager.PlayOneShot(_cawingSfx);
                break;
        }

    }
}
