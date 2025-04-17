using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSwitch : MonoBehaviour
{
    private GameObject OnSwitch;
    private GameObject OffSwitch;
    [SerializeField] FanSpin fanBlades;
    [SerializeField] FanWindArea wind;

    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "OnPosition")
            {
                OnSwitch = child.gameObject;
                break;
            }
        }
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "OffPosition")
            {
                OffSwitch = child.gameObject;
                break;
            }
        }
    }

    public void ToggleSwitchOn()
    {
        if(!OnSwitch.activeSelf && OffSwitch.activeSelf)
        {
            OnSwitch.gameObject.SetActive(true);
            OffSwitch.gameObject.SetActive(false);
            fanBlades.SetFanState(true);

        }
    }

    public void ToggleSwitchOff()
    {
        if(OnSwitch.activeSelf && !OffSwitch.activeSelf)
        {
            OnSwitch.gameObject.SetActive(false);
            OffSwitch.gameObject.SetActive(true);
            fanBlades.ToggleFan();

        }
    }
}
