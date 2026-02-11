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
    [SerializeField, TextArea(3, 5)] private string description;
    [SerializeField] private ObjectType itemType = ObjectType.Pickupable;

    [Header("Visual Display")]
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject previewModelPrefab;

    [Header("Narrative Data (for Narrative items)")]
    [SerializeField, TextArea(5, 10)] private string narrativeText;
    [SerializeField] private bool hasBeenRead = false;

    [Header("Pickup Settings")]
    [SerializeField] private bool showOnFirstPickup = true;
    [SerializeField] private float displayDuration = 2f;

    // Properties
    public string ItemName => itemName;
    public string Description => description;
    public ObjectType ItemType => itemType;
    public Sprite Icon => icon;
    public GameObject PreviewModelPrefab => previewModelPrefab;
    public string NarrativeText => narrativeText;
    public bool HasBeenRead { get => hasBeenRead; set => hasBeenRead = value; }
    public bool ShowOnFirstPickup => showOnFirstPickup;
    public float DisplayDuration => displayDuration;

    // Setters for editor/runtime creation
    public void SetItemName(string name) => itemName = name;
    public void SetDescription(string desc) => description = desc;
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
    /// Reset the read state.
    /// </summary>
    public void ResetReadState()
    {
        hasBeenRead = false;
    }
}
