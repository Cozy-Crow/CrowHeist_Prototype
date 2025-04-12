using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;


public class CoffeeMachine : MonoBehaviour
{
    [SerializeField] private Transform mugHolder;
    private Controller2Point5D playerController;

    private void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<Controller2Point5D>();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is tagged as "Cup"
        if (other.CompareTag("Mug") && playerController.heldObject == null)
        {
            // Make the cup a child of MugHolder
            other.transform.SetParent(mugHolder);

            // Optional: Snap the cup to MugHolder's position and rotation
            other.transform.localPosition = Vector3.zero;
            other.transform.localRotation = Quaternion.identity;
            Debug.Log("Child");
        }
    }
}
