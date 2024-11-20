using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EquipableBell : Equipable
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact()
    {
        Debug.Log("Ringing " + transform.name);
    }
}
