using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public Enum_Sockets SocketType {get;}
    public void PickUp(Transform parent);
    public void Drop(Vector3 position);
    public void Use();
    public void Consume();
}
