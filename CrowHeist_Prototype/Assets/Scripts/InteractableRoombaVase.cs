using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script by Mark D. - last updated 11/03/2025
// interaction with vase above roomba dock to tip over and break roomba

public class InteractableRoombaVase : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float interactDistance;

    public GameObject brokenPrefab;

    private Rigidbody rb;

    public VirtualCamManager virtualCamManager;
    

    void Start()
    {
        interactDistance = 2.6f;
        rb = GetComponent<Rigidbody>();
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
        rb.isKinematic = false;
        rb.AddForce(Vector3.left * 3f, ForceMode.Impulse);
        virtualCamManager.StartRoombaBreakSequence();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("BreakVase"))
        {
            GameObject broken = Instantiate(brokenPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
