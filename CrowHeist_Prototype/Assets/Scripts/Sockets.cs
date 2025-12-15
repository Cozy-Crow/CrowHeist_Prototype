using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sockets : MonoBehaviour
{
    [SerializeField] private SocketsType[] socketArr;
    private Dictionary<Enum_Sockets, Transform> socketDict = new();

    void Awake()
    {
        for(int i = 0; i < socketArr.Length; i++)
        {
            socketDict.Add(socketArr[i].socketType, socketArr[i].socketTransform);
        }
    }
}

[Serializable]
public class SocketsType
{
    public Enum_Sockets socketType;
    public Transform socketTransform;
}
