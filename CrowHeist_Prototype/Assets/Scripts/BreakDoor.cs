using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject brokenDoor;

    private bool isAttacking = false;

    public void SetAttacking()
    {
        isAttacking = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        if (other.CompareTag("BreakDoor"))
        {
            brokenDoor.SetActive(true);
            Destroy(door);
            isAttacking = false;
        }
    }
}

