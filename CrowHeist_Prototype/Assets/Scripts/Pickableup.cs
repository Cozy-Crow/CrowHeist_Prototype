using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;


[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour, IPickupable
{
    private Rigidbody _rigidbody;
    private GameObject _item;
    public bool pickedUp = false;
    public bool _isDirty = false;
    public GameObject player;
    public GameObject Item => _item;
    private AIEventManager aiEventManager;


    private void Awake()
    {
        _item = gameObject;
        _rigidbody = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");
    }
    void Start()
    {
        aiEventManager = FindObjectOfType<AIEventManager>();
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

        if(player != null)
        {
            Controller2Point5D playerController = player.GetComponent<Controller2Point5D>();
            if (_isDirty)
            {
                playerController._isDirty = true;
                aiEventManager.PlayerDirty(player.transform.position);
                Debug.Log("Player is dirty");
            }
        }
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

    void OnObjectDirty()
    {
        _isDirty = true;
        aiEventManager.GroundItemDirty(transform.position);
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
