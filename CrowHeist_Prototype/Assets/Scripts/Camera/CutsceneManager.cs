using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public CreateCutscene doorOpenCutscene;
    bool doorOpenCutscenePlayed = false;

    void Update()
    {
        if(GameManager.Score >= 1 && !doorOpenCutscenePlayed)
        {
            doorOpenCutscene.PlayCutscene();
            doorOpenCutscenePlayed = true;
        }
    }
}
