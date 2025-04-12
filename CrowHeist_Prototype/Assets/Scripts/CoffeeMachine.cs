using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;


public class CoffeeMachine : MonoBehaviour
{
    [SerializeField] private Transform mugHolder;
    private Controller2Point5D playerController;
    private GameObject player;
    private float playerDistance; 
    [SerializeField] private float checkCooldown = 1.0f;
    private float lastCheckTime = -Mathf.Infinity;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<Controller2Point5D>();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        // Timer so that distance isn't calculated every time the mug enters the trigger
        if (Time.time - lastCheckTime >= checkCooldown)
        {
            DistanceCalculator();
            lastCheckTime = Time.time;
        }

        // Check if the entering object is tagged as "Cup"
        if (other.CompareTag("Mug") && playerDistance < 5f)
        {
            // Make the cup a child of MugHolder & Set the mug's transform to equal the mug holder
            other.transform.SetParent(mugHolder);
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
            Debug.Log("Child");
        }
    }

    private void DistanceCalculator()
    {
        if(playerController.heldObject == null)
        {
            playerDistance = Vector3.Distance(transform.position, player.transform.position);
            Debug.Log("Distance to Player: " + playerDistance);

        }
    }
}
