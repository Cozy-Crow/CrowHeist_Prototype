using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Numerics;

public class AudioManager : MonoBehaviour
{
    //Reference the singleton

    private List<EventInstance> eventInstances;
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

        eventInstances = new List<EventInstance>();
    }


    //starts ambience intance using createinstance function
    private void InitializeAmbience(EventReference AmbienceReference)
    {
        AmbienceInstance = CreateInstance(AmbienceReference);
        AmbienceInstance.start();
    }

    //starts ambience on start
    private void Start()
    {
        InitializeAmbience(Ambience);
    }

    /// This is used for continous instances, such as looping sfx
    public EventInstance CreateInstance(EventReference eventSFX)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventSFX);
        eventInstances.Add(instance);
        return instance;
    }

    //Plays oneshot SFX that do not need spatialization
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    //This is to play oneshot SFX that need spatialization
    public void PlayOneShot3D(EventReference eventSFX, UnityEngine.Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(eventSFX, worldPos);
    }

    //Stops and releases all created instances
    public void CleanUp()
    {
        foreach (EventInstance instance in eventInstances)
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
