using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashBall : MonoBehaviour
{
    private Rigidbody rb;
    private bool isGrounded = false;

    [SerializeField] private float groundDrag = 2f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.7f)
        {
            rb.drag = groundDrag;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        rb.drag = 0f;
    }
}
