using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickupable
{
    public void PickUP(Transform parent);
    public void Drop(Vector3 position);
    public void Use();
}
