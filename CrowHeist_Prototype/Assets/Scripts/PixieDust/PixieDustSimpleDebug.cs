using System.Collections;
using UnityEngine;

// Simple debug version to test just the levitation without audio/particles
public class PixieDustSimpleDebug : MonoBehaviour, IPickupable
{
    [SerializeField] private Enum_Sockets socketType;
    [Header("Debug Settings")]
    [SerializeField] private float levitationDuration = 5f;
    [SerializeField] private float levitationForce = 50f; // Increased force for testing

    private GameObject item;
    private bool isUsed = false;
    private KinematicCharacterController.Examples.Controller2Point5D playerController;
    public Enum_Sockets SocketType { get => socketType; }

    void Awake()
    {
        item = gameObject;
        gameObject.tag = "PixieDust";
    }

    void Update()
    {
        if (transform.parent != null && !isUsed)
        {
            if (playerController == null)
            {
                playerController = FindObjectOfType<KinematicCharacterController.Examples.Controller2Point5D>();
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && playerController != null)
            {
                Debug.Log("Using Simple Pixie Dust!");
                Use();
            }
        }
    }

    public void PickUp(Transform parent)
    {
        if (isUsed) return;

        Debug.Log("Picking up Pixie Dust");
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void Drop(Vector3 position)
    {
        if (isUsed) return;

        Debug.Log("Dropping Pixie Dust");
        transform.SetParent(null);
        transform.position = position;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    public void Use()
    {
        if (isUsed) return;

        isUsed = true;
        Debug.Log("Starting simple levitation effect");

        var player = FindObjectOfType<KinematicCharacterController.Examples.Controller2Point5D>();
        if (player != null)
        {
            StartCoroutine(SimpleLevitation(player));
        }

        Destroy(gameObject, 0.1f);
    }

    private IEnumerator SimpleLevitation(KinematicCharacterController.Examples.Controller2Point5D player)
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        float timer = levitationDuration;

        Debug.Log($"Starting levitation - Duration: {levitationDuration}, Force: {levitationForce}");
        Debug.Log($"Player Rigidbody: {playerRb != null}");
        Debug.Log($"Initial player velocity: {playerRb?.velocity}");

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (playerRb != null)
            {
                // Apply strong upward force
                Vector3 upwardForce = Vector3.up * levitationForce;
                Vector3 antiGravity = -Physics.gravity * playerRb.mass;
                Vector3 totalForce = upwardForce + antiGravity;

                playerRb.AddForce(totalForce, ForceMode.Force);

                // Debug every second
                if ((int)(levitationDuration - timer) != (int)(levitationDuration - timer - Time.deltaTime))
                {
                    Debug.Log($"Time: {levitationDuration - timer:F1}s, Player Y velocity: {playerRb.velocity.y:F2}, Applied force: {totalForce}");
                }
            }

            yield return null;
        }

        Debug.Log("Levitation effect complete");
    }

    public void Consume()
    {
        return;
    }
}