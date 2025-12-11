using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveSystemInitializer : MonoBehaviour
{
    [Header("Auto-Save Settings")]
    [Tooltip("Automatically find and mark Coin objects as saveable")]
    [SerializeField] private bool autoMarkCoins = true;

    [Tooltip("Automatically find and mark AltCoin objects as saveable")]
    [SerializeField] private bool autoMarkAltCoins = true;

    [Tooltip("Automatically find and mark Pickupable objects as saveable")]
    [SerializeField] private bool autoMarkPickupables = true;

    [Tooltip("Additional tags to automatically mark as saveable")]
    [SerializeField] private List<string> customTagsToSave = new List<string>();

    [Header("ID Generation")]
    [Tooltip("Prefix for auto-generated IDs (helps with debugging)")]
    [SerializeField] private string idPrefix = "";

    [Header("Debug")]
    [Tooltip("Log information about initialized objects")]
    [SerializeField] private bool verboseLogging = true;

    private void Awake()
    {
        // Initialize save system on scene load
        InitializeSaveSystem();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-initialize when a new scene loads
        StartCoroutine(InitializeAfterSceneLoad());
    }

    private IEnumerator InitializeAfterSceneLoad()
    {
        // Wait for scene to fully load
        yield return new WaitForEndOfFrame();
        InitializeSaveSystem();
    }

    
    private void InitializeSaveSystem()
    {
        int totalObjectsMarked = 0;

        if (verboseLogging)
        {
            Debug.Log($"[SaveSystemInitializer] Initializing save system for scene: {SceneManager.GetActiveScene().name}");
        }

        // Find and mark coins
        if (autoMarkCoins)
        {
            int marked = MarkObjectsWithComponent<Coins>("Coin");
            totalObjectsMarked += marked;
            if (verboseLogging) Debug.Log($"[SaveSystemInitializer] Marked {marked} Coin objects");
        }

        // Find and mark alt coins
        if (autoMarkAltCoins)
        {
            int marked = MarkObjectsWithComponent<AltWinCoins>("AltCoin");
            totalObjectsMarked += marked;
            if (verboseLogging) Debug.Log($"[SaveSystemInitializer] Marked {marked} AltCoin objects");
        }

        // Find and mark pickupable objects
        if (autoMarkPickupables)
        {
            int marked = MarkPickupableObjects();
            totalObjectsMarked += marked;
            if (verboseLogging) Debug.Log($"[SaveSystemInitializer] Marked {marked} Pickupable objects");
        }

        // Mark objects with custom tags
        foreach (string tag in customTagsToSave)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                int marked = MarkObjectsByTag(tag);
                totalObjectsMarked += marked;
                if (verboseLogging) Debug.Log($"[SaveSystemInitializer] Marked {marked} objects with tag '{tag}'");
            }
        }

        if (verboseLogging)
        {
            Debug.Log($"[SaveSystemInitializer] Total objects marked for saving: {totalObjectsMarked}");
        }
    }

    
    private int MarkObjectsWithComponent<T>(string objectType) where T : Component
    {
        T[] objects = FindObjectsOfType<T>();
        int markedCount = 0;

        foreach (T obj in objects)
        {
            if (AddSaveableComponent(obj.gameObject, objectType))
            {
                markedCount++;
            }
        }

        return markedCount;
    }

    
    private int MarkPickupableObjects()
    {
        // Find all MonoBehaviours and check if they implement IPickupable
        MonoBehaviour[] allObjects = FindObjectsOfType<MonoBehaviour>();
        int markedCount = 0;

        foreach (MonoBehaviour obj in allObjects)
        {
            if (obj is IPickupable)
            {
                if (AddSaveableComponent(obj.gameObject, "Pickable"))
                {
                    markedCount++;
                }
            }
        }

        return markedCount;
    }

    
    private int MarkObjectsByTag(string tag)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        int markedCount = 0;

        foreach (GameObject obj in objects)
        {
            if (AddSaveableComponent(obj, tag))
            {
                markedCount++;
            }
        }

        return markedCount;
    }

    
    private bool AddSaveableComponent(GameObject obj, string objectType)
    {
        // Check if already has SaveableObject
        SaveableObject existing = obj.GetComponent<SaveableObject>();
        if (existing != null)
        {
            // Already has component, just ensure it has an ID and type
            if (string.IsNullOrEmpty(existing.UniqueId))
            {
                existing.SetUniqueId(GenerateUniqueId(obj, objectType));
            }
            if (string.IsNullOrEmpty(existing.ObjectType))
            {
                existing.SetObjectType(objectType);
            }
            return false; // Didn't add new component
        }

        // Add new SaveableObject component
        SaveableObject saveable = obj.AddComponent<SaveableObject>();

        // Generate unique ID
        string uniqueId = GenerateUniqueId(obj, objectType);
        saveable.SetUniqueId(uniqueId);
        saveable.SetObjectType(objectType);

        return true; // Added new component
    }

    
    private string GenerateUniqueId(GameObject obj, string objectType)
    {
        // Create a deterministic ID based on object properties
        // This ensures the same object gets the same ID across play sessions
        string positionHash = $"{obj.transform.position.x:F2}_{obj.transform.position.y:F2}_{obj.transform.position.z:F2}";
        string sceneName = SceneManager.GetActiveScene().name;

        // Combine into unique ID
        string baseId = $"{idPrefix}{sceneName}_{objectType}_{obj.name}_{positionHash}";

        // Add instance ID for uniqueness
        string uniqueId = $"{baseId}_{obj.GetInstanceID()}";

        return uniqueId;
    }

    
    [ContextMenu("Initialize Save System Now")]
    public void InitializeNow()
    {
        InitializeSaveSystem();
    }

    
    [ContextMenu("Remove All Saveable Components")]
    public void RemoveAllSaveableComponents()
    {
        SaveableObject[] saveables = FindObjectsOfType<SaveableObject>();
        int count = saveables.Length;

        foreach (SaveableObject saveable in saveables)
        {
            if (Application.isPlaying)
            {
                Destroy(saveable);
            }
            else
            {
                DestroyImmediate(saveable);
            }
        }

        Debug.Log($"[SaveSystemInitializer] Removed {count} SaveableObject components");
    }
}
