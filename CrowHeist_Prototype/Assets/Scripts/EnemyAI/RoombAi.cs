using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using KinematicCharacterController.Examples;

[RequireComponent(typeof(NavMeshAgent))]
public class RoombAi : MonoBehaviour
{
    [Header("Roomba Settings")]
    [HideInInspector]
    [SerializeField] private NavMeshAgent agent;            // The navmesh agent for the roomba
    [SerializeField] private List<Transform> targets;            // The target points for the roomba to move to
    [SerializeField] private RoombaPathing pathing;         // The pathing method for the roomba
    [SerializeField] private float bufferDistance;          // The buffer distance for the roomba to reach the target
    [SerializeField] private float detectionRadius = 5f;    // Radius for detecting dirty objects
    [SerializeField] private LayerMask dirtyLayerMask;      // The layer mask to detect dirty objects

    [SerializeField] private Transform dock;

    private int currentTargetIndex = 0;                     // The current target index for the roomba
    private bool goingBackwards = false;                    // If the roomba is going backwards in the target array if the pathing is sequential
    private bool isDocked = true;                            // Flag to check if Roomba is docked
    private GameObject dirtyObject;                         // The object the roomba needs to clean
    private AIEventManager aiEventManager;

    private Vector3 dirtyItemLocation;


    //Clmanp property for the current target index
    public int CurrentTargetIndex
    {
        get => currentTargetIndex;
        set
        {
            currentTargetIndex = Mathf.Clamp(value, 0, targets.Count - 1);
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        aiEventManager = FindObjectOfType<AIEventManager>();
        if (aiEventManager != null)
        {
            aiEventManager.OnGroundObjectDirty.AddListener(ItemPath);
        }
        targets.Add(dock);
    }

    //Updates the pathing method
    void Update()
    {
        switch (pathing)
        {
            case RoombaPathing.Stationary:
                StationaryPath();
                break;
            case RoombaPathing.ItemTarget:
                ItemPath(dirtyItemLocation);
                break;
            case RoombaPathing.PlayerTarget:
                PlayerPath();
                break;
        }
    }

    //Roomba stays docked if it does not detect anything dirty
    private void StationaryPath()
    {
        agent.SetDestination(dock.position);
    }

    //Roomba goes to the location of a dirty item on the floor and destroys it
    private void ItemPath(Vector3 dirtyItemPosition)
    {
        dirtyItemLocation = dirtyItemPosition;
        agent.SetDestination(dirtyItemPosition);
    }

    //Roomba targets player holding a dirty object and makes the player drop the item if hit
    private void PlayerPath()
    {
        return;
    }

    // Enum for the pathing type
    public enum RoombaPathing
    {
        Stationary,
        ItemTarget,
        PlayerTarget
    }

    void OnTriggerEnter(Collider other)
    {
        var itemScript = other.transform.parent.GetComponent<Pickable>();
        if(other.GetComponent<Interactable>() != null && itemScript != null && itemScript._isDirty) 
        {
            Destroy(other);
        }
    }
}
