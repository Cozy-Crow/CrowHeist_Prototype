using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomSaveable
{
    
    void SaveCustomData(SaveableObjectData data);

    
    void LoadCustomData(SaveableObjectData data);
}


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
            //Debug.Log($"Generated unique ID for {gameObject.name}: {_uniqueId}");
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

        // ADDED: Save physics state (velocity) for active objects
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && gameObject.activeSelf)
        {
            // CRITICAL: Save the kinematic state!
            data.wasKinematic = rb.isKinematic;
            Debug.Log($"[SAVE] {gameObject.name} - isKinematic: {rb.isKinematic}");

            // Only save velocity for active objects (objects in motion)
            if (rb.velocity.magnitude > 0.01f || rb.angularVelocity.magnitude > 0.01f)
            {
                data.velocity = new Vector3Data(rb.velocity);
                data.angularVelocity = new Vector3Data(rb.angularVelocity);
                data.hadVelocity = true;
                Debug.Log($"[SAVE] {gameObject.name} - SAVING VELOCITY: {rb.velocity}, Angular: {rb.angularVelocity}");
            }
            else
            {
                Debug.Log($"[SAVE] {gameObject.name} - No significant velocity (stationary)");
            }
        }
        else if (rb != null && !gameObject.activeSelf)
        {
            // Even if inactive, save kinematic state
            data.wasKinematic = rb.isKinematic;
            Debug.Log($"[SAVE] {gameObject.name} - Inactive, isKinematic: {rb.isKinematic}");
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

    
    public void LoadData(SaveableObjectData data)
    {
        Debug.Log($"[LOAD] === Loading {gameObject.name} (ID: {data.uniqueId}) ===");
        Debug.Log($"[LOAD] Target position: {data.position.ToVector3()}, isActive: {data.isActive}, hadVelocity: {data.hadVelocity}, wasKinematic: {data.wasKinematic}");

        // CRITICAL FIX: Temporarily disable physics to prevent interference during restoration
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // CRITICAL: Use the SAVED kinematic state, not the current state!
            // The object should be restored to how it was when saved
            rb.isKinematic = true; // Make kinematic temporarily for position setting

            // IMPROVED: Only reset velocity if object didn't have velocity when saved
            // This allows thrown objects to continue their trajectory
            if (!data.hadVelocity)
            {
                // Object was stationary when saved (like a collected coin)
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Debug.Log($"[LOAD] {gameObject.name} - Resetting velocity (was stationary)");
            }
            else
            {
                // Object was in motion when saved (like a thrown object)
                // Temporarily set to zero, will restore in coroutine
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Debug.Log($"[LOAD] {gameObject.name} - Will restore velocity: {data.velocity.ToVector3()}");
            }
        }

        // CRITICAL FIX: Set transform BEFORE activating to prevent physics/triggers at wrong position
        if (_savePosition)
        {
            transform.position = data.position.ToVector3();
            Debug.Log($"[LOAD] {gameObject.name} - Position set to: {transform.position}");
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
        Debug.Log($"[LOAD] {gameObject.name} - SetActive({data.isActive})");

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
            // Use the SAVED kinematic state from data
            StartCoroutine(RestoreRigidbodyState(rb, data.wasKinematic, data));
        }
        else if (rb != null && !data.isActive)
        {
            // If object is inactive, restore kinematic state immediately
            rb.isKinematic = data.wasKinematic;
            Debug.Log($"[LOAD] {gameObject.name} - Inactive, kinematic restored to: {data.wasKinematic}");
        }

        Debug.Log($"[LOAD] {gameObject.name} - Load complete, final position: {transform.position}");
    }

   
    private IEnumerator RestoreRigidbodyState(Rigidbody rb, bool savedKinematicState, SaveableObjectData data)
    {
        Debug.Log($"[LOAD] {gameObject.name} - Waiting for physics to settle...");

        // Wait for physics to settle
        yield return new WaitForFixedUpdate();

        if (rb != null)
        {
            Debug.Log($"[LOAD] {gameObject.name} - After fixed update, position: {transform.position}");

            // Restore velocity if object had it when saved (thrown objects)
            if (data.hadVelocity)
            {
                rb.velocity = data.velocity.ToVector3();
                rb.angularVelocity = data.angularVelocity.ToVector3();
                Debug.Log($"[LOAD] {gameObject.name} - RESTORED VELOCITY: {rb.velocity}, Angular: {rb.angularVelocity}");
            }
            else
            {
                // Object was stationary, ensure velocity stays zero
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                Debug.Log($"[LOAD] {gameObject.name} - Velocity kept at zero (stationary object)");
            }

            // CRITICAL: Restore kinematic state to the SAVED state
            rb.isKinematic = savedKinematicState;

            Debug.Log($"[LOAD] {gameObject.name} - Physics FULLY RESTORED! kinematic: {savedKinematicState}, velocity: {rb.velocity}");
        }
    }

    
    public void SetUniqueId(string id)
    {
        _uniqueId = id;
    }

   
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
