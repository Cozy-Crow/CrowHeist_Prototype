using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

public class FanWindArea : MonoBehaviour
{
    [SerializeField] private float windForce = 2f;
    [SerializeField] private GameObject fanBlades;

    private void OnTriggerStay(Collider other)
    {
        var bladeScript = fanBlades.GetComponent<FanSpin>();

        if (this.gameObject.CompareTag("CeilingFan"))
        {
            if (bladeScript != null && bladeScript.isOn)
            {
                if (other.attachedRigidbody != null)
                {
                    other.attachedRigidbody.AddForce(Vector3.down * windForce * 5, ForceMode.Acceleration);
                }

                var controller = other.GetComponent<CharacterController>();
                if (controller != null)
                {
                    var player = controller.GetComponent<Controller2Point5D>();
                    if (player != null)
                    {
                        player.ApplyExternalForce(Vector3.down * windForce);
                    }
                }
            }
        }

        if (this.gameObject.CompareTag("MovingFan"))
        {
            if (bladeScript != null && bladeScript.isOn)
            {
                Vector3 directionOutward = (other.transform.position - transform.position).normalized;

                if (other.attachedRigidbody != null)
                {
                    other.attachedRigidbody.AddForce(directionOutward * windForce, ForceMode.Acceleration);
                }

                var controller = other.GetComponent<CharacterController>();
                if (controller != null)
                {
                    var player = controller.GetComponent<Controller2Point5D>();
                    if (player != null)
                    {
                        player.ApplyExternalForce(directionOutward * windForce);
                    }
                }
            }
        }
        
        if (this.gameObject.CompareTag("FloorFanForce") && other.CompareTag("Player"))
        {
            var controller = other.GetComponent<CharacterController>();
            if (controller != null)
            {
                var player = controller.GetComponent<Controller2Point5D>();
                if (player != null && player.heldObject != null)
                {
                    return;
                }
            }
        }
        if (this.gameObject.CompareTag("FloorFanForce") && transform.eulerAngles.y > 1f)
        {
            if (bladeScript != null && bladeScript.isOn)
            {
                if (other.attachedRigidbody != null)
                {
                    other.attachedRigidbody.AddForce(Vector3.up * windForce, ForceMode.Acceleration);
                }

                var controller = other.GetComponent<CharacterController>();
                if (controller != null)
                {
                    var player = controller.GetComponent<Controller2Point5D>();
                    if (player != null)
                    {
                        player.ApplyExternalForce(Vector3.up * windForce * 20);
                    }
                }
            }
        }
    }

}
