using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;


[RequireComponent(typeof(Rigidbody))]
public class Pickable : MonoBehaviour, IPickupable
{
    [SerializeField] private Enum_Sockets socketType;
    protected Rigidbody rb;
    public bool pickedUp = false;
    public bool _isDirty = false;
    public Controller2Point5D player;
    private AIEventManager aiEventManager;
    private ItemEventManager itemEventManager;
    public SpawnItem mySpawner;
    private bool hasBeenPickedUp;

    public Enum_Sockets SocketType {get => socketType;}

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player").GetComponent<Controller2Point5D>();
    }
    void Start()
    {
        itemEventManager = FindObjectOfType<ItemEventManager>();
        aiEventManager = FindObjectOfType<AIEventManager>();
        if (aiEventManager != null)
        {
            aiEventManager.e_makedirty.AddListener(OnObjectDirty);
        }
    }
    public virtual void PickUp(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        //MusicManager.SetParameterByName("ItemYes", 1);

        // Show visual for picked up item if it has not been picked up before
        // Could be used in tandem with narrative menu later
        // Need to rework later to make it specific to object types, this will trigger for every pickupable
        if(hasBeenPickedUp == false)
        {
            PickupVisualManager.Instance.PlayFirstPickupAnim(Item);
            hasBeenPickedUp = true;
        }

        if (this.tag == "Knife")
        {
            transform.localRotation = Quaternion.Euler(90f, 90f, 0f);
            Debug.Log("Knife Picked up");
        }
        else if (this.CompareTag("Paintbrush"))
        {
            transform.localRotation = Quaternion.Euler(45f, -90f, 0f);
            transform.localPosition = new Vector3(-1, -1, 0);
        }
        // added by Mark D. 11/09/2025 to fix dart rotation and jumping issue
        else if (this.CompareTag("Dart"))
        {
            transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        }
        // Adjust rotation to lay flat and face forward
        else
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Debug.Log("Non-Knife Picked up");
        }

        rb.isKinematic = true;
        pickedUp = true;

        if (player != null)
        {
            //Controller2Point5D playerController = player.GetComponent<Controller2Point5D>();
            if (_isDirty)
            {
                player.isDirty = true;
                aiEventManager.PlayerDirty(player.transform.position);
                Debug.Log("Player is dirty");
            }
        }

        //TooltipManager.Instance.ShowTooltip(tag);
    }
    
    public virtual void Drop(Vector3 position)
    {
        transform.SetParent(null);
        transform.position = position;

        if (this.CompareTag("HanoiDisc"))
        {
            transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        rb.isKinematic = false;
        pickedUp = false;
        MusicManager.SetParameterByName("ItemYes", 0);

        //TooltipManager.Instance.HideTooltip(tag);
    }

    public void Use()
    {
        Debug.Log("Using " + gameObject.name);
        //MusicManager.SetParameterByName("ItemYes", 0);
    }

    public void Consume()
    {
        player.ConsumeItem();
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
    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag ("Waypoint"))
        {
            SpawnItem spawner = other.GetComponentInParent<SpawnItem>();
            Debug.Log(spawner);
            if(spawner != null)
            {
                mySpawner = spawner;
                mySpawner.NotifyIfRemoved(this.gameObject);
            }
        }
    }
}

public enum Enum_Sockets
{
    HEAD,
    HAND
}