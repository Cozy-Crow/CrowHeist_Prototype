using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

// This class is responsible for managing all audio in the game, including one-shot and instance SFX, as well as ambience.
public class AudioManager : MonoBehaviour
{
    private Dictionary<string, EventInstance> eventInstances = new();
    public static AudioManager Instance { get; private set; }

    [SerializeField] private EventReference Ambience;
    private EventInstance AmbienceInstance;

    // This checks if there is only one instance of audio manager
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


    // Starts ambience intance using createinstance function
    private void InitializeAmbience(EventReference AmbienceReference)
    {
        AmbienceInstance = CreateInstance("Ambience", AmbienceReference);
        AmbienceInstance.start();
    }

    // Starts ambience on start
    private void Start()
    {
        InitializeAmbience(Ambience);
    }

    /// This is used for continous instances, which are created at the start of the game and played/updated when needed.
    /// ex) 
    ///  - looping sfx
    ///  - sfx with parameters that need to be updated
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

    // Set exsisting instance parameter by float value
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

    // Set exsisting instance parameter by label value
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
    
    /// <summary>
    /// Plays an instance SFX that have settable parameters, and does not release it
    /// <param name="name"> the name of the event instance </param>
    public void PlayInstance(string name)
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

    /// <summary>
    /// Play an instance SFX and release it immediately
    /// </summary>
    /// <param name="name"> the name of the event instance </param>
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

        eventInstances.Remove(key);
        instance.start();
        instance.release();
    }

    /// <summary>
    /// Plays a one-shot 3D sound effect at a specified world position.
    /// </summary>
    /// <param name="eventSFX">The event reference for the sound effect.</param>
    /// <param name="worldPos">The world position where the sound should be played.</param>
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
