using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public void Interact(){}
    public virtual void OnTriggerEnter(Collider other){}
    public virtual void OnTriggerExit(Collider other) { }
}
