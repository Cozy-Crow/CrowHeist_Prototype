using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartBoardTarget : MonoBehaviour
{
    public ParticleSystem confetti;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Knife"))
        {
            if (confetti != null)
            {
                confetti.Play();
            }
            Debug.Log("Paint tube appears");
        }
    }
}
