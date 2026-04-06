using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;
using System.Linq;
using KinematicCharacterController.Examples;
using FMOD;

[RequireComponent(typeof(NavMeshAgent))]
public class RoombAi : MonoBehaviour
{

    public static RoombAi Instance;

    [Header("Roomba Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private RoombaPathing pathing;
    [SerializeField] private float bufferDistance = 0.5f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask dirtyLayerMask;
    [SerializeField] private Transform dock;

    //Patrol Mode for both rooms - added 1/29/25 by Mark D.
    [SerializeField] private List<Transform> patrolPoints_Room1;
    [SerializeField] private List<Transform> patrolPoints_Room2;
    //active patrol points (one of the lists above will be assigned)
    private List<Transform> patrolPoints;

    private int currentPatrolIndex = 0;
    public bool isActivated = false;
    public bool isBroken = false;

    private int currentTargetIndex = 0;
    private bool isDocked = true;
    private GameObject dirtyObject;
    private List<Transform> allDirtyObjects;
    private AIEventManager aiEventManager;

    private Vector3 dirtyItemLocation;
    private Vector3 playerLocation;

    public GameObject player;
    private Controller2Point5D playerController;

    private bool playerIsDirty = false;
    private bool anyObjectDirty = false;

    [Header("Attack Door Sequence")]
    [SerializeField] private RoombaAttackDoor roombaAttackDoor;
    [SerializeField] private float attackDoorSpeed = 15f;
    [SerializeField] private float circleRadius = 1f;
    [SerializeField] private float spinDuration = 2f;
    private bool isInAttackDoorSequence = false;

    [Header("Audio")]
    [SerializeField] public EventReference roombaDetect;
    [SerializeField] public EventReference roombaOn;
    [SerializeField] public EventReference roombaOff;
    [SerializeField] public EventReference roombaEat;
    [SerializeField] public EventReference roombaMovement;
    [SerializeField] public EventReference damageCaw;
    public StudioEventEmitter roombaEmitter;


    //For roomba activation cutscene - added 12/2/25 by Mark D.
    // public RoombaCamManager roombaCamManager;
    public VirtualCamManager virtualCamManager;

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

        // if (!targets.Contains(dock))
        //     targets.Add(dock);

        patrolPoints = patrolPoints_Room1;
        
        //checks if audiomanager exists, creates roombamovementinstance
        if(AudioManager.Instance != null)
        {  
         roombaEmitter = GetComponent<FMODUnity.StudioEventEmitter>();
         roombaEmitter.SetParameter("RoombaOnOff", 0);
        }

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        HandleDirtyItemCollection(); // Keep list of dirty objects updated

        if (isInAttackDoorSequence) return;

        // check if Roomba has been activated before movement logic - edited by Mark D. 9/10/25
        if(isActivated)
        {
            // Prioritize player if dirty and holding something
            if (playerIsDirty && playerController.heldObject != null)
            {
                PlayerPath(player.transform.position);
            }
            else if (allDirtyObjects.Count > 0)
            {
                anyObjectDirty = true;
                Transform nearest = allDirtyObjects[0];
                dirtyItemLocation = nearest.transform.position;
                ItemPath(dirtyItemLocation);
            }
            else
            {
                anyObjectDirty = false;
                Patrol();
            }
        }
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
            Patrol();
        }
    }

    private void HandleDirtyItemCollection()
    {
        allDirtyObjects = new List<Transform>();

        // Add dirty pickables
        var dirtyPickables = FindObjectsOfType<Pickable>()
            .Where(obj => obj._isDirty && !obj.transform.IsChildOf(player.transform))
            .Select(obj => obj.transform);

        allDirtyObjects.AddRange(dirtyPickables);

        // Add puddles
        //var puddles = GameObject.FindGameObjectsWithTag("Puddle")
          //  .Select(obj => obj.transform);

        //allDirtyObjects.AddRange(puddles);

        // Sort by distance to this Roomba
        allDirtyObjects = allDirtyObjects
            .OrderBy(obj => Vector3.Distance(transform.position, obj.position))
            .ToList();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puddle") && other != null)
        {
            UnityEngine.Debug.Log(other);
            Destroy(other.gameObject);
            HandleDirtyItemCollection();
        }

        Transform parentTransform = other.transform.parent;

        if (parentTransform != null)
        {
            var itemScript = parentTransform.GetComponent<Pickable>();

            if (other.GetComponent<Interactable>() != null && itemScript != null && itemScript._isDirty)
            {
                RespawnObject respawnComp = parentTransform.GetComponentInChildren<RespawnObject>();
                if (respawnComp != null)
                {
                    respawnComp.Respawn();
                }
                else
                {
                    Destroy(parentTransform.gameObject);
                }
                AudioManager.Instance?.PlayOneShot(roombaEat);
                HandleDirtyItemCollection();
            }

        }

        if (other.CompareTag("Player") && playerController.heldObject != null)
        {
            playerController.Drop();
            playerController._pickUpsList.Clear();
            playerIsDirty = false;
            Vector3 knockbackDir = (other.transform.position - transform.position).normalized;
            knockbackDir.y = 0f; 
            float knockbackForce = 10f;
            playerController.ApplyKnockback(knockbackDir,knockbackForce);
            AudioManager.Instance?.PlayOneShot(damageCaw);
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

    // Patrol method added by Mark D. 9/9/25
    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        agent.SetDestination(target.position);
        UnityEngine.Debug.Log(target);

        if (!agent.pathPending && agent.remainingDistance <= bufferDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
        }
    }

    // Called when door opens, roomba goes into other room - added 1/29/25 by Mark D.
    public void SwitchPatrol()
    {
        patrolPoints = patrolPoints_Room2;
        currentPatrolIndex = 0;
    }

    // Activate method added by Mark D. 9/10/25
    public void Activate()
    {
        if(!isActivated && !isBroken)
        {
            isActivated = true;
            agent.baseOffset = 1f;
            //virtualCamManager.StartRoombaActivateSequence();
            AudioManager.Instance?.PlayOneShot(roombaOn);
            roombaEmitter.Play();
        }
    }

    // Activate method added by Mark D. 11/17/25
    public void Deactivate()
    {
        isActivated = false;
        isBroken = true;
        // agent.isStopped = true;
        AudioManager.Instance?.PlayOneShot(roombaOff);
        roombaEmitter.Stop();
        StartCoroutine(AttackDoorSequence());
    }

    public void PlayRoombaDetectSFX()
    {
        if (isActivated == true)
        {
            AudioManager.Instance?.PlayOneShot(roombaDetect);
        }
    }

    private IEnumerator AttackDoorSequence()
    {
        isInAttackDoorSequence = true;

        // Generate circle waypoints around current position
        Vector3 center = transform.position;
        int pointCount = 8;
        List<Vector3> circlePoints = new List<Vector3>();
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * (360f / pointCount) * Mathf.Deg2Rad;
            Vector3 point = center + new Vector3(Mathf.Cos(angle) * circleRadius, 0f, Mathf.Sin(angle) * circleRadius);
            circlePoints.Add(point);
        }

        // Loop circle waypoints for spinDuration seconds
        float elapsed = 0f;
        int circleIndex = 0;
        agent.isStopped = false;
        agent.speed = 6f;

        while (elapsed < spinDuration)
        {
            if (!agent.pathPending && agent.remainingDistance <= bufferDistance)
            {
                circleIndex = (circleIndex + 1) % pointCount;
                agent.SetDestination(circlePoints[circleIndex]);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Charge the door
        agent.speed = attackDoorSpeed;
        roombaAttackDoor.ArmForAttack();
        agent.SetDestination(roombaAttackDoor.transform.position);

        isInAttackDoorSequence = false;
    }
}
