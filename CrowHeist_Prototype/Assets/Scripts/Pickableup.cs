using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Pickable : MonoBehaviour, IPickupable
{
    private Rigidbody _rigidbody;
    private GameObject _item;
    public bool pickedUp = false;

    public GameObject Item => _item;

    private void Awake()
    {
        _item = gameObject;
        _rigidbody = GetComponent<Rigidbody>();
    }
    public void PickUP(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        // Adjust rotation to lay flat and face forward
        transform.localRotation = Quaternion.Euler(90f, 90f, 0f);

        _rigidbody.isKinematic = true;
        pickedUp = true;
    }


    public void Drop(Vector3 position)
    {
        transform.SetParent(null);
        transform.position = position;
        _rigidbody.isKinematic = false;
        pickedUp = false;
    }

    public void Use()
    {
        Debug.Log("Using " + gameObject.name);
    }
}
