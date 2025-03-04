using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    //Reference the singleton
    public static AudioManager Instance;
    /// <summary>
    /// Use to play oneshot sfx
    /// </summary>
    /// <param name="eventSFX"></param>
    public static void PlayOneShot(EventReference eventSFX)
    {
        RuntimeManager.PlayOneShot(eventSFX);
    }
    /// <summary>
    /// This is used for continous instances, such as looping sfx or bg music
    /// </summary>
    /// <param name="eventSFX"></param>
    /// <returns></returns>
    public static EventInstance CreateInstance(EventReference eventSFX)
    {
        EventInstance instance = RuntimeManager.CreateInstance(eventSFX);
        return instance;
    }
}
