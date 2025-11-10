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
    public float roombaCamTime = 3f;

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
