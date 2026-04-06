using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using KinematicCharacterController.Examples;

// Script by Mark D. - created 1/25/2026
// This script allows for easy creation of cutscenes
// Create and empty game object called "Blank Cutscene" and add this script to it
// Create Cinemachine virtual cameras for the cutscene and place them where you want in the scene
// Drag the virtual cameras into the inspector for the cutscene object and enter activation times
// Call the PlayCutscene method using the CutsceneManager script

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

    private CinemachineVirtualCamera[] allCams;

    private void Awake()
    {
        // Collect all vcams in scene (including children)
        allCams = FindObjectsOfType<CinemachineVirtualCamera>();
        ActivateVcam(playerCam);
        if(playerCam != null)
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

    public void FreezePlayer()
    {
        playerRb.constraints =
            RigidbodyConstraints.FreezePosition |
            RigidbodyConstraints.FreezeRotation;

        playerObject.SetCanInput(false);        
    }

    private void UnfreezePlayer()
    {
        // playerRb.constraints = RigidbodyConstraints.FreezeRotation;

        playerObject.SetCanInput(true);        

    }
}

