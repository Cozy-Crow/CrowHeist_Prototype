using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class PlayCrowleySFX : MonoBehaviour
{
    [SerializeField] private List<SoundData> crowleySFX;
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
            .ToUpperInvariant();             // Uppercase consistently

            soundDictionary.Add(key, sfx.sound);
        }
    }
    public void PlaySFX(String sfx)
    {
        string key = sfx
        .Trim()                          // Remove leading/trailing spaces
        .Replace(" ", "")                // Remove spaces
        .Replace("_", "")                // Remove underscores
        .Replace("-", "")                // Remove dashes
        .ToUpperInvariant();             // Uppercase consistently
        
        soundDictionary.TryGetValue(key, out EventReference sound);
        if (sound.Equals(null))
        {
            Debug.LogWarning("Sound " + sfx + " not found in dictionary!");
            return;
        }

        AudioManager.Instance.PlayOneShot(sound);
    }
}

[Serializable]
public struct SoundData
{
    public string name;
    public EventReference sound;
}
