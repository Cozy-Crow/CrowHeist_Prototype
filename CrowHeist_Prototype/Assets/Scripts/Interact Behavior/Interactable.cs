using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour, IInteractable
{
    protected Outline _outline;

    protected virtual void Awake()
    {
        _outline = GetComponentInParent<Outline>();
    }
    public virtual void Interact()
    {
        Debug.Log("Interacting with " + transform.name);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = false;
        }
    }
}
