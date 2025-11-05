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
    private List<Pickable> processedItems = new List<Pickable>();

    [Header("Launch Settings")]
    public float launchForce = 10f;
    public Vector3 launchDirection = Vector3.up;

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

    void OnTriggerEnter(Collider other)
    {
        // Check if the object has the Pickable component (check parent objects too)
        Pickable pickable = other.GetComponentInParent<Pickable>();

        if (pickable != null)
        {
            // Start filling water when item enters
            StartFilling();

            // If the sink is filled and item hasn't been processed yet, process it
            if (isFilled && !processedItems.Contains(pickable) && !itemsBeingCleaned.Contains(pickable))
            {
                itemsBeingCleaned.Add(pickable); // Add immediately to prevent double processing
                StartCoroutine(ProcessItem(pickable));
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // Check if water just finished filling (check parent objects too)
        Pickable pickable = other.GetComponentInParent<Pickable>();

        if (pickable != null && isFilled && !itemsBeingCleaned.Contains(pickable) && !processedItems.Contains(pickable))
        {
            itemsBeingCleaned.Add(pickable); // Add immediately to prevent double processing
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
                item.player._isDirty = false;
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

        // Mark this item as processed so it won't be processed again until it exits
        processedItems.Add(item);
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

            if (processedItems.Contains(pickable))
            {
                processedItems.Remove(pickable);
            }
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

            // Apply launch force
            rb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);

            Debug.Log(item.gameObject.name + " launched into the air!");

            // Drain the sink after a short delay
            StartCoroutine(DrainAfterDelay(drainDelay));
        }
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