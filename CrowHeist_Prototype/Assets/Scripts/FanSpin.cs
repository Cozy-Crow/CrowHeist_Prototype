using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSpin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // degrees per second
    private bool isOn = true;

    void Update()
    {
        if(isOn)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
        
    }

    public void ToggleFan()
    {
        isOn = !isOn;
    }
    public void SetFanState(bool on)
    {
        isOn = on;
    }

    public bool IsFanOn()
    {
        return isOn;
    }


}
