using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;

[RequireComponent(typeof(Pickable))]

public class KnifeStick : MonoBehaviour
{
    private bool isStuck = false;
    private float bounceForce = 1f;
    private float maxBounceForce = 2.5f;
    private float bounceIncrease = 0.125f;
    private float resetTime = 1.5f;
    private Coroutine resetCoroutine;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;
    private float rotationSpeed = 0f;
    private Rigidbody rb;
    private BoxCollider[] childColliders;
    private Pickable pickableup;

    public BoxCollider blade;
    public BoxCollider bouncer;
    public BoxCollider butt;

    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
        rb = GetComponent<Rigidbody>();
        childColliders = GetComponentsInChildren<BoxCollider>();
        pickableup = GetComponent<Pickable>();
        bouncer.isTrigger = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false;

    }
    void Update()
    {
        if (!isStuck && rb != null)
        {
            // Rotate the knife while it's moving
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            
        }
        
    }
    
    void SetColliderActive(int index, bool isActive)
    {
        if (index >= 0 && index < childColliders.Length) // Ensure valid index
        {
            childColliders[index].isTrigger = isActive;
        }
        else
        {
            Debug.LogWarning("Invalid index for child colliders.");
        }
    }


    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }

    void OnTriggerStay(Collider other)
    {
        if (!isStuck && other.CompareTag("Wall") && !pickableup.pickedUp)
        {
            //rb = GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = true; // Stops physics interactions
                rb.velocity = Vector3.zero;
                rotationSpeed = 0f; // Stop spinning

            }

            // Store world position and rotation before disabling physics
            Vector3 worldPosition = transform.position;
            Quaternion worldRotation = transform.rotation;

            transform.SetParent(null); // Unparent to prevent scaling issues

            // Restore position and rotation so the knife doesn't move unexpectedly
            transform.position = worldPosition;
            transform.rotation = worldRotation;

            // Ensure scale remains unchanged
            transform.localScale = originalScale;
            bouncer.isTrigger = true;
            isStuck = true;
            

        }

        if (isStuck && other.CompareTag("Player"))
        {
            bouncer.isTrigger = true;
            Controller2Point5D playerController = other.GetComponent<Controller2Point5D>();

            if (playerController != null && playerController.IsGrounded && Input.GetButton("Jump"))
            {
                Debug.Log("Player Bounced on Knife!");
                playerController.ApplyBounce(bounceForce);

                // Increase bounce force, capped at maxBounceForce
                bounceForce = Mathf.Min(bounceForce + bounceIncrease, maxBounceForce);

                // Reset bounce force if player stops bouncing
                if (resetCoroutine != null)
                    StopCoroutine(resetCoroutine);
                resetCoroutine = StartCoroutine(ResetBounceForce());
            }
        }
       
    }

    void OnCollisionEnter(Collision collision)
    {
        // Stop the knife if it hits a wall or the ground
        if (!isStuck && collision.gameObject.CompareTag("Ground"))
        {
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero; // Stop spinning
            }

            rotationSpeed = 0f; // Ensure rotation stops

        }
    }


    void OnTriggerExit(Collider other)
        {
            if (isStuck && other.CompareTag("Wall"))
            {
                isStuck = false;
                bouncer.isTrigger = false;
            }
        }



    private IEnumerator ResetBounceForce()
    {
        yield return new WaitForSeconds(resetTime);
        bounceForce = 1f;
        Debug.Log("Bounce Force Reset");
    }
}