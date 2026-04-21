using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ShakeOnPlayerHit : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] private float hitAmplitudeGain = 2;
    [SerializeField] private float hitFrequencyGain = 2;

    private CinemachineBasicMultiChannelPerlin noisePerlin;
    private void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        noisePerlin = vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    public void Shake()
    {
        noisePerlin.m_AmplitudeGain = hitAmplitudeGain;
        noisePerlin.m_FrequencyGain = hitFrequencyGain;
        StartCoroutine(Timer());

    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.2f);
        noisePerlin.m_FrequencyGain = 0f;
        noisePerlin.m_AmplitudeGain = 0f;
    }
}
