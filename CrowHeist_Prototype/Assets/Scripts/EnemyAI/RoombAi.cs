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
    [SerializeField] private List<Transform> targets;       // The target points for the roomba to move to
    [SerializeField] private RoombaPathing pathing;         // The pathing method for the roomba
    [SerializeField] private float bufferDistance;          // The buffer distance for the roomba to reach the target
    [SerializeField] private float detectionRadius = 5f;    // Radius for detecting dirty objects
    [SerializeField] private LayerMask dirtyLayerMask;      // The layer mask to detect dirty objects

    [SerializeField] private Transform dock;

    private int currentTargetIndex = 0;                     // The current target index for the roomba
    private bool isDocked = true;                            // Flag to check if Roomba is docked
    private GameObject dirtyObject;                         // The object the roomba needs to clean
    private AIEventManager aiEventManager;

    private Vector3 dirtyItemLocation;
    private Vector3 playerLocation;

    public GameObject player;
    private Controller2Point5D playerController;

    // Roomba's status flags
    private bool playerIsDirty = false;
    private bool anyObjectDirty = false;

    // Clmanp property for the current target index
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
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        aiEventManager = FindObjectOfType<AIEventManager>();
        if (aiEventManager != null)
        {
            aiEventManager.OnGroundObjectDirty.AddListener(UpdateItemPath);
            aiEventManager.OnPlayerDirty.AddListener(UpdatePlayerPath);
        }
        targets.Add(dock);
    }

    // Updates the pathing method based on conditions
    void Update()
    {
        // If the player is dirty, prioritize going to the player
        if (playerIsDirty)
        {
            PlayerPath(playerLocation);
        }
        // If there are dirty objects, go to the next dirty object
        else if (anyObjectDirty)
        {
            ItemPath(dirtyItemLocation);
        }
        // If nothing else is dirty, go back to the dock
        else
        {
            StationaryPath();
        }

        // Continually check the player's position if they are dirty
        if (playerIsDirty)
        {
            PlayerPath(player.transform.position);
        }
    }

    // Roomba stays docked if it does not detect anything dirty
    private void StationaryPath()
    {
        // Check if there are any dirty objects left
        if (!anyObjectDirty)
        {
            agent.SetDestination(dock.position);
        }
    }

    // Roomba goes to the location of a dirty item on the floor and destroys it
    private void ItemPath(Vector3 dirtyItemPosition)
    {
        //dirtyItemLocation = dirtyItemPosition;
        agent.SetDestination(dirtyItemPosition);

        // After reaching the dirty object, check if there's another one
        StartCoroutine(WaitAndCheckForMoreDirtyObjects());
    }

    // Roomba targets player holding a dirty object and makes the player drop the item if hit
    private void PlayerPath(Vector3 playerPosition)
    {
        //playerLocation = playerPosition; 
        agent.SetDestination(playerPosition);
    }

    // Enum for the pathing type
    public enum RoombaPathing
    {
        Stationary,
        ItemTarget,
        PlayerTarget
    }

    // Event listener for a dirty object
    private void UpdateItemPath(Vector3 dirtyObjectPosition)
    {
        anyObjectDirty = true;
        dirtyItemLocation = dirtyObjectPosition;
        //agent.SetDestination(dirtyObjectPosition);
    }

    // Event listener for player dirty state
    private void UpdatePlayerPath(Vector3 playerPosition)
    {
        playerIsDirty = true;
        playerLocation = playerPosition;
        //agent.SetDestination(playerPosition);
    }

    // Wait for Roomba to finish cleaning and check if more dirty objects are available
    private IEnumerator WaitAndCheckForMoreDirtyObjects()
    {
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= bufferDistance);

        // Check if there are any remaining dirty objects on the ground
        anyObjectDirty = false;
        Collider[] dirtyObjects = Physics.OverlapSphere(transform.position, detectionRadius, dirtyLayerMask);

        foreach (Collider obj in dirtyObjects)
        {
            // If a dirty object is found, go to it
            if (obj.GetComponent<Pickable>() != null)
            {
                anyObjectDirty = true;
                dirtyItemLocation = obj.transform.position;
                agent.SetDestination(dirtyItemLocation);
                break;
            }
        }

        // If no dirty objects are found, go back to the dock
        if (!anyObjectDirty)
        {
            StationaryPath();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Get the parent transform
        Transform parentTransform = other.transform.parent;

        // Check if parentTransform is null before proceeding
        if (parentTransform != null)
        {
            // Try to get the Pickable component from the parent
            var itemScript = parentTransform.GetComponent<Pickable>();

            // Check if the component exists and if the object is dirty
            if (other.GetComponent<Interactable>() != null && itemScript != null && itemScript._isDirty) 
            {
                Destroy(parentTransform.gameObject); // Destroy the parent GameObject if conditions are met
            }
        }

        // Handle the player dropping the held object
        if (other.CompareTag("Player") && playerController.heldObject != null)
        {
            playerController.Drop();
            playerIsDirty = false;  // Stop chasing the player when the object is dropped
        }
    }

    // Reset dirty states when cleaning is complete
    private void ResetDirtyState()
    {
        playerIsDirty = false;
        anyObjectDirty = false;
    }
}
