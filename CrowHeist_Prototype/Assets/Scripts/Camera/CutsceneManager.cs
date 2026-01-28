using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public CreateCutscene doorOpenCutscene;
    bool doorOpenCutscenePlayed = false;
    public DoorOpen doorOpen;

    void Update()
    {
        if(GameManager.Score >= 3 && !doorOpenCutscenePlayed)
        {
            doorOpenCutscene.PlayCutscene();
            doorOpenCutscenePlayed = true;
            doorOpen.OpenDoor();
        }
    }
}
