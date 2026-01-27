using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXData : ScriptableObject
{
    [SerializeField] private AudioClip sfx;
    [SerializeField] private float minPitch;
    [SerializeField] private float maxPitch;
    [SerializeField] private float minVolume;
    [SerializeField] private float maxVolume;

    #region Properties
    public AudioClip SFX => sfx;
    public float MinPitch => minPitch;
    public float MaxPitch => maxPitch;
    public float MinVolume => minVolume;
    public float MaxVolume => maxVolume;
    #endregion
}
