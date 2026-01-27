using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    [Header("Water Settings")]
    public GameObject waterObject;
    public float fillTime = 2f;
    public float drainTime = 1.5f;
    public float maxWaterHeight = 1f;
    public Material cleanWaterMaterial;
    public Material dirtyWaterMaterial;
    public float drainDelay = 1f;
    private float currentWaterHeight = 0f;
    private bool isFilling = false;
    private bool isFilled = false;
    private bool isDraining = false;
    private bool isWaterDirty = false;

    [Header("Processing Settings")]
    public float processingTime = 1f;
    private List<Pickable> itemsBeingCleaned = new List<Pickable>();
    private Dictionary<Pickable, float> itemLaunchCooldowns = new Dictionary<Pickable, float>();
    private float launchCooldownDuration = 3f; // Time before an item can be processed again

    [Header("Launch Settings")]
    public float launchForce = 10f;
    public Vector3 launchDirection = Vector3.up;
    public float randomnessX = 0.3f; // Random variation in X direction (-randomnessX to +randomnessX)
    public float randomnessY = 0.2f; // Random variation in Y direction
    public float randomnessZ = 0.3f; // Random variation in Z direction

    private Vector3 waterInitialScale;

    void Start()
    {
        if (waterObject != null)
        {
            waterInitialScale = waterObject.transform.localScale;
            // Start with water at minimum height
            waterObject.transform.localScale = new Vector3(
                waterInitialScale.x,
                0.01f,
                waterInitialScale.z
            );

            // Store the clean water material if not already set
            if (cleanWaterMaterial == null)
            {
                MeshRenderer renderer = waterObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    cleanWaterMaterial = renderer.material;
                }
            }
        }
    }

    void Update()
    {
        // Update cooldown timers
        List<Pickable> itemsToRemove = new List<Pickable>();
        foreach (var kvp in itemLaunchCooldowns)
        {
            if (Time.time >= kvp.Value)
            {
                itemsToRemove.Add(kvp.Key);
            }
        }
        foreach (var item in itemsToRemove)
        {
            itemLaunchCooldowns.Remove(item);
        }

        // Fill water when needed
        if (isFilling && !isFilled)
        {
            currentWaterHeight += (maxWaterHeight / fillTime) * Time.deltaTime;

            if (currentWaterHeight >= maxWaterHeight)
            {
                currentWaterHeight = maxWaterHeight;
                isFilled = true;
                isFilling = false;
            }

            if (waterObject != null)
            {
                waterObject.transform.localScale = new Vector3(
                    waterInitialScale.x,
                    currentWaterHeight,
                    waterInitialScale.z
                );
            }
        }

        // Drain water when needed
        if (isDraining)
        {
            currentWaterHeight -= (maxWaterHeight / drainTime) * Time.deltaTime;

            if (currentWaterHeight <= 0.01f)
            {
                currentWaterHeight = 0.01f;
                isDraining = false;

                // Reset water back to clean material after draining
                if (isWaterDirty && cleanWaterMaterial != null && waterObject != null)
                {
                    MeshRenderer renderer = waterObject.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.material = cleanWaterMaterial;
                        isWaterDirty = false;
                        Debug.Log("Water has been reset to clean!");
                    }
                }
            }

            if (waterObject != null)
            {
                waterObject.transform.localScale = new Vector3(
                    waterInitialScale.x,
                    currentWaterHeight,
                    waterInitialScale.z
                );
            }
        }
    }

    public void StartFilling()
    {
        if (!isFilled && !isFilling)
        {
            isFilling = true;
        }
    }

    bool CanProcessItem(Pickable pickable)
    {
        // Check if item is on cooldown
        if (itemLaunchCooldowns.ContainsKey(pickable))
        {
            return false;
        }

        // Check if item is already being cleaned
        if (itemsBeingCleaned.Contains(pickable))
        {
            return false;
        }

        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the Pickable component (check parent objects too)
        Pickable pickable = other.GetComponentInParent<Pickable>();

        if (pickable != null)
        {
            // Start filling water when item enters
            StartFilling();

            // If the sink is filled and item can be processed, process it
            if (isFilled && CanProcessItem(pickable))
            {
                itemsBeingCleaned.Add(pickable);
                StartCoroutine(ProcessItem(pickable));
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Check if water just finished filling (check parent objects too)
        Pickable pickable = other.GetComponentInParent<Pickable>();

        if (pickable != null && isFilled && CanProcessItem(pickable))
        {
            itemsBeingCleaned.Add(pickable);
            StartCoroutine(ProcessItem(pickable));
        }
    }

    IEnumerator ProcessItem(Pickable item)
    {
        yield return new WaitForSeconds(processingTime);

        // Check if this item has a Paintbrush component with isDirty boolean
        Paintbrush paintbrush = item.GetComponent<Paintbrush>();
        bool wasDirty = false;

        if (paintbrush != null)
        {
            wasDirty = paintbrush.isDirty;
            Debug.Log($"Processing {item.gameObject.name} - Paintbrush.isDirty BEFORE: {paintbrush.isDirty}");
        }

        // Clean the item if it was dirty
        if (wasDirty && paintbrush != null)
        {
            paintbrush.isDirty = false;

            Debug.Log($"Set {item.gameObject.name}.Paintbrush.isDirty to false - Current value: {paintbrush.isDirty}");

            // Update player dirty status if the item is picked up
            if (item.pickedUp && item.player != null)
            {
                item.player.isDirty = false;
            }

            Debug.Log(item.gameObject.name + " has been cleaned!");

            // Turn water brown/dirty only if the item was dirty
            MakeWaterDirty();
        }
        else
        {
            Debug.Log(item.gameObject.name + " processed (was already clean or no Paintbrush component)");
        }

        if (paintbrush != null)
        {
            Debug.Log($"Processing {item.gameObject.name} - Paintbrush.isDirty AFTER: {paintbrush.isDirty}");
        }

        // Launch the item into the air (happens regardless of dirty state)
        LaunchItem(item);

        itemsBeingCleaned.Remove(item);

        // Add item to cooldown to prevent immediate reprocessing
        itemLaunchCooldowns[item] = Time.time + launchCooldownDuration;
    }

    void OnTriggerExit(Collider other)
    {
        Pickable pickable = other.GetComponentInParent<Pickable>();

        if (pickable != null)
        {
            if (itemsBeingCleaned.Contains(pickable))
            {
                itemsBeingCleaned.Remove(pickable);
            }
            // Note: We don't remove from cooldown on exit - the cooldown timer handles that
        }
    }

    void LaunchItem(Pickable item)
    {
        Rigidbody rb = item.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Make sure the item isn't kinematic
            rb.isKinematic = false;

            // Reset velocity before launching
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Calculate randomized launch direction
            Vector3 randomizedDirection = GetRandomizedLaunchDirection();

            // Apply launch force
            rb.AddForce(randomizedDirection * launchForce, ForceMode.Impulse);

            // Add some random torque for more natural movement
            Vector3 randomTorque = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * launchForce * 0.5f;
            rb.AddTorque(randomTorque, ForceMode.Impulse);

            Debug.Log($"{item.gameObject.name} launched with direction {randomizedDirection}!");

            // Drain the sink after a short delay
            StartCoroutine(DrainAfterDelay(drainDelay));
        }
    }

    Vector3 GetRandomizedLaunchDirection()
    {
        // Add random variation to each component of the launch direction
        Vector3 randomOffset = new Vector3(
            Random.Range(-randomnessX, randomnessX),
            Random.Range(-randomnessY, randomnessY),
            Random.Range(-randomnessZ, randomnessZ)
        );

        // Combine base direction with random offset and normalize
        Vector3 finalDirection = (launchDirection + randomOffset).normalized;

        return finalDirection;
    }

    IEnumerator DrainAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        DrainSink();
    }

    void MakeWaterDirty()
    {
        if (!isWaterDirty && waterObject != null && dirtyWaterMaterial != null)
        {
            MeshRenderer renderer = waterObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = dirtyWaterMaterial;
                isWaterDirty = true;
                Debug.Log("Water has turned brown/dirty!");
            }
        }
    }

    // Public method to drain the sink
    public void DrainSink()
    {
        if (!isDraining)
        {
            isFilled = false;
            isFilling = false;
            isDraining = true;
            Debug.Log("Sink is draining...");
        }
    }
}