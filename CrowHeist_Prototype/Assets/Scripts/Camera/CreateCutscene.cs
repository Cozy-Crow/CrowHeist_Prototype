using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[System.Serializable]
public class CutsceneCameraSwitch
{
    public CinemachineVirtualCamera virtualCamera;
    public float activeTime = 1f;
}

public class CreateCutscene : MonoBehaviour
{
    [Header("Player Camera")]
    public CinemachineVirtualCamera playerCam;
    public float startDelay = 0.5f;

    [Header("Cutscene Cameras")]
    public List<CutsceneCameraSwitch> cameraSwitches = new();

    [Header("Optional")]
    public Rigidbody playerRb;

    private CinemachineVirtualCamera[] allCams;

    private void Awake()
    {
        // Collect all vcams in scene (including children)
        allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        ActivateVcam(playerCam);
    }

    public void PlayCutscene()
    {
        Debug.Log("PlayCutscene Method");
        StopAllCoroutines();
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {
        Debug.Log("Cutscene Coroutine");

        if (playerRb != null)
            FreezePlayer();

        yield return new WaitForSeconds(startDelay);

        foreach (var camSwitch in cameraSwitches)
        {
            if (camSwitch.virtualCamera == null)
                continue;

            ActivateVcam(camSwitch.virtualCamera);
            yield return new WaitForSeconds(camSwitch.activeTime);
        }

        ActivateVcam(playerCam);

        if (playerRb != null)
            UnfreezePlayer();
    }

    private void ActivateVcam(CinemachineVirtualCamera vcam)
    {
        foreach (var cam in allCams)
            cam.Priority = 0;

        vcam.Priority = 10;
    }

    private void FreezePlayer()
    {
        playerRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotation;
    }

    private void UnfreezePlayer()
    {
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}

