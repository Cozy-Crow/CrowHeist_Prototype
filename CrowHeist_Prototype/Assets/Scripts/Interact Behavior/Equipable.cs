using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Equipable : MonoBehaviour
{
    private bool _isEquipped = false;
    private Interactable _interactable;
    private Rigidbody _rigidbody;

    protected virtual void Awake()
    {
        _rigidbody = GetComponentInParent<Rigidbody>();
        _interactable = GetComponentInChildren<Interactable>();
    }

    public virtual void Interact()
    {
        Debug.Log("Using " + transform.name);
    }

    public virtual void Equip(Transform parent)
    {
        Debug.Log("Equipping " + transform.name);
        _isEquipped = true;
        _interactable.Outline.enabled = false;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, -70);
        _rigidbody.isKinematic = true;

        // Set the layer to ignore interactable objects
        gameObject.layer = 7;
    }

    public virtual void UnEquip(Vector3 position)
    {
        _isEquipped = false;
        transform.SetParent(null);
        transform.position = position;
        _rigidbody.isKinematic = false;

        // Set the layer back to interactable objects
        gameObject.layer = 3;
    }
}
