using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class RoombAi : MonoBehaviour
{
    [Header("Roomba Settings")]
    [HideInInspector]
    [SerializeField] private NavMeshAgent agent;            // The navmesh agent for the roomba
    [SerializeField] private Transform[] target;            // The target points for the roomba to move to
    [SerializeField] private RoombaPathing pathing;         // The pathing method for the roomba
    [SerializeField] private float bufferDistance;          // The buffer distance for the roomba to reach the target
    [SerializeField] private float detectionRadius = 5f;    // Radius for detecting dirty objects
    [SerializeField] private LayerMask dirtyLayerMask;      // The layer mask to detect dirty objects

    private int currentTargetIndex = 0;                     // The current target index for the roomba
    private bool goingBackwards = false;                    // If the roomba is going backwards in the target array if the pathing is sequential
    private bool isDocked = true;                            // Flag to check if Roomba is docked
    private GameObject dirtyObject;                         // The object the roomba needs to clean

    //Clmanp property for the current target index
    public int CurrentTargetIndex
    {
        get => currentTargetIndex;
        set
        {
            currentTargetIndex = Mathf.Clamp(value, 0, target.Length - 1);
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    //Updates the pathing method
    void Update()
    {
        switch (pathing)
        {
            case RoombaPathing.Random:
                RandomPathing();
                break;
            case RoombaPathing.Sequential:
                SequentialPathingMethod();
                break;
        }
    }

    /// <summary>
    /// Moves the roomba in a sequential order to each targetpoint
    /// </summary>
    private void SequentialPathingMethod()
    {
        //TODO: FIX first target not being reached (P2)
        if (goingBackwards)
        {
            if (agent.remainingDistance <= bufferDistance)
            {
                CurrentTargetIndex--;
                if (CurrentTargetIndex == 0)
                {
                    goingBackwards = false;
                }
                agent.SetDestination(target[CurrentTargetIndex].position);
            }
        }
        else
        {
            if (agent.remainingDistance <= bufferDistance)
            {
                CurrentTargetIndex++;
                if (CurrentTargetIndex == target.Length - 1)
                {
                    goingBackwards = true;
                }
                agent.SetDestination(target[CurrentTargetIndex].position);
            }
        }
    }

    /// <summary>
    /// Randomly selects a targetpoint to move to
    /// </summary>
    private void RandomPathing()
    {
        if (agent.remainingDistance <= bufferDistance)
        {
            agent.SetDestination(target[Random.Range(0, target.Length)].position);
        }
    }

    // Enum for the pathing type
    public enum RoombaPathing
    {
        Random,
        Sequential
    }
}
