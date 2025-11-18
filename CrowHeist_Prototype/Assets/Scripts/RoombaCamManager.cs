using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaCamManager : MonoBehaviour
{
    // Script created by Mark D. 11/09/2025
    // Handles the roomba break "cut scene"
    // switches between the cameras using a coroutine

    public Camera playerCam;
    public Camera dockCam;
    public Camera roombaCam;

    public float playerCamTime = 1f;
    public float dockCamTime = 3f;
    public float roombaDeactivateTime = 1f;
    public float roombaLightsOffTime = 1f;
    public float roombaCamTime = 3f;

    // references to deactivate and dispense - added 11/17/25
    public RoombAi roombAi;
    public RoombaDispense roombaDispense;
    public RoombaLightsOff roombaLightsOff;


    public void StartRoombaSequence()
    {
        StartCoroutine(RoombaSequence());
    }

    private IEnumerator RoombaSequence()
    {
        yield return new WaitForSeconds(playerCamTime);

        // Dock camera
        SetActiveCamera(dockCam);
        yield return new WaitForSeconds(dockCamTime);

        // Roomba camera
        SetActiveCamera(roombaCam);
        // call deactivate & pause before dispense - added 11/17/25
        roombAi.Deactivate();
        yield return new WaitForSeconds(roombaDeactivateTime);

        // call LightsOff method - added 11/17/25
        roombaLightsOff.LightsOff();
        yield return new WaitForSeconds(roombaLightsOffTime);

        // call dispense method - added 11/17/25
        roombaDispense.Dispense();
        yield return new WaitForSeconds(roombaCamTime);

        // Back to player camera
        SetActiveCamera(playerCam);
    }

    private void SetActiveCamera(Camera cam)
    {
        playerCam.gameObject.SetActive(false);
        dockCam.gameObject.SetActive(false);
        roombaCam.gameObject.SetActive(false);
        cam.gameObject.SetActive(true);
    }
}
