using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaAttackDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject brokenDoor;

    private bool isAttacking = false;

    public void ArmForAttack()
    {
        isAttacking = true;
    }

    // public void AttackDoor()
    // {
    //     // Called externally if needed, arms the system
    //     isAttacking = true;
    // }

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

