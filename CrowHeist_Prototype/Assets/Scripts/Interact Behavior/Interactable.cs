using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour, IInteractable
{
    protected Outline _outline;
    protected bool _interact;
    protected GameObject _canvas;

    public Outline Outline => _outline;
    public GameObject realObject;
    public bool Interact => _interact;

    protected virtual void Awake()
    {
        if (realObject == null)
        {
            realObject = transform.parent != null ? transform.parent.gameObject : gameObject;
        }
        _outline = GetComponentInParent<Outline>();
        if (transform.parent.Find("KeyBind") is not null)
        {
            _canvas = transform.parent.Find("KeyBind").gameObject;
            _canvas.SetActive(false);
        }
    }

    public void SetOutline(bool enabled)
    {
        if (_outline != null)
        {
            _outline.enabled = enabled;
        }
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_canvas is not null)
            {
                _canvas.SetActive(true);
            }
            _outline.enabled = true;
            _interact = true;
        }
    }

    public virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_canvas is not null)
            {
                _canvas.SetActive(false);
            }

            _outline.enabled = false;
            _interact = false;
        }
    }
}
