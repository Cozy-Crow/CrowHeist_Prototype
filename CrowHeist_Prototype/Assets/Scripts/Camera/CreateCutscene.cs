using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using KinematicCharacterController.Examples;

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
    Controller2Point5D playerObject;

    [Header("Intro Cutscene")]
    public bool isIntroCutscene = false;
    public float tutorialDelay = 0.2f;
    public UnityEvent onCutsceneComplete;

    private CinemachineVirtualCamera[] allCams;

    private void Awake()
    {
        allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        ActivateVcam(playerCam);
        if (playerCam != null)
            playerObject = playerRb.GetComponent<Controller2Point5D>();
    }

    public void PlayCutscene()
    {
        Debug.Log("PlayCutscene Method");
        StopAllCoroutines();
        StartCoroutine(CutsceneSequence());
    }

    private IEnumerator CutsceneSequence()
    {

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


        if (isIntroCutscene)
        {
            StartCoroutine(DelayedTutorial());
        }
    }

    private IEnumerator DelayedTutorial()
    {
        yield return new WaitForSeconds(tutorialDelay);
        onCutsceneComplete?.Invoke();
    }

    private void ActivateVcam(CinemachineVirtualCamera vcam)
    {
        foreach (var cam in allCams)
            cam.Priority = 0;

        vcam.Priority = 10;
    }

    public void FreezePlayer()
    {
        playerRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotation;

        playerObject.SetCanInput(false);
    }

    private void UnfreezePlayer()
    {
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;

        playerObject.SetCanInput(true);
    }
}