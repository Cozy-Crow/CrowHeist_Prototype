using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableRoombaVase : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float interactDistance;

    void start()
    {
        interactDistance = 2.6f;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < interactDistance)
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                TipVase();
            }
        }
    }

    private void TipVase()
    {
        Debug.Log("Tip Roomba Vase");
    }
}
