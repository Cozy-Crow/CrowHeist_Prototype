using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class CrowleySFX : MonoBehaviour
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
    
    }

    public void PlayOneShot(string sfx)
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

    public void PlayInstanceOneShot(string instance)
    {
        AudioManager.Instance?.PlayInstanceOneShot(instance);
    }

    public void SetInstanceFloatParam(string instance, string parameter, float value)
    {
        AudioManager.Instance?.SetInstanceFloatParam(instance, parameter, value);
    }

    public void SetInstanceLabelParam(string instance, string parameter, string label)
    {
        AudioManager.Instance?.SetInstanceLabelParam(instance, parameter, label);
    }

}


[Serializable]
public struct SoundData
{
    public string name;
    public EventReference sound;
}
