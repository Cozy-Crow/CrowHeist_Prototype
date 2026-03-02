using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class CutsceneManager : MonoBehaviour
{
    // Script by Mark D. - created 1/25/2026
    // This script is temporary to play my new easy-to-create cutscenes
    // It will be changed to work with unity events so cutscenes
    // can be triggered by a variety of conditions

    public CreateCutscene introCutscene;
    bool introCutscenePlayed = false;
    
    public CreateCutscene roombaActivationCutscene;
    bool roombaActivationCutscenePlayed = false;

    public CreateCutscene roombaBreakCutscene;
    bool roombaBreakCutscenePlayed = false;

    public CreateCutscene doorOpenCutscene;
    bool doorOpenCutscenePlayed = false;
    public DoorOpen doorOpen;

    public int coinThreshold;

    void Start()
    {
        introCutscene.PlayCutscene();
        introCutscenePlayed = true;
    }


    void Update()
    {
        if(GameManager.Score >= coinThreshold && !doorOpenCutscenePlayed)
        {
            doorOpenCutscene.PlayCutscene();
            doorOpenCutscenePlayed = true;
            doorOpen.OpenDoor();
        }
    }

    public void RoombaActivationCutscene()
    {
        Debug.Log("cutscene manager roomba activation cutscene");
        roombaActivationCutscene.PlayCutscene();
        roombaActivationCutscenePlayed = true;
    }

    public void RoombaBreakCutscene()
    {
        Debug.Log("cutscene manager roomba break cutscene");
        roombaBreakCutscene.PlayCutscene();
        roombaBreakCutscenePlayed = true;
    }
}
