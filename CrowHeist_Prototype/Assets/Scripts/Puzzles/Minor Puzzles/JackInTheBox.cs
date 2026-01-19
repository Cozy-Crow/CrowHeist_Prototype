using KinematicCharacterController.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBox : MonoBehaviour
{
    public float cooldownDuration = 2f; // seconds between impulses
    private float lastBounceTime = -Mathf.Infinity;

    public bool bounce = false;
    private Vector3 launchForce = new Vector3(0, 20f, 0);
    public GameObject player;
    private Controller2Point5D playerController;
    private GameObject jack;

    void Start()
    {
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "SpringFunction")
            {
                jack = child.gameObject;
                break;
            }
        }

        if (player != null)
        {
            playerController = player.GetComponent<Controller2Point5D>();

        }

    }
    private void OnTriggerStay(Collider other)
    {

        if (Time.time - lastBounceTime >= cooldownDuration && 
            playerController.canBounce && 
            other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddForce(launchForce, ForceMode.Impulse);
            jack.gameObject.SetActive(true);
            playerController.canBounce = false;

            lastBounceTime = Time.time; // set cooldown timer
        }

    }
}
