using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaCamFollow : MonoBehaviour
{
    public Transform roombaPos;
    public float offsetZ = 7.0f;
    public float offsetY = 3.0f;

    void LateUpdate()
    {
        Vector3 pos = transform.position;
        pos.x = roombaPos.position.x;
        pos.z = roombaPos.position.z - offsetZ;
        pos.y = roombaPos.position.y + offsetY;
        transform.position = pos;
    }
}
