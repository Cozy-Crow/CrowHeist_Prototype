using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject brokenDoor;

    // [SerializeField] private Transform roomba;
    // [SerializeField] private float breakDistance = 9f;

    private bool isAttacking = false;

    public void SetAttacking()
    {
        isAttacking = true;
    }

    private void Update()
    {
        // if (!isAttacking || roomba == null) return;

        // float distance = Vector3.Distance(roomba.position, door.transform.position);
        // UnityEngine.Debug.Log("Roomba distance to door: " + distance);

        // if (distance <= breakDistance)
        // {
        //     UnityEngine.Debug.Log("Roomba broke the door!");
        //     brokenDoor.SetActive(true);
        //     door.SetActive(false);
        //     isAttacking = false;

        //     Destroy(roomba.gameObject);
        // }
    }

    public void Break()
    {
        brokenDoor.SetActive(true);
        door.SetActive(false);
    }

    // private void OnTriggerEnter(Collider other)
    // {
    //     // if (!isAttacking) return;

    //     if (other.CompareTag("RoombaTrigger"))
    //     {
    //         brokenDoor.SetActive(true);
    //         door.SetActive(false);
    //         isAttacking = false;

    //         UnityEngine.Debug.Log("roomba hit door");
    //         // brokenDoor.SetActive(true);
    //         // Destroy(door);
    //     }
    // }
}

