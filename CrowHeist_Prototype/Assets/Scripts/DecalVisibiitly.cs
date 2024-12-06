using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecalVisibility : MonoBehaviour
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                GameObject.SetActive(true);
            }
        }
    }
}
