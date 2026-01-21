using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCamManager : MonoBehaviour
{
    // Script by Mark D. - created 12/06/2025
    // This script was created to replace RoombaCamManager script
    // It works with Cinemachine vitual cameras rather than normal cameras

    public CinemachineVirtualCamera playerCam;
    public CinemachineVirtualCamera dockCam;
    public CinemachineVirtualCamera roombaCam;
    public CinemachineVirtualCamera doorCam;

    public float activateDelayTime = 0.5f;
    public float activateTime = 3f;

    public float playerCamTime = 1f;
    public float dockCamTime = 3f;
    public float roombaDeactivateTime = 2f;
    public float roombaLightsOffTime = 0.5f;
    public float roombaCamTime = 3f;
    public float doorCamTime = 0.5f;
    public float doorOpenTime = 3f;


    public RoombAi roombAi;
    public RoombaDispense roombaDispense;
    public RoombaLightsOff roombaLightsOff;
    public DoorOpen doorOpen;

    public Rigidbody playerRb;

    private CinemachineVirtualCamera[] allCams;

    private void Awake()
    {
        allCams = new CinemachineVirtualCamera[] {playerCam, dockCam, roombaCam, doorCam};
        ActivateVcam(playerCam);
    }

    public void StartRoombaActivateSequence() =>
        StartCoroutine(RoombaActivateSequence());

    public void StartRoombaBreakSequence() =>
        StartCoroutine(RoombaBreakSequence());

    private IEnumerator RoombaActivateSequence()
    {
        yield return new WaitForSeconds(activateDelayTime);

        FreezePlayer();

        ActivateVcam(roombaCam);
        yield return new WaitForSeconds(activateTime);

        UnfreezePlayer();
        ActivateVcam(playerCam);
    }

    private IEnumerator RoombaBreakSequence()
    {
        FreezePlayer();
        yield return new WaitForSeconds(playerCamTime);

        // Dock camera
        ActivateVcam(dockCam);
        yield return new WaitForSeconds(dockCamTime);

        // Roomba cam + events
        ActivateVcam(roombaCam);

        roombAi.Deactivate();
        yield return new WaitForSeconds(roombaDeactivateTime);

        roombaLightsOff.LightsOff();
        yield return new WaitForSeconds(roombaLightsOffTime);

        roombaDispense.Dispense();
        yield return new WaitForSeconds(roombaCamTime);

        // Door camera + event
        ActivateVcam(doorCam);
        yield return new WaitForSeconds(doorCamTime);

        doorOpen.OpenDoor();
        yield return new WaitForSeconds(doorOpenTime);

        // Back to player
        ActivateVcam(playerCam);
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
        playerRb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    private void UnfreezePlayer()
    {
        playerRb.constraints = RigidbodyConstraints.None;
        playerRb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
