using System;
using System.Collections.Generic;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

/// <summary>
/// This class is responsible for managing Crowley's sound effects. 
/// It holds references to one-shot and instance SFX,
/// providing methods to play them in the animation and set parameters in other scripts. 
/// </summary> 
public class CrowleySFX : MonoBehaviour
{
    // Use to serialize one-shot SFX in the inspector, which will be added to the dictionary for easy access by name
    [SerializeField] private List<SoundData> crowleyOneShotSFX;
    [SerializeField] private List<SoundData> crowleyInstanceSFX;

    // For faster lookup when playing one shot sfx
    private Dictionary<string, EventReference> soundDictionary = new();

    public void Start()
    {
        //Populates the dictionary with the key:name, value:one-shot SFX,
        foreach (SoundData sfx in crowleyOneShotSFX)
        {
            soundDictionary.Add(sfx.name, sfx.sound);
        }

        // Creates instances for all instance SFX, using the name as the key for later access
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
    
    // Use for animator to call instance SFX, which have settable parameters
    public void PlayInstanceOneShot(string instance)
    {
        AudioManager.Instance?.PlayInstanceOneShot(instance);
    }


    public void PlayInstance(string instance)
    {
        AudioManager.Instance?.PlayInstance(instance);
    }

    // Set instance parameter by float value
    public void SetInstanceFloatParam(string instance, string parameter, float value)
    {
        AudioManager.Instance?.SetInstanceFloatParam(instance, parameter, value);
    }

    // Set instance parameter by label value
    public void SetInstanceLabelParam(string instance, string parameter, string label)
    {
        AudioManager.Instance?.SetInstanceLabelParam(instance, parameter, label);
    }
}


[Serializable]
public struct SoundData
{
    public string _name;

    // Name property ensure consistent name formatting
    public string name
    {
        get => _name.Trim().Replace(" ", "").Replace("_", "").Replace("-", "").ToUpper();
    }
    public EventReference sound;
}
