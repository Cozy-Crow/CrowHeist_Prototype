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

    public Transform GetSockets(Enum_Sockets socketType)
    {
        if(socketDict.TryGetValue(socketType, out Transform transform))
        {
            return transform;
        }
        else
        {
            Debug.LogWarning("Enum_SocketType not added to the dictionary");
            return null;
        }        
    }
}

[Serializable]
public class SocketsType
{
    public Enum_Sockets socketType;
    public Transform socketTransform;
}
