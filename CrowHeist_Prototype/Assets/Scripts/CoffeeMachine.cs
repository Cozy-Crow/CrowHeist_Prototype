using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController.Examples;


public class CoffeeMachine : MonoBehaviour
{
    [SerializeField] private Transform mugHolder;

    private Controller2Point5D playerController;
    private GameObject player;
    private float playerDistance; 
    [SerializeField] private float checkCooldown = 1.0f;
    [SerializeField] private float fillTimer = 3f;
    [SerializeField] private float maxFillHeight = 0.1f; 


    private float lastCheckTime = -Mathf.Infinity;
    private bool isWaiting = false;
    private GameObject mug;
    private GameObject coffee;


    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerController = player.GetComponent<Controller2Point5D>();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        // Timer so that distance isn't calculated every time the mug enters the trigger
        if (Time.time - lastCheckTime >= checkCooldown)
        {
            DistanceCalculator();
            lastCheckTime = Time.time;
        }

        // Check if the entering object is tagged as "Mug"
        if (other.CompareTag("Mug") && playerDistance < 5f && playerController.heldObject == null)
        {
            var rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            other.transform.SetParent(mugHolder);
            other.transform.position = mugHolder.position;
            other.transform.rotation = mugHolder.rotation;
            mug = other.gameObject;
        }


        if(mug != null)
        {
            Debug.Log("Mug Detected");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && Input.GetKeyDown(KeyCode.Q) && mug != null && !isWaiting)
        {
            Debug.Log("Filling!!!!");
            StartCoroutine(WaitAndDoAction(fillTimer));
        }
    }


    
    private void DistanceCalculator()
    {
        if(playerController.heldObject == null)
        {
            playerDistance = Vector3.Distance(transform.position, player.transform.position);
        }
    }

    private IEnumerator WaitAndDoAction(float seconds)
    {
        isWaiting = true;
        float elapsed = 0f;
        Debug.Log("Timer started...");

        // Find the coffee liquid GameObject
        foreach (Transform child in GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "CoffeeLiquid")
            {
                coffee = child.gameObject;
                break;
            }
        }

        coffee.SetActive(true);

        Vector3 startLocalPos = coffee.transform.localPosition;
        float targetY = startLocalPos.y + maxFillHeight;

        while (elapsed < seconds)
        {
            float t = elapsed / seconds;
            float newY = Mathf.Lerp(startLocalPos.y, targetY, t);
            coffee.transform.localPosition = new Vector3(startLocalPos.x, newY, startLocalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap to final position
        coffee.transform.localPosition = new Vector3(startLocalPos.x, targetY, startLocalPos.z);

        mug.tag = "Dashable";
        isWaiting = false;
    }


}
