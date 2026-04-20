using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;
using FMODUnity;


//[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UniqueID))]
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
    [Header("Audio")]
    [SerializeField] private EventReference ObjPuAudio;
    [SerializeField] public EventReference ObjThrowAudio;
    [SerializeField] private EventReference ObjLandAudio;
    private RoombAi RoombaAi;
    private EventReference roombaDetectSFX;

    // UniqueID reference for registry integration
    private UniqueID uniqueID;

    public Enum_Sockets SocketType { get => socketType; }

    
    public UniqueID UniqueID => uniqueID;

    // Gets or sets whether this item has been picked up before (for first-pickup detection).  
    public bool HasBeenPickedUp
    {
        get => hasBeenPickedUp;
        set => hasBeenPickedUp = value;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        uniqueID = GetComponent<UniqueID>();
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
         string eventpath = "event:/SFX/Roomba/RoombaDetect";
         roombaDetectSFX = RuntimeManager.PathToEventReference(eventpath);
    }
    public virtual void PickUp(Transform parent)
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        //MusicManager.SetParameterByName("ItemYes", 1);

        // Show visual for picked up item if it has not been picked up before
        // Uses registry to track first pickup state across save/load
        bool isFirstPickup = !hasBeenPickedUp;

        // Check registry for persistent first-pickup state
        if (PickupRegistry.Instance != null && uniqueID != null)
        {
            isFirstPickup = !PickupRegistry.Instance.HasBeenPickedUp(uniqueID.ID);
            PickupRegistry.Instance.MarkAsPickedUp(uniqueID.ID);
        }

        if (isFirstPickup)
        {
            // Check if this item type should show pickup animation
            bool shouldShowVisual = true;
            if (uniqueID != null && uniqueID.ItemData != null)
            {
                shouldShowVisual = uniqueID.ItemData.ShowOnFirstPickup;
            }

            if (shouldShowVisual && PickupVisualManager.Instance != null)
            {
                PickupVisualManager.Instance.PlayFirstPickupAnim(this.gameObject);
            }
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
        else if (this.CompareTag("Trinket"))
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

        AudioManager.Instance?.PlayOneShot(ObjPuAudio);
        //stops coin emitter SFX when object is picked up
         if (GetComponent<FMODUnity.StudioEventEmitter>() != null)
        {
            GetComponent<FMODUnity.StudioEventEmitter>().Stop();   
        }

        if (player != null)
        {
            Controller2Point5D playerController = player.GetComponent<Controller2Point5D>();
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

        //starts emitter SFX when player drops narrative object
        // if (GetComponent<FMODUnity.StudioEventEmitter>() != null)
       // {
        //    GetComponent<FMODUnity.StudioEventEmitter>().Play();   
        //}

        if (MusicManager.Instance != null)
        {
            MusicManager.SetParameterByName("ItemYes", 0);
        }
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
        RoombAi.Instance?.PlayRoombaDetectSFX();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && _isDirty == false)
        {
            OnObjectDirty();
        }

        //makes it so the land sfx doesnt trigger on player throwing it initially
        if (!other.CompareTag("Player") && rb.velocity.y < -0.5f)
        {
         AudioManager.Instance?.PlayOneShot3D(ObjLandAudio, transform.localPosition);
         Debug.Log("LandSFXPlayed"); 
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Waypoint"))
        {
            SpawnItem spawner = other.GetComponentInParent<SpawnItem>();
            Debug.Log(spawner);
            if (spawner != null)
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