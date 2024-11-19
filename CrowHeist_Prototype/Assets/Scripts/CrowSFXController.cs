using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowSFXController : MonoBehaviour
{
    [SerializeField] private SFXData[] _sfx;
    private AudioSource _audioSource;
    private Dictionary<string, SFXData> _sfxDictionary = new Dictionary<string, SFXData>();

    private void Awake()
    {
        _audioSource = GetComponentInParent<AudioSource>();
        foreach (var sfx in _sfx)
        {
            _sfxDictionary.Add(sfx.name, sfx);
        }
    }

    public void PlaySFXOneShot(string sfxName)
    {
        if (_sfxDictionary.ContainsKey(sfxName))
        {
            _audioSource.PlayOneShot(_sfxDictionary[sfxName].SFX);
        }else
        {
            Debug.LogWarning("SFX name not found");
        }
    }

    public void PlaySFXWithParameters(string sfxName)
    {
        float pitch = Random.Range(_sfxDictionary[sfxName].MinPitch, _sfxDictionary[sfxName].MaxPitch);
        float volume = Random.Range(_sfxDictionary[sfxName].MinVolume, _sfxDictionary[sfxName].MaxVolume);

        _audioSource.pitch = pitch;

        if (_sfxDictionary.ContainsKey(sfxName))
        {
            _audioSource.clip = _sfxDictionary[sfxName].SFX;
            _audioSource.PlayOneShot(_sfxDictionary[sfxName].SFX, volume);
        }else
        {
            Debug.LogWarning("SFX name not found");
        }

        _audioSource.pitch = 1;
    }
}
