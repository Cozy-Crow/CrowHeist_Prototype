using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBox : MonoBehaviour
{
    public bool bounce = false;
    private Vector3 launchForce = new Vector3(0, 10f, 0);
    public GameObject player;
    private Controller2Point5D playerController; 

    void Start()
    {
        if(player != null)
        {
            playerController = player.GetComponent<Controller2Point5D>();

        }
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (playerController.canBounce && other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddForce(launchForce, ForceMode.Impulse);
        }

    }
}
