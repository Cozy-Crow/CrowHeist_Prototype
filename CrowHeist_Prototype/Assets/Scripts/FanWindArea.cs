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
        if(this.gameObject.CompareTag("CeilingFan"))
        {
            var bladeScript = fanBlades.GetComponent<FanSpin>();
            // Affect rigidbodies
            if (other.attachedRigidbody != null && bladeScript.isOn)
            {
                other.attachedRigidbody.AddForce(Vector3.down * windForce * 5, ForceMode.Acceleration);
            }

            // Affect CharacterController
            var controller = other.GetComponent<CharacterController>();
            if (controller != null && bladeScript.isOn)
            {
                // Assuming the controller script has a public method or flag to receive external forces
                var player = controller.GetComponent<Controller2Point5D>(); // replace with your script
                if (player != null)
                {
                    player.ApplyExternalForce(Vector3.down * windForce);
                }
            }
        }
        if (this.gameObject.CompareTag("MovingFan"))
        {
            var bladeScript = fanBlades.GetComponent<FanSpin>();
            if (bladeScript != null && bladeScript.isOn)
            {
                Vector3 directionOutward = (other.transform.position - transform.position).normalized;

                // Rigidbodies
                if (other.attachedRigidbody != null)
                {
                    other.attachedRigidbody.AddForce(directionOutward * windForce, ForceMode.Acceleration);
                }

                // CharacterController
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
    }
}
