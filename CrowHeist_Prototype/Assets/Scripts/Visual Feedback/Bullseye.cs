using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullseye : MonoBehaviour
{
    public ParticleSystem confetti;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dart"))
        {
            if (confetti != null)
            {
                confetti.Play();
            }
        }
    }
}
