using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class PlayCrowleySFX : MonoBehaviour
{
    [SerializeField] private List<SoundData> crowleyOneShotSFX;
    [SerializeField] private List<SoundData> crowleyInstanceSFX;
    private Dictionary<string, EventReference> soundDictionary = new();
    public void Start()
    {
        foreach (SoundData sfx in crowleyOneShotSFX)
        {
            string key = sfx.name
            .Trim()                          // Remove leading/trailing spaces
            .Replace(" ", "")                // Remove spaces
            .Replace("_", "")                // Remove underscores
            .Replace("-", "")                // Remove dashes
            .ToUpper();             // Uppercase consistently

            soundDictionary.Add(key, sfx.sound);
        }

        foreach (SoundData sfx in crowleyInstanceSFX)
        {
            AudioManager.Instance?.CreateInstance(sfx.name, sfx.sound);
        }
    }

    public void Update()
    {
        //Debug.Log("Hi");
        // if (Input.GetKey(KeyCode.Q))
        // {
        //     AudioManager.Instance?.PlayOneShotWithParameter(Footstep, "WalkRun", 1f);
        //     Debug.Log("Run");
        //}
        // else
        // {  
        //     // AudioManager.Instance?.PlayOneShotWithParameter(Footstep, "WalkRun", 0f); 
        //     // //AudioManager.Instance?.PlayOneShotWithParameter(Footstep, "WalkRun", -1f, "Walk"); 
        //     // Debug.Log("Walk");
        // }
    }

    public void PlayOneShot(String sfx)
    {
        string key = sfx
        .Trim()                          // Remove leading/trailing spaces
        .Replace(" ", "")                // Remove spaces
        .Replace("_", "")                // Remove underscores
        .Replace("-", "")                // Remove dashes
        .ToUpper();                      // Uppercase consistently
        
        soundDictionary.TryGetValue(key, out EventReference sound);

        if (sound.Equals(null))
        {
            Debug.LogWarning("Sound " + sfx + " not found in dictionary!");
            return;
        }

        AudioManager.Instance?.PlayOneShot(sound);      
    }

    public void PlayInstanceOneShot(String instance)
    {
        AudioManager.Instance?.PlayInstanceOneShot(instance);
    }

    // private void OnTriggerEnter(Collider collider)
    // {
    //     if (collider.tag.Equals("MetalFootstep"))
    //     {
    //         print("METAL");
    //         footstepInstance.setParameterByNameWithLabel("Surface", "Metal");
    //     }
    //     else if (collider.tag.Equals("WoodFootstep"))
    //     {
    //         print("WOOD");
    //         footstepInstance.setParameterByNameWithLabel("Surface", "Wood");
    //     }
    //     else if (collider.tag.Equals("CarpetFootstep"))
    //     {
    //         print("Carpet");
    //         footstepInstance.setParameterByNameWithLabel("Surface", "Carpet");
    //     }
    //     else
    //     {
    //         footstepInstance.setParameterByNameWithLabel("Surface", "Generic");
    //         print("GENERIC");
    //     }
    // }
}


[Serializable]
public struct SoundData
{
    public string name;
    public EventReference sound;
}
