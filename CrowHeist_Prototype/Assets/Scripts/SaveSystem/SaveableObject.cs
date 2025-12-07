using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for components that need to save custom state data
/// Implement this on components like PixieDust to save/load custom state
/// </summary>
public interface ICustomSaveable
{
    /// <summary>
    /// Save custom state to the provided data object
    /// </summary>
    void SaveCustomData(SaveableObjectData data);

    /// <summary>
    /// Load custom state from the provided data object
    /// </summary>
    void LoadCustomData(SaveableObjectData data);
}

/// <summary>
/// Component to mark objects as saveable and handle their save/load logic
/// Attach this to any object that should be saved (coins, pickable items, etc.)
/// </summary>
public class SaveableObject : MonoBehaviour
{
    [Header("Save Settings")]
    [Tooltip("Unique identifier for this object. Leave empty to auto-generate.")]
    [SerializeField] private string _uniqueId;

    [Tooltip("Type of object (Coin, AltCoin, Pickable, etc.)")]
    [SerializeField] private string _objectType;

    [Tooltip("Should this object's position be saved?")]
    [SerializeField] private bool _savePosition = true;

    [Tooltip("Should this object's rotation be saved?")]
    [SerializeField] private bool _saveRotation = true;

    [Tooltip("Should this object's scale be saved?")]
    [SerializeField] private bool _saveScale = false;

    [Header("Optional Component References")]
    [Tooltip("For coins - will save the coin value")]
    [SerializeField] private Coins _coinsComponent;

    [Tooltip("For alt coins - will save the coin value")]
    [SerializeField] private AltWinCoins _altCoinsComponent;

    public string UniqueId => _uniqueId;
    public string ObjectType => _objectType;

    // Cache for custom saveable components
    private ICustomSaveable[] _customSaveables;

    private void Awake()
    {
        // Generate unique ID if not set
        if (string.IsNullOrEmpty(_uniqueId))
        {
            _uniqueId = System.Guid.NewGuid().ToString();
            Debug.Log($"Generated unique ID for {gameObject.name}: {_uniqueId}");
        }

        // Auto-detect object type if not set
        if (string.IsNullOrEmpty(_objectType))
        {
            DetectObjectType();
        }

        // Auto-find components if not assigned
        if (_coinsComponent == null)
        {
            _coinsComponent = GetComponent<Coins>();
        }

        if (_altCoinsComponent == null)
        {
            _altCoinsComponent = GetComponent<AltWinCoins>();
        }

        // Find all components that implement ICustomSaveable
        _customSaveables = GetComponents<ICustomSaveable>();
    }

    private void DetectObjectType()
    {
        if (GetComponent<Coins>() != null)
        {
            _objectType = "Coin";
        }
        else if (GetComponent<AltWinCoins>() != null)
        {
            _objectType = "AltCoin";
        }
        else if (GetComponent<IPickupable>() != null)
        {
            _objectType = "Pickable";
        }
        else
        {
            _objectType = "Generic";
        }
    }

    /// <summary>
    /// Gets the save data for this object
    /// </summary>
    public SaveableObjectData GetSaveData()
    {
        SaveableObjectData data = new SaveableObjectData();

        data.uniqueId = _uniqueId;
        data.objectType = _objectType;
        data.isActive = gameObject.activeSelf;

        // Save transform data
        if (_savePosition)
        {
            data.position = new Vector3Data(transform.position);
        }

        if (_saveRotation)
        {
            data.rotation = new Vector3Data(transform.rotation.eulerAngles);
        }

        if (_saveScale)
        {
            data.scale = new Vector3Data(transform.localScale);
        }

        // Save component-specific data
        if (_coinsComponent != null)
        {
            data.intValue = _coinsComponent.CoinValue;
        }
        else if (_altCoinsComponent != null)
        {
            // AltWinCoins doesn't expose its value, but we can save a default
            data.intValue = 1;
        }

        // Save tag
        data.stringValue = gameObject.tag;

        // ADDED: Save custom data from components implementing ICustomSaveable
        if (_customSaveables != null && _customSaveables.Length > 0)
        {
            foreach (var customSaveable in _customSaveables)
            {
                if (customSaveable != null)
                {
                    customSaveable.SaveCustomData(data);
                }
            }
        }

        return data;
    }

    /// <summary>
    /// Loads data into this object
    /// </summary>
    public void LoadData(SaveableObjectData data)
    {
        // CRITICAL FIX: Temporarily disable physics to prevent interference during restoration
        Rigidbody rb = GetComponent<Rigidbody>();
        bool wasKinematic = false;
        if (rb != null)
        {
            wasKinematic = rb.isKinematic;
            rb.isKinematic = true;
            // CRITICAL: Reset velocity to prevent old physics forces from moving the object
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // CRITICAL FIX: Set transform BEFORE activating to prevent physics/triggers at wrong position
        if (_savePosition)
        {
            transform.position = data.position.ToVector3();
        }

        if (_saveRotation)
        {
            transform.rotation = Quaternion.Euler(data.rotation.ToVector3());
        }

        if (_saveScale)
        {
            transform.localScale = data.scale.ToVector3();
        }

        // NOW restore active state (after position is set)
        gameObject.SetActive(data.isActive);

        // Restore component-specific data
        if (_coinsComponent != null && data.intValue > 0)
        {
            _coinsComponent.CoinValue = data.intValue;
        }

        // ADDED: Load custom data to components implementing ICustomSaveable
        if (_customSaveables != null && _customSaveables.Length > 0)
        {
            foreach (var customSaveable in _customSaveables)
            {
                if (customSaveable != null)
                {
                    customSaveable.LoadCustomData(data);
                }
            }
        }

        // CRITICAL FIX: Restore rigidbody kinematic state after a frame to let physics settle
        if (rb != null && data.isActive)
        {
            // Only restore non-kinematic if object is active
            StartCoroutine(RestoreRigidbodyState(rb, wasKinematic));
        }
        else if (rb != null && !data.isActive)
        {
            // If object is inactive, restore kinematic state immediately
            rb.isKinematic = wasKinematic;
        }

        Debug.Log($"Loaded data for {gameObject.name} (ID: {_uniqueId}) at position {data.position.ToVector3()}");
    }

    /// <summary>
    /// Restore rigidbody state after physics has settled
    /// </summary>
    private IEnumerator RestoreRigidbodyState(Rigidbody rb, bool wasKinematic)
    {
        // Wait for physics to settle
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            // Reset velocity again just to be safe
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Restore kinematic state
            rb.isKinematic = wasKinematic;

            Debug.Log($"Restored physics for {gameObject.name}, kinematic: {wasKinematic}");
        }
    }

    /// <summary>
    /// Set a custom unique ID (useful for prefab instances)
    /// </summary>
    public void SetUniqueId(string id)
    {
        _uniqueId = id;
    }

    /// <summary>
    /// Set the object type
    /// </summary>
    public void SetObjectType(string type)
    {
        _objectType = type;
    }

#if UNITY_EDITOR
    // Editor helper to generate ID in inspector
    [ContextMenu("Generate New Unique ID")]
    private void GenerateNewId()
    {
        _uniqueId = System.Guid.NewGuid().ToString();
        Debug.Log($"Generated new ID: {_uniqueId}");
    }

    [ContextMenu("Detect Object Type")]
    private void DetectTypeInEditor()
    {
        DetectObjectType();
        Debug.Log($"Detected object type: {_objectType}");
    }
#endif
}
