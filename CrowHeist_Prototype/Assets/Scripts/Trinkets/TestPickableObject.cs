using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPickableObject : Pickable
{
    [SerializeField] private Collider objectCollider;
    public override void PickUp(Transform parent)
    {
        rb.isKinematic = true;
        pickedUp = true;
        objectCollider.enabled = false;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
    }

    public override void Drop(Vector3 position)
    {
        rb.isKinematic = false;
        pickedUp = false;
        objectCollider.enabled = true;

        transform.SetParent(null);
        transform.position = position;
    }
}
