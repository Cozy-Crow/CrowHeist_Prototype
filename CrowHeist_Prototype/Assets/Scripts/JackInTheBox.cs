using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBox : MonoBehaviour
{
    public bool bounce = false;
    public float forceAmount = 10f;  // Adjust force value

    private void OnTriggerStay(Collider other)
    {
        if (bounce && other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * forceAmount, ForceMode.Impulse);
            Debug.Log("Hit!");

        }
    }
}
