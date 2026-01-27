using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaLightsOff : MonoBehaviour
{
    public List<GameObject> lights;

    public void LightsOff()
    {
        foreach (GameObject obj in lights)
        {
            obj.SetActive(false);
        }

    }
}
