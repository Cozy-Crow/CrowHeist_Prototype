using System;
using UnityEngine;

/// <summary>
/// Enum defining the types of objects that can have unique IDs.
/// Used to categorize items for the save/load system and UI display.
/// </summary>
public enum ObjectType
{
    None,
    Pickupable,
    Narrative,
    Collectible,
    Interactable,
    Key
}

/// <summary>
/// Component that provides a unique identifier for game objects.
/// Used by the save/load system to track individual items.
/// </summary>
public class UniqueID : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Unique identifier for this object. Generated automatically.")]
    private string id = string.Empty;

    [SerializeField]
    [Tooltip("The type of object this is (Pickupable, Narrative, Collectible, etc.)")]
    private ObjectType objectType = ObjectType.None;

    [SerializeField]
    [Tooltip("Reference to the ScriptableObject containing display data for this item.")]
    private ItemDataSO itemData;

    public string ID
    {
        get
        {
            if (string.IsNullOrEmpty(id))
            {
                GenerateID();
            }
            return id;
        }
        private set => id = value;
    }

    public ObjectType ObjectType => objectType;

    public ItemDataSO ItemData => itemData;

    private void Awake()
    {
        // Ensure we have an ID
        if (string.IsNullOrEmpty(id))
        {
            GenerateID();
        }
    }

    private void OnValidate()
    {
        // Generate ID in editor if it doesn't exist
        if (string.IsNullOrEmpty(id))
        {
            GenerateID();
        }
    }

    /// <summary>
    /// Generates a new unique identifier using GUID.
    /// </summary>
    [ContextMenu("Generate New ID")]
    public void GenerateID()
    {
        id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Manually set the ID. Use with caution - IDs should be unique!
    /// </summary>
    public void SetID(string newID)
    {
        if (!string.IsNullOrEmpty(newID))
        {
            id = newID;
        }
    }
}
