using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using KinematicCharacterController.Examples;

[RequireComponent(typeof(NavMeshAgent))]
public class RoombAi : MonoBehaviour
{
    [Header("Roomba Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private RoombaPathing pathing;
    [SerializeField] private float bufferDistance = 0.5f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask dirtyLayerMask;
    [SerializeField] private Transform dock;

    private int currentTargetIndex = 0;
    private bool isDocked = true;
    private GameObject dirtyObject;
    private List<Pickable> allDirtyObjects = new List<Pickable>();
    private AIEventManager aiEventManager;

    private Vector3 dirtyItemLocation;
    private Vector3 playerLocation;

    public GameObject player;
    private Controller2Point5D playerController;

    private bool playerIsDirty = false;
    private bool anyObjectDirty = false;

    public int CurrentTargetIndex
    {
        get => currentTargetIndex;
        set => currentTargetIndex = Mathf.Clamp(value, 0, targets.Count - 1);
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        aiEventManager = FindObjectOfType<AIEventManager>();
        if (aiEventManager != null)
        {
            aiEventManager.OnGroundObjectDirty.AddListener(UpdateItemPath);
            aiEventManager.OnPlayerDirty.AddListener(UpdatePlayerPath);
        }

        if (!targets.Contains(dock))
            targets.Add(dock);
    }

    private void Update()
    {
        HandleDirtyItemCollection(); // Keep list of dirty objects updated

        // Prioritize player if dirty and holding something
        if (playerIsDirty && playerController.heldObject != null)
        {
            PlayerPath(player.transform.position);
        }
        else if (allDirtyObjects.Count > 0)
        {
            anyObjectDirty = true;
            Pickable nearest = allDirtyObjects[0]; // already sorted by distance
            dirtyItemLocation = nearest.transform.position;
            ItemPath(dirtyItemLocation);
        }
        else
        {
            anyObjectDirty = false;
            StationaryPath();
        }
    }

    private void StationaryPath()
    {
        agent.SetDestination(dock.position);
    }

    private void ItemPath(Vector3 targetPos)
    {
        if (Vector3.Distance(agent.destination, targetPos) > 0.1f)
        {
            agent.SetDestination(targetPos);
        }

        StartCoroutine(WaitAndCheckForMoreDirtyObjects());
    }

    private void PlayerPath(Vector3 playerPosition)
    {
        if (Vector3.Distance(agent.destination, playerPosition) > 0.1f)
        {
            agent.SetDestination(playerPosition);
        }
    }

    private void UpdateItemPath(Vector3 dirtyObjectPosition)
    {
        anyObjectDirty = true;
        dirtyItemLocation = dirtyObjectPosition;
    }

    private void UpdatePlayerPath(Vector3 playerPosition)
    {
        playerIsDirty = true;
        playerLocation = playerPosition;
    }

    private IEnumerator WaitAndCheckForMoreDirtyObjects()
    {
        yield return new WaitUntil(() => !agent.pathPending && agent.remainingDistance <= bufferDistance);

        HandleDirtyItemCollection();

        if (allDirtyObjects.Count > 0)
        {
            anyObjectDirty = true;
            dirtyItemLocation = allDirtyObjects[0].transform.position;
            agent.SetDestination(dirtyItemLocation);
        }
        else
        {
            anyObjectDirty = false;
            StationaryPath();
        }
    }

    private void HandleDirtyItemCollection()
    {
        allDirtyObjects = FindObjectsOfType<Pickable>()
            .Where(obj => obj._isDirty && !obj.transform.IsChildOf(player.transform))
            .OrderBy(obj => Vector3.Distance(transform.position, obj.transform.position))
            .ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform parentTransform = other.transform.parent;

        if (parentTransform != null)
        {
            var itemScript = parentTransform.GetComponent<Pickable>();

            if (other.GetComponent<Interactable>() != null && itemScript != null && itemScript._isDirty)
            {
                Destroy(parentTransform.gameObject);
                HandleDirtyItemCollection(); // Update list after removal
            }
        }

        if (other.CompareTag("Player") && playerController.heldObject != null)
        {
            playerController.Drop();
            playerIsDirty = false;
        }
    }

    private void ResetDirtyState()
    {
        playerIsDirty = false;
        anyObjectDirty = false;
    }

    public enum RoombaPathing
    {
        Stationary,
        ItemTarget,
        PlayerTarget
    }
}
