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
    public bool _isDirty = false;

    public GameObject Item => _item;

    private void Awake()
    {
        _item = gameObject;
        _rigidbody = GetComponent<Rigidbody>();
        //AIEventManager.instance.e_pickup.AddListener();
    }
    void Start()
    {
        AIEventManager aiEventManager = FindObjectOfType<AIEventManager>();
        if (aiEventManager != null)
        {
            aiEventManager.e_makedirty.AddListener(OnObjectDirty);
        }
    }
    public void PickUP(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;

        if(this.tag == "Knife")
        {
            transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
        }
        // Adjust rotation to lay flat and face forward
        else
        {
            transform.localRotation = Quaternion.Euler(0f,0f,0f);
        }

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

    ////TODO: Make this work
    //public bool PrintDirty()
    //{
    //    if (true)
    //    {
    //        AIEventManager.instance.e_playerdirty.Invoke();
    //    }

    //    return true;
    //}
    void OnObjectDirty()
    {
        _isDirty = true;
        Debug.Log("Dirty");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && _isDirty == false)
        {
            OnObjectDirty();
        }
    }


}
