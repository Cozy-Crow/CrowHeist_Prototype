using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public CreateCutscene exampleCutscene;
    bool exampleCutscenePlayed = false;

    void Update()
    {
        if(GameManager.Score >= 1 && !exampleCutscenePlayed)
        {
            exampleCutscene.PlayCutscene();
            exampleCutscenePlayed = true;
        }
    }
}
