using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour, IInteractable
{
    protected Outline _outline;
    protected bool _interact;

    public Outline Outline => _outline;
    public bool Interact => _interact;

    protected virtual void Awake()
    {
        _outline = GetComponentInParent<Outline>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = true;
            _interact = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = false;
            _interact = false;
        }
    }
}
