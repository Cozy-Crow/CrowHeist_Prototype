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
    [SerializeField] private List<SoundData> crowleySFX;
    private EventInstance footstepInstance;
    private Dictionary<string, EventReference> soundDictionary = new();
    public void Awake()
    {
        foreach (SoundData sfx in crowleySFX)
        {
            string key = sfx.name
            .Trim()                          // Remove leading/trailing spaces
            .Replace(" ", "")                // Remove spaces
            .Replace("_", "")                // Remove underscores
            .Replace("-", "")                // Remove dashes
            .ToUpper();             // Uppercase consistently

            soundDictionary.Add(key, sfx.sound);
        }

        // Debug.Log("Loaded " + soundDictionary.Count + " Crowley SFX into dictionary.");
        // Debug.Log("Sounds: " + string.Join(", ", soundDictionary.Keys));

        
    }


    public void PlaySFX(String sfx)
    {
        string key = sfx
        .Trim()                          // Remove leading/trailing spaces
        .Replace(" ", "")                // Remove spaces
        .Replace("_", "")                // Remove underscores
        .Replace("-", "")                // Remove dashes
        .ToUpper();             // Uppercase consistently
        
        soundDictionary.TryGetValue(key, out EventReference sound);
        if (sound.Equals(null))
        {
            Debug.LogWarning("Sound " + sfx + " not found in dictionary!");
            return;
        }
        if (AudioManager.Instance != null)
        { 
            footstepInstance = AudioManager.Instance.CreateInstance(sound);
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
              footstepInstance.setParameterByName("WalkRun", 1);  
            }
            else
            {
             footstepInstance.setParameterByName("WalkRun", 0);   
            } 

            footstepInstance.start();
            footstepInstance.release();
        }         
    }
        private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("MetalFootstep"))
        {
            print("METAL");
            footstepInstance.setParameterByNameWithLabel("Surface", "Metal");
        }
        else if (collider.tag.Equals("WoodFootstep"))
        {
            print("WOOD");
            footstepInstance.setParameterByNameWithLabel("Surface", "Wood");
        }
        else
        {
            footstepInstance.setParameterByNameWithLabel("Surface", "Generic");
            print("GENERIC");
        }
    }
}


[Serializable]
public struct SoundData
{
    public string name;
    public EventReference sound;
}
