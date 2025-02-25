using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableCube : MonoBehaviour
{
    public GameObject brokenPrefab; // Assign the broken cube prefab in the Inspector
    public float breakForce = 5f; // Velocity needed to break

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > breakForce)
        {
            Break();
        }
    }

    void Break()
    {
        // Spawn the broken version
        GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);

        // Get all Rigidbody components and add force for explosion effect
        foreach (Rigidbody rb in broken.GetComponentsInChildren<Rigidbody>())
        {
            rb.AddExplosionForce(5f, transform.position, 2f);
        }

        // Destroy the original cube
        Destroy(gameObject);
    }
}
