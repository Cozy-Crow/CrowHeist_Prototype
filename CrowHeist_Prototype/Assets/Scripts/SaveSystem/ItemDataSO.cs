using UnityEngine;

/// <summary>
/// ScriptableObject containing display and metadata for pickupable/narrative items.
/// Used by PickupVisualManager and PickupRegistry for UI display and tracking.
/// </summary>
[CreateAssetMenu(fileName = "New Item Data", menuName = "Items/ItemDataSO")]
public class ItemDataSO : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string itemName;
    [SerializeField, TextArea(3, 5)] private string[] descriptions;
    [SerializeField] private ObjectType itemType = ObjectType.Pickupable;

    [Header("Visual Display")]
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject previewModelPrefab;

    [Header("Narrative Data (for Narrative items)")]
    [SerializeField, TextArea(5, 10)] private string narrativeText;
    [SerializeField] private bool hasBeenRead = false;

    [Header("Narrative Menu Display")]
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private string locationHint;

    [Header("Pickup Settings")]
    [SerializeField] private bool showOnFirstPickup = true;
    [SerializeField] private float displayDuration = 2f;

    // Runtime state for narrative menu (stage 1: picked up, stage 2: hasBeenRead)
    [HideInInspector] public bool isPickedUp = false;
    private int collectedDescIndex = 0;
    private const string DEFAULT_TEXT = "???";

    // Properties
    public string ItemName => itemName;

    /// <summary>
    /// Returns a random description from the descriptions array.
    /// </summary>
    public string Description
    {
        get
        {
            if (descriptions == null || descriptions.Length == 0)
                return string.Empty;
            if (descriptions.Length == 1)
                return descriptions[0];
            return descriptions[Random.Range(0, descriptions.Length)];
        }
    }

    /// <summary>
    /// Returns the full array of descriptions.
    /// </summary>
    public string[] Descriptions => descriptions;

    /// <summary>
    /// Returns descriptions[0] — the only description shown before the item is collected.
    /// </summary>
    public string FirstDescription
    {
        get
        {
            if (descriptions == null || descriptions.Length == 0)
                return string.Empty;
            return descriptions[0];
        }
    }

    /// <summary>
    /// Returns descriptions[1..n] — the descriptions unlocked after the item is collected.
    /// descriptions[0] is no longer shown once collected.
    /// </summary>
    public string[] CollectedDescriptions
    {
        get
        {
            if (descriptions == null || descriptions.Length <= 1)
                return new string[0];
            string[] collected = new string[descriptions.Length - 1];
            System.Array.Copy(descriptions, 1, collected, 0, collected.Length);
            return collected;
        }
    }

    public ObjectType ItemType => itemType;
    public Sprite Icon => icon;
    public Sprite LockedIcon { get => lockedIcon; set => lockedIcon = value; }
    public string LocationHint { get => locationHint; set => locationHint = value; }
    public GameObject PreviewModelPrefab => previewModelPrefab;
    public string NarrativeText => narrativeText;
    public bool HasBeenRead { get => hasBeenRead; set => hasBeenRead = value; }
    public bool ShowOnFirstPickup => showOnFirstPickup;
    public float DisplayDuration => displayDuration;

    // Setters for editor/runtime creation
    public void SetItemName(string name) => itemName = name;
    public void SetDescription(string desc) => descriptions = new string[] { desc };
    public void SetDescriptions(string[] descs) => descriptions = descs;
    public void SetItemType(ObjectType type) => itemType = type;
    public void SetIcon(Sprite newIcon) => icon = newIcon;
    public void SetPreviewModel(GameObject model) => previewModelPrefab = model;
    public void SetNarrativeText(string text) => narrativeText = text;

    /// <summary>
    /// Returns true if this item is a narrative item type.
    /// </summary>
    public bool IsNarrativeItem => itemType == ObjectType.Narrative;

    /// <summary>
    /// Returns true if this item is a pickupable item type.
    /// </summary>
    public bool IsPickupableItem => itemType == ObjectType.Pickupable;

    /// <summary>
    /// Mark the narrative item as read.
    /// </summary>
    public void MarkAsRead()
    {
        if (IsNarrativeItem)
        {
            hasBeenRead = true;
        }
    }

    /// <summary>
    /// Reset the read state and pickup state.
    /// </summary>
    public void ResetReadState()
    {
        hasBeenRead = false;
        isPickedUp = false;
    }

    // --- Narrative Menu Display Properties ---

    /// <summary>
    /// Returns item name if picked up or collected, otherwise "???".
    /// </summary>
    public string DisplayName
    {
        get
        {
            if (isPickedUp || hasBeenRead)
                return itemName;
            return DEFAULT_TEXT;
        }
    }

    /// <summary>
    /// Returns unlocked icon if picked up or collected, otherwise locked icon.
    /// </summary>
    public Sprite DisplayIcon
    {
        get
        {
            if (isPickedUp || hasBeenRead)
                return icon;
            return lockedIcon;
        }
    }

    /// <summary>
    /// Staged description display:
    /// Locked    → "???"
    /// PickedUp  → descriptions[0] only
    /// Collected → descriptions[1..n] joined
    /// </summary>
    public string DisplayDescription
    {
        get
        {
            if (hasBeenRead)
            {
                string[] collected = CollectedDescriptions;
                if (collected.Length == 0) return "";
                return collected[collectedDescIndex % collected.Length];
            }
            else if (isPickedUp)
            {
                return FirstDescription;
            }
            return DEFAULT_TEXT;
        }
    }

    public void CycleDescription()
    {
        string[] collected = CollectedDescriptions;
        if (collected.Length > 0)
        {
            collectedDescIndex = (collectedDescIndex + 1) % collected.Length;
        }
    }
}
