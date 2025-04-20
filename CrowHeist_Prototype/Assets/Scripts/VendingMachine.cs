using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;


public class VendingMachine : MonoBehaviour
{
    private ItemEventManager itemEventManager;
    private Controller2Point5D playerController;

    private GameObject player;
    private float playerDistance; 
    [SerializeField] private float checkCooldown = 1.0f;
    private float lastCheckTime = -Mathf.Infinity;



    private void Start()
    {
        itemEventManager = FindObjectOfType<ItemEventManager>();
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<Controller2Point5D>();
        }
        
    }
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (Time.time - lastCheckTime >= checkCooldown)
        {
            DistanceCalculator();
            lastCheckTime = Time.time;
        }
        if(other.CompareTag("Quarter") && playerDistance < 5f)
        {
            Destroy(other.gameObject);
            Notify();
        }
    }
    private void DistanceCalculator()
    {
        if(playerController.heldObject == null)
        {
            playerDistance = Vector3.Distance(transform.position, player.transform.position);
        }
    }


    // Update is called once per frame
    public void Notify()
    {
        Debug.Log("Notifying...");
        itemEventManager.SpawnItemTrigger();
    }
}
