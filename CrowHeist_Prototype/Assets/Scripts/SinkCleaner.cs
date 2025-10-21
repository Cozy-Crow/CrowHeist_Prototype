using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public class SinkCleaner : MonoBehaviour
{
    [Header("Water Settings")]
    [SerializeField] private GameObject waterObject; // Assign a water plane/mesh in the sink
    [SerializeField] private float waterFillSpeed = 0.5f;
    [SerializeField] private float maxWaterLevel = 0.5f; // Local Y position for max water
    [SerializeField] private float minWaterLevel = -0.5f; // Local Y position when empty
    [SerializeField] private ParticleSystem waterParticles; // Optional water pour effect

    [Header("Interaction")]
    [SerializeField] private bool isInteractable = true;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode fillKey = KeyCode.E;
    [SerializeField] private KeyCode drainKey = KeyCode.Q;

    [Header("Cleaning")]
    [SerializeField] private Transform cleaningZone; // Trigger area for detecting objects
    [SerializeField] private float cleaningDelay = 1f; // Time object needs to be in water
    [SerializeField] private ParticleSystem bubbleEffect; // Optional cleaning bubbles
    [SerializeField] private string[] dirtyFieldNames = { "isFilthy", "isDirty"}; // Field names to check
    [SerializeField] private string[] cleanableTags = { "Cleanable", "cleanable" }; // Tags that can be cleaned
    [SerializeField] private bool cleanAllObjects = true; // If true, tries to clean ANY object regardless of tag

    [Header("Audio")]
    [SerializeField] private AudioSource waterAudioSource;
    [SerializeField] private AudioClip waterFillSound;
    [SerializeField] private AudioClip waterDrainSound;
    [SerializeField] private AudioClip cleaningSound;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI; // "Press E to fill / Q to drain" prompt

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private float currentWaterLevel;
    private bool isPlayerNearby = false;
    private bool isFilling = false;
    private bool isDraining = false;
    private bool hasWater = false;
    private List<GameObject> objectsInWater = new List<GameObject>();
    private Dictionary<GameObject, float> cleaningTimers = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, List<FieldInfo>> dirtyFields = new Dictionary<GameObject, List<FieldInfo>>();

    private Transform player;

    void Start()
    {
        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Initialize water at minimum level
        currentWaterLevel = minWaterLevel;
        if (waterObject != null)
        {
            waterObject.SetActive(false);
            Vector3 waterPos = waterObject.transform.localPosition;
            waterPos.y = minWaterLevel;
            waterObject.transform.localPosition = waterPos;
        }

        // Setup cleaning zone if not assigned
        if (cleaningZone == null)
        {
            // Create a trigger zone for cleaning detection
            GameObject cleanZone = new GameObject("CleaningZone");
            cleanZone.transform.SetParent(transform);
            cleanZone.transform.localPosition = Vector3.zero;
            BoxCollider trigger = cleanZone.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(1f, 0.5f, 1f); // Adjust based on sink size
            cleaningZone = cleanZone.transform;
        }

        // Hide interaction UI initially
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    void Update()
    {
        // Check player distance
        CheckPlayerProximity();

        // Handle water filling/draining
        if (isPlayerNearby && isInteractable)
        {
            HandleWaterControls();
        }

        // Update water level visually
        UpdateWaterVisual();

        // Process cleaning for objects in water
        ProcessCleaning();
    }

    void CheckPlayerProximity()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        bool wasNearby = isPlayerNearby;
        isPlayerNearby = distance <= interactionRange;

        // Show/hide interaction UI
        if (interactionUI != null && wasNearby != isPlayerNearby)
        {
            interactionUI.SetActive(isPlayerNearby);
        }
    }

    void HandleWaterControls()
    {
        // Fill water
        if (Input.GetKey(fillKey) && !isFilling && currentWaterLevel < maxWaterLevel)
        {
            StartFilling();
        }
        else if (Input.GetKeyUp(fillKey) && isFilling)
        {
            StopFilling();
        }

        // Drain water
        if (Input.GetKey(drainKey) && !isDraining && currentWaterLevel > minWaterLevel)
        {
            StartDraining();
        }
        else if (Input.GetKeyUp(drainKey) && isDraining)
        {
            StopDraining();
        }
    }

    void StartFilling()
    {
        isFilling = true;
        isDraining = false;

        // Show water object when starting to fill
        if (waterObject != null && !waterObject.activeSelf && currentWaterLevel <= minWaterLevel)
        {
            waterObject.SetActive(true);
        }

        // Play water particles
        if (waterParticles != null && !waterParticles.isPlaying)
        {
            waterParticles.Play();
        }

        // Play fill sound
        if (waterAudioSource != null && waterFillSound != null)
        {
            waterAudioSource.clip = waterFillSound;
            waterAudioSource.loop = true;
            waterAudioSource.Play();
        }
    }

    void StopFilling()
    {
        isFilling = false;

        // Stop water particles
        if (waterParticles != null && waterParticles.isPlaying)
        {
            waterParticles.Stop();
        }

        // Stop fill sound
        if (waterAudioSource != null && waterAudioSource.isPlaying)
        {
            waterAudioSource.Stop();
        }
    }

    void StartDraining()
    {
        isDraining = true;
        isFilling = false;

        // Play drain sound
        if (waterAudioSource != null && waterDrainSound != null)
        {
            waterAudioSource.clip = waterDrainSound;
            waterAudioSource.loop = true;
            waterAudioSource.Play();
        }
    }

    void StopDraining()
    {
        isDraining = false;

        // Stop drain sound
        if (waterAudioSource != null && waterAudioSource.isPlaying)
        {
            waterAudioSource.Stop();
        }
    }

    void UpdateWaterVisual()
    {
        // Update water level
        if (isFilling)
        {
            currentWaterLevel = Mathf.MoveTowards(currentWaterLevel, maxWaterLevel, waterFillSpeed * Time.deltaTime);
            hasWater = currentWaterLevel > minWaterLevel + 0.1f;
        }
        else if (isDraining)
        {
            currentWaterLevel = Mathf.MoveTowards(currentWaterLevel, minWaterLevel, waterFillSpeed * Time.deltaTime);
            hasWater = currentWaterLevel > minWaterLevel + 0.1f;
        }

        // Update water object position
        if (waterObject != null)
        {
            Vector3 waterPos = waterObject.transform.localPosition;
            waterPos.y = currentWaterLevel;
            waterObject.transform.localPosition = waterPos;

            // Hide water when fully drained
            if (currentWaterLevel <= minWaterLevel && waterObject.activeSelf)
            {
                waterObject.SetActive(false);
                hasWater = false;
            }
        }

        // Auto-stop when reaching limits
        if (isFilling && currentWaterLevel >= maxWaterLevel)
        {
            StopFilling();
        }
        else if (isDraining && currentWaterLevel <= minWaterLevel)
        {
            StopDraining();
        }
    }

    void ProcessCleaning()
    {
        if (!hasWater) return;

        // Update cleaning timers for objects in water
        List<GameObject> cleanedObjects = new List<GameObject>();

        foreach (var obj in objectsInWater)
        {
            if (obj == null)
            {
                cleanedObjects.Add(obj);
                continue;
            }

            // Update timer
            if (!cleaningTimers.ContainsKey(obj))
            {
                cleaningTimers[obj] = 0f;
            }

            cleaningTimers[obj] += Time.deltaTime;

            // Check if object has been in water long enough
            if (cleaningTimers[obj] >= cleaningDelay)
            {
                CleanObject(obj);
                cleanedObjects.Add(obj);
            }
        }

        // Remove cleaned objects from tracking
        foreach (var obj in cleanedObjects)
        {
            objectsInWater.Remove(obj);
            if (obj != null && cleaningTimers.ContainsKey(obj))
            {
                cleaningTimers.Remove(obj);
            }
        }
    }

    void CleanObject(GameObject obj)
    {
        bool wasCleaned = false;

        // Use cached fields if available
        if (dirtyFields.ContainsKey(obj) && dirtyFields[obj] != null)
        {
            foreach (var field in dirtyFields[obj])
            {
                if (field.FieldType == typeof(bool))
                {
                    field.SetValue(field.DeclaringType.IsValueType ? obj : obj.GetComponent(field.DeclaringType), false);
                    wasCleaned = true;
                    if (debugMode) Debug.Log($"Cleaned {obj.name} - Set {field.Name} to false");
                }
            }
        }

        if (wasCleaned)
        {
            // Play cleaning effect
            if (bubbleEffect != null)
            {
                ParticleSystem bubbles = Instantiate(bubbleEffect, obj.transform.position, Quaternion.identity);
                Destroy(bubbles.gameObject, 2f);
            }

            // Play cleaning sound
            if (waterAudioSource != null && cleaningSound != null)
            {
                waterAudioSource.PlayOneShot(cleaningSound);
            }

            // Send message to object (for custom cleaning logic)
            obj.SendMessage("OnCleaned", SendMessageOptions.DontRequireReceiver);

            Debug.Log($"Successfully cleaned: {obj.name}");
        }
    }

    bool IsObjectCleanable(GameObject obj)
    {
        // Check if object should be cleaned based on settings
        if (!cleanAllObjects)
        {
            // Check if object has any of the cleanable tags
            bool hasCleanableTag = false;
            foreach (string tag in cleanableTags)
            {
                if (obj.CompareTag(tag))
                {
                    hasCleanableTag = true;
                    break;
                }
            }
            if (!hasCleanableTag) return false;
        }

        // Find and cache dirty fields
        List<FieldInfo> foundFields = new List<FieldInfo>();
        Component[] components = obj.GetComponents<Component>();

        foreach (Component comp in components)
        {
            if (comp == null) continue;

            System.Type type = comp.GetType();

            // Check all fields in the component
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (FieldInfo field in fields)
            {
                // Check if field name matches any of our dirty field names
                foreach (string dirtyFieldName in dirtyFieldNames)
                {
                    if (field.Name.ToLower() == dirtyFieldName.ToLower() && field.FieldType == typeof(bool))
                    {
                        // Check if the field is currently true (dirty)
                        bool currentValue = (bool)field.GetValue(comp);
                        if (currentValue)
                        {
                            foundFields.Add(field);
                            if (debugMode) Debug.Log($"Found dirty field '{field.Name}' in {comp.GetType().Name} on {obj.name}");
                        }
                    }
                }
            }
        }

        // Cache the fields for this object
        if (foundFields.Count > 0)
        {
            dirtyFields[obj] = foundFields;
            return true;
        }

        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Skip if it's the cleaning zone itself
        if (cleaningZone != null && other.transform.IsChildOf(cleaningZone)) return;

        // Skip the player
        if (other.CompareTag("Player")) return;

        // Check if object is cleanable
        if (IsObjectCleanable(other.gameObject))
        {
            if (!objectsInWater.Contains(other.gameObject))
            {
                objectsInWater.Add(other.gameObject);
                cleaningTimers[other.gameObject] = 0f;
                Debug.Log($"Cleanable object entered sink water: {other.name}");
            }
        }
        else if (debugMode)
        {
            Debug.Log($"Object {other.name} entered sink but is not cleanable (no dirty fields found or wrong tag)");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Remove object from water tracking
        if (objectsInWater.Contains(other.gameObject))
        {
            objectsInWater.Remove(other.gameObject);
            if (cleaningTimers.ContainsKey(other.gameObject))
            {
                cleaningTimers.Remove(other.gameObject);
            }
            if (dirtyFields.ContainsKey(other.gameObject))
            {
                dirtyFields.Remove(other.gameObject);
            }
            Debug.Log($"Object left sink water: {other.name}");
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw interaction range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        // Draw cleaning zone
        if (cleaningZone != null)
        {
            Gizmos.color = Color.blue;
            BoxCollider box = cleaningZone.GetComponent<BoxCollider>();
            if (box != null)
            {
                Gizmos.DrawWireCube(cleaningZone.position, box.size);
            }
            else
            {
                Gizmos.DrawWireCube(cleaningZone.position, Vector3.one);
            }
        }
    }
}
