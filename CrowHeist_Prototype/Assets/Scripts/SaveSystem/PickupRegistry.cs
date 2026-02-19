using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Registry that tracks all pickupable and narrative items in the game.
/// Maintains dictionaries by UniqueID for easy lookup and save/load integration.
/// </summary>
public class PickupRegistry : MonoBehaviour
{
    public static PickupRegistry Instance { get; private set; }

    // Dictionary of all registered items by their UniqueID
    private Dictionary<string, RegistryEntry> allItems = new Dictionary<string, RegistryEntry>();

    // Quick access dictionaries by object type
    private Dictionary<string, RegistryEntry> pickupableItems = new Dictionary<string, RegistryEntry>();
    private Dictionary<string, RegistryEntry> narrativeItems = new Dictionary<string, RegistryEntry>();
    private Dictionary<string, RegistryEntry> collectibleItems = new Dictionary<string, RegistryEntry>();

    // Events for when items are registered or state changes
    public event Action<RegistryEntry> OnItemRegistered;
    public event Action<RegistryEntry> OnItemPickedUp;
    public event Action<RegistryEntry> OnNarrativeRead;
    public event Action<RegistryEntry> OnNarrativeItemCollected;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Auto-register all items with UniqueID in the scene
        RegisterAllSceneItems();
    }


    // Finds and registers all items with UniqueID component in the scene.
    
    public void RegisterAllSceneItems()
    {
        UniqueID[] allUniqueItems = FindObjectsOfType<UniqueID>();
        foreach (UniqueID item in allUniqueItems)
        {
            RegisterItem(item);
        }
        Debug.Log($"PickupRegistry: Registered {allItems.Count} items ({pickupableItems.Count} pickupables, {narrativeItems.Count} narrative, {collectibleItems.Count} collectibles)");
    }

  
    public void RegisterItem(UniqueID uniqueID)
    {
        if (uniqueID == null) return;

        string id = uniqueID.ID;
        if (allItems.ContainsKey(id))
        {
            // Already registered, update the reference
            allItems[id].UniqueID = uniqueID;
            return;
        }

        RegistryEntry entry = new RegistryEntry
        {
            UniqueID = uniqueID,
            ObjectType = uniqueID.ObjectType,
            ItemData = uniqueID.ItemData,
            HasBeenPickedUp = false,
            HasBeenRead = false,
            PickupCount = 0
        };

        allItems[id] = entry;

        // Add to type-specific dictionaries
        switch (uniqueID.ObjectType)
        {
            case ObjectType.Pickupable:
                pickupableItems[id] = entry;
                break;
            case ObjectType.Narrative:
                narrativeItems[id] = entry;
                break;
            case ObjectType.Collectible:
                collectibleItems[id] = entry;
                break;
        }

        OnItemRegistered?.Invoke(entry);
    }

    /// <summary>
    /// Unregisters an item from the registry.
    /// </summary>
    public void UnregisterItem(string id)
    {
        if (allItems.TryGetValue(id, out RegistryEntry entry))
        {
            allItems.Remove(id);
            pickupableItems.Remove(id);
            narrativeItems.Remove(id);
            collectibleItems.Remove(id);
        }
    }

    
  
    public RegistryEntry GetEntry(string id)
    {
        allItems.TryGetValue(id, out RegistryEntry entry);
        return entry;
    }

  
    public RegistryEntry GetEntry(UniqueID uniqueID)
    {
        if (uniqueID == null) return null;
        return GetEntry(uniqueID.ID);
    }

   
    public void MarkAsPickedUp(string id)
    {
        if (allItems.TryGetValue(id, out RegistryEntry entry))
        {
            entry.HasBeenPickedUp = true;
            entry.PickupCount++;
            if (entry.ObjectType == ObjectType.Narrative && entry.ItemData != null)
            {
                entry.ItemData.isPickedUp = true;
            }
            OnItemPickedUp?.Invoke(entry);
        }
    }


    public void MarkNarrativeAsRead(string id)
    {
        if (allItems.TryGetValue(id, out RegistryEntry entry))
        {
            if (entry.ObjectType == ObjectType.Narrative)
            {
                entry.HasBeenRead = true;
                if (entry.ItemData != null)
                {
                    entry.ItemData.MarkAsRead();
                }
                OnNarrativeRead?.Invoke(entry);
            }
        }
    }

    /// <summary>
    /// Marks a narrative item as collected (deposited at HeistZone).
    /// This unlocks the item's information in the narrative menu.
    /// </summary>
    public void MarkNarrativeAsCollected(string id)
    {
        if (allItems.TryGetValue(id, out RegistryEntry entry))
        {
            if (entry.ObjectType == ObjectType.Narrative)
            {
                entry.HasBeenPickedUp = true;
                entry.HasBeenRead = true;
                entry.PickupCount++;
                if (entry.ItemData != null)
                {
                    entry.ItemData.MarkAsRead();
                }
                OnNarrativeItemCollected?.Invoke(entry);
                Debug.Log($"PickupRegistry: Narrative item collected - {entry.ItemData?.ItemName ?? id}");
            }
        }
    }

    /// <summary>
    /// Marks a narrative item as collected by UniqueID component reference.
    /// </summary>
    public void MarkNarrativeAsCollected(UniqueID uniqueID)
    {
        if (uniqueID != null)
        {
            MarkNarrativeAsCollected(uniqueID.ID);
        }
    }

  
    public bool HasBeenPickedUp(string id)
    {
        if (allItems.TryGetValue(id, out RegistryEntry entry))
        {
            return entry.HasBeenPickedUp;
        }
        return false;
    }

    
    public bool HasBeenRead(string id)
    {
        if (allItems.TryGetValue(id, out RegistryEntry entry))
        {
            return entry.HasBeenRead;
        }
        return false;
    }

  
    public Dictionary<string, RegistryEntry> GetAllPickupables()
    {
        return new Dictionary<string, RegistryEntry>(pickupableItems);
    }

   
    public Dictionary<string, RegistryEntry> GetAllNarrativeItems()
    {
        return new Dictionary<string, RegistryEntry>(narrativeItems);
    }

  
    public Dictionary<string, RegistryEntry> GetAllCollectibles()
    {
        return new Dictionary<string, RegistryEntry>(collectibleItems);
    }

    
    public Dictionary<string, RegistryEntry> GetAllItems()
    {
        return new Dictionary<string, RegistryEntry>(allItems);
    }

   
    public RegistrySaveData GenerateSaveData()
    {
        RegistrySaveData saveData = new RegistrySaveData();

        foreach (var kvp in allItems)
        {
            RegistryEntrySaveData entrySaveData = new RegistryEntrySaveData
            {
                UniqueID = kvp.Key,
                ObjectType = (int)kvp.Value.ObjectType,
                HasBeenPickedUp = kvp.Value.HasBeenPickedUp,
                HasBeenRead = kvp.Value.HasBeenRead,
                PickupCount = kvp.Value.PickupCount
            };
            saveData.Entries.Add(entrySaveData);
        }

        return saveData;
    }

   
    // Loads registry state from save data.
  
    public void LoadFromSaveData(RegistrySaveData saveData)
    {
        if (saveData == null || saveData.Entries == null) return;

        foreach (RegistryEntrySaveData entrySaveData in saveData.Entries)
        {
            if (allItems.TryGetValue(entrySaveData.UniqueID, out RegistryEntry entry))
            {
                entry.HasBeenPickedUp = entrySaveData.HasBeenPickedUp;
                entry.HasBeenRead = entrySaveData.HasBeenRead;
                entry.PickupCount = entrySaveData.PickupCount;

                // Update ItemData if present
                if (entry.ItemData != null && entry.HasBeenRead)
                {
                    entry.ItemData.MarkAsRead();
                }
            }
        }
        Debug.Log($"PickupRegistry: Loaded state for {saveData.Entries.Count} items");
    }

   
    public void ResetAllStates()
    {
        foreach (var kvp in allItems)
        {
            kvp.Value.HasBeenPickedUp = false;
            kvp.Value.HasBeenRead = false;
            kvp.Value.PickupCount = 0;

            if (kvp.Value.ItemData != null)
            {
                kvp.Value.ItemData.ResetReadState();
            }
        }
    }

  
    public void ClearRegistry()
    {
        allItems.Clear();
        pickupableItems.Clear();
        narrativeItems.Clear();
        collectibleItems.Clear();
    }
}

/// Registry entry containing item data and state.

[System.Serializable]
public class RegistryEntry
{
    public UniqueID UniqueID;
    public ObjectType ObjectType;
    public ItemDataSO ItemData;
    public bool HasBeenPickedUp;
    public bool HasBeenRead;
    public int PickupCount;

    public string ID => UniqueID != null ? UniqueID.ID : string.Empty;
    public GameObject GameObject => UniqueID != null ? UniqueID.gameObject : null;
}

[System.Serializable]
public class RegistrySaveData
{
    public List<RegistryEntrySaveData> Entries = new List<RegistryEntrySaveData>();
}

[System.Serializable]
public class RegistryEntrySaveData
{
    public string UniqueID;
    public int ObjectType;
    public bool HasBeenPickedUp;
    public bool HasBeenRead;
    public int PickupCount;
}
