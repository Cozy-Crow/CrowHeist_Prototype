using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBox : MonoBehaviour
{
    public bool bounce = false;
    private Vector3 launchForce = new Vector3(0, 30f, 0);
    public GameObject player;
    private Controller2Point5D playerController;
    private Transform jack;

    void Start()
    {
        jack = transform.Find("SpringFunction");

        if (player != null)
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
            jack.gameObject.SetActive(true);
            playerController.canBounce = false;
        }

    }
}
