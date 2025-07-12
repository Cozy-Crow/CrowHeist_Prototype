using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    //Reference the singleton

    private List<EventInstance> eventInstances;
    public static AudioManager Instance { get; private set; }

    [SerializeField] private EventReference Ambience;

    [SerializeField] private EventReference Music;
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

    //starts music intance using createinstance function
    private void InitializeMusic(EventReference MusicReference)
    {
        AmbienceInstance = CreateInstance(MusicReference);
        AmbienceInstance.start();
    }

    //starts ambience and music on start
    private void Start()
    {
        InitializeAmbience(Ambience);
        InitializeMusic(Music);
    }

    private void SetAdaptiveMusic ()
    {
        
    }

    /// <summary>
    /// Use to play oneshot sfx
    /// </summary>
    /// <param name="eventSFX"></param>
    public void PlayOneShot(EventReference eventSFX)
    {
        RuntimeManager.PlayOneShot(eventSFX);
    }
    /// <summary>
    /// This is used for continous instances, such as looping sfx or bg music
    /// </summary>
    /// <param name="eventSFX"></param>
    /// <returns></returns>
    public EventInstance CreateInstance(EventReference eventSFX)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventSFX);
        eventInstances.Add(instance);
        return instance;
    }

    // <summary>
    //This is to play oneshot SFX that need spatialization
    /// <param name="eventSFX"></param>
    /// <param name="worldPos"></param>
    public void PlayOneShot3D(EventReference eventSFX, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(eventSFX, worldPos);
    }

    private void CleanUp()
    {
        //stop and release any created instances
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
