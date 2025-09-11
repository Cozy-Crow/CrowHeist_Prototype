using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PixieDust : MonoBehaviour, IPickupable
{
    [Header("Pixie Dust Settings")]
    [SerializeField] private float levitationDuration = 5f;
    [SerializeField] private float levitationForce = 15f;
    [SerializeField] private GameObject pixieDustParticlesPrefab;
    [SerializeField] private GameObject levitationParticlesPrefab;
    [SerializeField] private Material pixieDustShaderMaterial;

    [Header("Audio")]
    [SerializeField] private EventReference pixieDustShimmerSound;
    [SerializeField] private EventReference levitationSound;
    [SerializeField] private EventReference pixieDustEndSound;

    private GameObject item;
    private bool isUsed = false;
    private PixieDustFloorManager floorManager;
    private KinematicCharacterController.Examples.Controller2Point5D playerController;

    public GameObject Item => item;

    void Awake()
    {
        item = gameObject;

        // Set the tag so the controller recognizes it as a pixie dust item
        gameObject.tag = "PixieDust";

        floorManager = FindObjectOfType<PixieDustFloorManager>();
        if (floorManager == null)
        {
            // Create floor manager if it doesn't exist
            GameObject managerObj = new GameObject("PixieDustFloorManager");
            floorManager = managerObj.AddComponent<PixieDustFloorManager>();
        }
    }

    void Update()
    {
        // Check if we're being held and if the player pressed the use key
        if (transform.parent != null && !isUsed)
        {
            // Get the player controller
            if (playerController == null)
            {
                playerController = FindObjectOfType<KinematicCharacterController.Examples.Controller2Point5D>();
            }

            // Check for use input (same key as other special items in your controller)
            if (Input.GetKeyDown(KeyCode.LeftShift) && playerController != null)
            {
                Debug.Log("Using Pixie Dust!");
                Use();
            }
        }
    }

    public void PickUP(Transform parent)
    {
        if (isUsed) return;

        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Disable physics while being held
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Disable collider while being held
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }
    }

    public void Drop(Vector3 position)
    {
        if (isUsed) return;

        transform.SetParent(null);
        transform.position = position;

        // Re-enable physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Re-enable collider
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

        // Find the player controller
        var playerController = FindObjectOfType<KinematicCharacterController.Examples.Controller2Point5D>();
        if (playerController != null)
        {
            StartCoroutine(ApplyPixieDustEffect(playerController));
        }

        // Destroy this item after use
        Destroy(gameObject, 0.1f);
    }

    private IEnumerator ApplyPixieDustEffect(KinematicCharacterController.Examples.Controller2Point5D player)
    {
        // Create the pixie dust controller and attach it to the player
        GameObject effectController = new GameObject("PixieDustEffect");
        effectController.transform.SetParent(player.transform);
        effectController.transform.localPosition = Vector3.zero;

        PixieDustEffectController pixieController = effectController.AddComponent<PixieDustEffectController>();
        pixieController.Initialize(
            levitationDuration,
            levitationForce,
            pixieDustParticlesPrefab,
            levitationParticlesPrefab,
            pixieDustShaderMaterial,
            pixieDustShimmerSound,
            levitationSound,
            pixieDustEndSound,
            floorManager
        );

        // The effect controller will handle everything from here
        yield return null;
    }
}