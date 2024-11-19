using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour, IInteractable
{
    private Outline _outline;
    private Collider _collider;

    private void Awake()
    {
        _outline = GetComponentInParent<Outline>();
        _collider = GetComponent<Collider>();
    }
    public virtual void Interact()
    {
        Debug.Log("Interacting with " + transform.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _outline.enabled = false;
        }
    }
}
