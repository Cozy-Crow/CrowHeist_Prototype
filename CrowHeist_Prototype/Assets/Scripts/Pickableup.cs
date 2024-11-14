using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Pickable : MonoBehaviour, IPickupable
{
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void PickUP(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _rigidbody.isKinematic = true;
    }

    public void Drop(Vector3 position)
    {
        transform.SetParent(null);
        transform.position = position;
        _rigidbody.isKinematic = false;
    }

    public void Use()
    {
        Debug.Log("Using " + gameObject.name);
    }
}
