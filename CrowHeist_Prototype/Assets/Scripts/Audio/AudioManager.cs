using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    //Reference the singleton
    private Dictionary<string, EventInstance> eventInstances = new();
    public static AudioManager Instance { get; private set; }

    [SerializeField] private EventReference Ambience;
    private EventInstance AmbienceInstance;

    //This checks if there is only one instance of audio manager
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    //starts ambience intance using createinstance function
    private void InitializeAmbience(EventReference AmbienceReference)
    {
        AmbienceInstance = CreateInstance("Ambience", AmbienceReference);
        AmbienceInstance.start();
    }

    //starts ambience on start
    private void Start()
    {
        InitializeAmbience(Ambience);
    }

    /// This is used for continous instances, such as looping sfx
    public EventInstance CreateInstance(string name,EventReference eventSFX)
    {
        string key = name
            .Trim()                          // Remove leading/trailing spaces
            .Replace(" ", "")                // Remove spaces
            .Replace("_", "")                // Remove underscores
            .Replace("-", "")                // Remove dashes
            .ToUpper();                      // Uppercase consistently

        Debug.Log("Creating instance for: " + key);

        EventInstance instance = RuntimeManager.CreateInstance(eventSFX);
        eventInstances.Add(key, instance);
        return instance;
    }

    public void SetInstanceFloatParam(string instance, string parameter, float value)
    {
        string key = instance
            .Trim()                          // Remove leading/trailing spaces
            .Replace(" ", "")                // Remove spaces
            .Replace("_", "")                // Remove underscores
            .Replace("-", "")                // Remove dashes
            .ToUpper();                      // Uppercase consistently

        eventInstances.TryGetValue(key, out EventInstance eventInstance);

        if (!eventInstance.isValid()){
            Debug.LogWarning("No instance found for: " + key);
            return;
        }

        eventInstance.setParameterByName(parameter, value);
    }

    public void SetInstanceLabelParam(string instance, string parameter, string label)
    {
        string key = instance
            .Trim()                          // Remove leading/trailing spaces
            .Replace(" ", "")                // Remove spaces
            .Replace("_", "")                // Remove underscores
            .Replace("-", "")                // Remove dashes
            .ToUpper();                      // Uppercase consistently

        eventInstances.TryGetValue(key, out EventInstance eventInstance);

        if (!eventInstance.isValid()){
            Debug.LogWarning("No instance found for: " + key);
            return;
        }

        eventInstance.setParameterByNameWithLabel(parameter, label);
    }

    //Plays oneshot SFX that do not need spatialization
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayInstanceOneShot(string name)
    {
        string key = name
            .Trim()                          // Remove leading/trailing spaces
            .Replace(" ", "")                // Remove spaces
            .Replace("_", "")                // Remove underscores
            .Replace("-", "")                // Remove dashes
            .ToUpper();                      // Uppercase consistently

        eventInstances.TryGetValue(key, out EventInstance instance);

        if (!instance.isValid()){
            Debug.LogWarning("No instance found for: " + key);
            return;
        }

        instance.start();
    }

    //This is to play oneshot SFX that need spatialization
    public void PlayOneShot3D(EventReference eventSFX, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(eventSFX, worldPos);
    }

    //Stops and releases all created instances
    public void CleanUp()
    {
        foreach (KeyValuePair<string, EventInstance> kvp in eventInstances)
        {
            kvp.Value.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            kvp.Value.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
