using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDebugger : MonoBehaviour
{
      // Update is called once per frame
    void Update()
    {
        //Audio Changer
        if (Input.GetKeyDown(KeyCode.F8) && Input.GetKey(KeyCode.LeftShift))
        {
            float currentValue;
            MusicManager.Instance.CurrentMusicInstance.getParameterByName("TrinketsCollected", out currentValue);
            float newValue = currentValue += 1;
            MusicManager.SetParameterByName("TrinketsCollected", + newValue);
            MusicManager.Instance.CurrentMusicInstance.getParameterByName("TrinketsCollected", out float value1);
        }
        if (Input.GetKeyDown(KeyCode.F9) && Input.GetKey(KeyCode.LeftShift))
        {
            MusicManager.SetParameterByName("ItemYes", 1);
            MusicManager.Instance.CurrentMusicInstance.getParameterByName("ItemYes", out float value2);
            print("ItemYes: " + value2);
        }
         if (Input.GetKeyDown(KeyCode.F10) && Input.GetKey(KeyCode.LeftShift))
        {
            MusicManager.SetParameterByName("ItemYes", 0);
            MusicManager.Instance.CurrentMusicInstance.getParameterByName("ItemYes", out float value2);
            print("ItemYes: " + value2);
        }

        if (Input.GetKeyDown(KeyCode.F11) && Input.GetKey(KeyCode.LeftShift))
        {
            print(MusicManager.Instance.ActiveMusicName);
        }
    }
}

