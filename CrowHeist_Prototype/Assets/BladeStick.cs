using UnityEngine;

public class BladeStick : MonoBehaviour
{
    private Rigidbody rb;
    private bool isStuck = false;
    public LayerMask stickableLayer; // Assign in Inspector to specify what the knife can stick to

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the knife has already stuck to something
        if (isStuck) return;

        // Check if the object is in the stickable layer
        if (((1 << collision.gameObject.layer) & stickableLayer) != 0)
        {
            // Get the contact point and normal
            ContactPoint contact = collision.contacts[0];

            // Ensure the knife is hitting with the blade (forward direction)
            if (Vector3.Dot(contact.normal, transform.forward) < -0.7f) // Adjust threshold as needed
            {
                StickKnife(collision.gameObject, contact.point, contact.normal);
            }
        }
    }

    private void StickKnife(GameObject surface, Vector3 contactPoint, Vector3 normal)
    {
        isStuck = true;

        // Disable physics to prevent further movement
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Align the knife with the surface normal
        transform.position = contactPoint;
        transform.rotation = Quaternion.LookRotation(-normal, Vector3.up); // Adjust orientation

        // Parent the knife to the surface while maintaining world position
        transform.SetParent(surface.transform, true);

        Debug.Log("Knife stuck successfully!");
    }
}
