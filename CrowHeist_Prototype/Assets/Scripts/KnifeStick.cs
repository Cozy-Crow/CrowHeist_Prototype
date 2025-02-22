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


    void OnTriggerStay(Collider other)
    {
        if (!isStuck && other.CompareTag("Wall"))
        {
            GetComponent<Rigidbody>().isKinematic = true;
            transform.parent = other.transform;
            isStuck = true;
        }

        if (isStuck && other.CompareTag("Player"))
        {
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
    void OnTriggerExit(Collider other)
    {
        if (isStuck && other.CompareTag("Wall"))
        {
            isStuck = false;
        }
    }



    private IEnumerator ResetBounceForce()
    {
        yield return new WaitForSeconds(resetTime);
        bounceForce = 1f;
        Debug.Log("Bounce Force Reset");
    }
}