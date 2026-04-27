using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsMenu : MonoBehaviour
{
    public bool IsOpen { get; private set; }

    public void OpenControls()
    {
        gameObject.SetActive(true);
        IsOpen = true;
    }

    public void CloseControls()
    {
        gameObject.SetActive(false);
        IsOpen = false;
    }

}
