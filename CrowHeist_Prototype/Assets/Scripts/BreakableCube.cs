using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakableCube : MonoBehaviour
{
    public GameObject brokenPrefab; // Assign the broken cube prefab in the Inspector
    public float breakForce = 5f; // Velocity needed to break
    public Vector3[] velocities = new Vector3[3] { Vector3.zero, Vector3.zero, Vector3.zero };

    private void FixedUpdate()
    {
        Vector3[] hold = new Vector3[3];

        hold[0] = GetComponent<Rigidbody>().velocity;
        hold[1] = velocities[0];
        hold[2] = velocities[1];

        velocities = hold;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null) // Ensure the other object has a Rigidbody
        {
            Vector3 impactForce = collision.impulse / Time.fixedDeltaTime; // Calculate force
            Debug.Log($"Collision with {collision.gameObject.name}, Impact Force: {impactForce.magnitude}");

            if (impactForce.magnitude > breakForce)
            {
                Break();
            }
        }
    }


    void Break()
    {
        // Spawn the broken version
        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);

        Vector3 max = Vector3.zero;

        // Gets the max velocity of the unbroken object of the last three frames
        foreach(Vector3 x in velocities){
            if(x.magnitude > max.magnitude)
            {
                max = x;
            }
        }

        // Get all Rigidbody components, sets velocity to original unbroken object, and add force for explosion effect
        foreach (Rigidbody rb in broken.GetComponentsInChildren<Rigidbody>())
        {
            rb.velocity = max;
            rb.AddExplosionForce(5f, transform.position, 2f);
        }

        // Destroy the original cube
        Destroy(gameObject);
    }
}
