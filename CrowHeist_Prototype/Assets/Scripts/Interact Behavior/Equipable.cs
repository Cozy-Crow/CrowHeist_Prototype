using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Equipable : Interactable
{
    private bool _isEquipped = false;
    private Rigidbody _rigidbody;

    protected override void Awake()
    {
        base.Awake();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }

    public override void Interact()
    {
        Debug.Log("Using " + transform.name);
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isEquipped)
        {
            _outline.enabled = true;
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !_isEquipped)
        {
            _outline.enabled = false;
        }
    }

    public void Equip(Transform parent)
    {
        Debug.Log("Equipping " + transform.name);
        _isEquipped = true;
        _outline.enabled = false;
        transform.parent.parent = parent;
        transform.parent.localPosition = Vector3.zero;
        transform.parent.localRotation = Quaternion.Euler(0, 0, -70);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void UnEquip(Vector3 position)
    {
        _isEquipped = false;
        transform.parent.SetParent(null);
        transform.parent.position = position;
        _rigidbody.isKinematic = false;
    }
}
