using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

/// <summary>
/// Backend for the narrative menu. Subscribes to PickupRegistry events to
/// unlock item information when Crowley picks up or collects items.
///
/// First screen: grid of item slots (locked/unlocked icons).
/// Second screen: detail view for the selected item (name, description, icon).
/// </summary>
public class NarrativeMenu : MonoBehaviour
{
    public static NarrativeMenu Instance { get; private set; }

    [Header("Menu Panels")]
    [SerializeField] private GameObject narrativeMenuFirst;
    [SerializeField] private GameObject narrativeMenuSecond;

    [Header("First Screen - Item Grid")]
    [SerializeField] private Transform itemGridParent;
    [SerializeField] private NarrativeMenuEntry[] itemSlots;

    [Header("Second Screen - Detail View")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDescription;
    [SerializeField] private TMP_Text detailNarrativeText;
    [SerializeField] private Button backButton;

    [Header("Locked Display")]
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private string lockedDisplayName = "???";
    [SerializeField] private string lockedDescription = "???";

    private bool isFirstMenuActive = false;
    private bool isSecondMenuActive = false;

    // Ordered list of all narrative item IDs that this menu tracks.
    // Populated on Start from the PickupRegistry's narrative items dictionary.
    private List<string> trackedItemIDs = new List<string>();

    // Maps item IDs to their unlock state for fast lookup.
    private Dictionary<string, bool> unlockedItems = new Dictionary<string, bool>();

    // The currently selected item index (for detail view).
    private int selectedIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Wire up back button
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToItemGrid);
        }

        isFirstMenuActive = false;
        isSecondMenuActive = false;
        narrativeMenuFirst.SetActive(false);
        narrativeMenuSecond.SetActive(false);

        // Subscribe to PickupRegistry events
        if (PickupRegistry.Instance != null)
        {
            PickupRegistry.Instance.OnItemPickedUp += OnItemPickedUp;
            PickupRegistry.Instance.OnNarrativeItemCollected += OnNarrativeItemCollected;

            // Build the tracked items list from all registered narrative items
            PopulateTrackedItems();
        }
        else
        {
            Debug.LogWarning("NarrativeMenu: PickupRegistry.Instance is null at Start. Will retry on menu open.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events to prevent leaks
        if (PickupRegistry.Instance != null)
        {
            PickupRegistry.Instance.OnItemPickedUp -= OnItemPickedUp;
            PickupRegistry.Instance.OnNarrativeItemCollected -= OnNarrativeItemCollected;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }
    }

    // ──────────────────────────────────────────────
    //  Registry Event Handlers
    // ──────────────────────────────────────────────

    /// <summary>
    /// Called when any item is picked up for the first time.
    /// For pickupable items this triggers the pickup UI element via PickupVisualManager
    /// (already handled in Pickable.cs). Here we track it in our dictionary
    /// so the narrative menu can show "discovered" state for pickupables.
    /// </summary>
    private void OnItemPickedUp(RegistryEntry entry)
    {
        if (entry == null) return;

        string id = entry.ID;

        // Track all pickupable first-pickups (the pickup UI anim is handled by Pickable.cs)
        if (entry.ObjectType == ObjectType.Pickupable && !unlockedItems.ContainsKey(id))
        {
            unlockedItems[id] = true;
            Debug.Log($"NarrativeMenu: Pickupable first-pickup tracked - {entry.ItemData?.ItemName ?? id}");
        }
    }

    /// <summary>
    /// Called when a narrative item is collected (deposited at the HeistZone).
    /// This unlocks the item's full information in the narrative menu.
    /// </summary>
    private void OnNarrativeItemCollected(RegistryEntry entry)
    {
        if (entry == null) return;

        string id = entry.ID;
        unlockedItems[id] = true;
        Debug.Log($"NarrativeMenu: Narrative item unlocked - {entry.ItemData?.ItemName ?? id}");

        // Refresh the grid if the menu is currently open
        if (isFirstMenuActive)
        {
            RefreshItemGrid();
        }
    }

    // ──────────────────────────────────────────────
    //  Menu Toggle & Navigation
    // ──────────────────────────────────────────────

    private void ToggleMenu()
    {
        if (isSecondMenuActive)
        {
            // Close detail view
            isSecondMenuActive = false;
            narrativeMenuSecond.SetActive(false);
        }
        else if (isFirstMenuActive)
        {
            // Close item grid
            isFirstMenuActive = false;
            narrativeMenuFirst.SetActive(false);
        }
        else
        {
            // Don't open if the pause menu is already open (check both the flag and the UI state
            // so the narrative menu stays closed even when the pause menu is shown as a main menu
            // with isGamePaused still false)
            PauseManager pauseManager = FindObjectOfType<PauseManager>();
            bool pauseMenuActive = pauseManager != null &&
                (pauseManager.isGamePaused ||
                 (pauseManager.PauseMenu != null && pauseManager.PauseMenu.activeSelf));
            if (pauseMenuActive)
                return;

            // Open item grid
            // Late-bind registry if it wasn't available at Start
            if (trackedItemIDs.Count == 0 && PickupRegistry.Instance != null)
            {
                PickupRegistry.Instance.OnItemPickedUp -= OnItemPickedUp;
                PickupRegistry.Instance.OnNarrativeItemCollected -= OnNarrativeItemCollected;
                PickupRegistry.Instance.OnItemPickedUp += OnItemPickedUp;
                PickupRegistry.Instance.OnNarrativeItemCollected += OnNarrativeItemCollected;
                PopulateTrackedItems();
            }

            isFirstMenuActive = true;
            narrativeMenuFirst.SetActive(true);
            RefreshItemGrid();
        }
    }

    /// <summary>
    /// Immediately closes all narrative menu panels.
    /// Called by PauseManager when the pause menu opens.
    /// </summary>
    public void ForceClose()
    {
        isFirstMenuActive = false;
        isSecondMenuActive = false;
        if (narrativeMenuFirst != null) narrativeMenuFirst.SetActive(false);
        if (narrativeMenuSecond != null) narrativeMenuSecond.SetActive(false);
    }

    /// <summary>
    /// Navigate from detail view back to item grid.
    /// </summary>
    public void BackToItemGrid()
    {
        isSecondMenuActive = false;
        isFirstMenuActive = true;
        narrativeMenuSecond.SetActive(false);
        narrativeMenuFirst.SetActive(true);
        RefreshItemGrid();
    }

    /// <summary>
    /// Called when an item slot is clicked. Opens the detail view for that item.
    /// </summary>
    public void SelectItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= trackedItemIDs.Count) return;

        string id = trackedItemIDs[slotIndex];
        bool isUnlocked = unlockedItems.ContainsKey(id) && unlockedItems[id];

        if (!isUnlocked) return; // Don't open locked items

        selectedIndex = slotIndex;

        // Switch to detail screen
        isFirstMenuActive = false;
        isSecondMenuActive = true;
        narrativeMenuFirst.SetActive(false);
        narrativeMenuSecond.SetActive(true);

        // Populate detail view
        RefreshDetailView(id);
    }

    // ──────────────────────────────────────────────
    //  Data Population
    // ──────────────────────────────────────────────

    /// <summary>
    /// Builds the tracked items list from all narrative items registered in the PickupRegistry.
    /// Also restores unlock state from the registry (e.g. after loading a save).
    /// </summary>
    private void PopulateTrackedItems()
    {
        trackedItemIDs.Clear();
        unlockedItems.Clear();

        if (PickupRegistry.Instance == null) return;

        Dictionary<string, RegistryEntry> narrativeEntries = PickupRegistry.Instance.GetAllNarrativeItems();
        foreach (var kvp in narrativeEntries)
        {
            trackedItemIDs.Add(kvp.Key);

            // Restore unlock state from registry (for save/load support)
            if (kvp.Value.HasBeenRead)
            {
                unlockedItems[kvp.Key] = true;
            }
        }

        // Also track pickupable items that have been picked up
        Dictionary<string, RegistryEntry> pickupableEntries = PickupRegistry.Instance.GetAllPickupables();
        foreach (var kvp in pickupableEntries)
        {
            if (kvp.Value.HasBeenPickedUp)
            {
                unlockedItems[kvp.Key] = true;
            }
        }

        Debug.Log($"NarrativeMenu: Tracking {trackedItemIDs.Count} narrative items, {unlockedItems.Count} already unlocked");
    }

    /// <summary>
    /// Refreshes all item slots on the first screen to reflect current unlock state.
    /// </summary>
    private void RefreshItemGrid()
    {
        if (itemSlots == null) return;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null) continue;

            if (i < trackedItemIDs.Count)
            {
                string id = trackedItemIDs[i];
                RegistryEntry entry = PickupRegistry.Instance?.GetEntry(id);
                bool isUnlocked = unlockedItems.ContainsKey(id) && unlockedItems[id];

                itemSlots[i].SetSlotData(
                    slotIndex: i,
                    isUnlocked: isUnlocked,
                    itemData: entry?.ItemData,
                    lockedSprite: lockedIcon,
                    menu: this
                );
                itemSlots[i].gameObject.SetActive(true);
            }
            else
            {
                // Empty slot
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Populates the detail view with the selected item's data.
    /// </summary>
    private void RefreshDetailView(string id)
    {
        RegistryEntry entry = PickupRegistry.Instance?.GetEntry(id);
        if (entry == null || entry.ItemData == null)
        {
            if (detailName != null) detailName.text = lockedDisplayName;
            if (detailDescription != null) detailDescription.text = lockedDescription;
            if (detailNarrativeText != null) detailNarrativeText.text = "";
            if (detailIcon != null && lockedIcon != null) detailIcon.sprite = lockedIcon;
            return;
        }

        ItemDataSO data = entry.ItemData;

        if (detailName != null) detailName.text = data.ItemName;
        if (detailDescription != null) detailDescription.text = data.Description;
        if (detailNarrativeText != null) detailNarrativeText.text = data.NarrativeText;
        if (detailIcon != null && data.Icon != null) detailIcon.sprite = data.Icon;
    }

    // ──────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns true if the item with the given ID has been unlocked in the narrative menu.
    /// </summary>
    public bool IsItemUnlocked(string id)
    {
        return unlockedItems.ContainsKey(id) && unlockedItems[id];
    }

    /// <summary>
    /// Returns the number of narrative items currently unlocked.
    /// </summary>
    public int GetUnlockedNarrativeCount()
    {
        int count = 0;
        foreach (string id in trackedItemIDs)
        {
            if (unlockedItems.ContainsKey(id) && unlockedItems[id])
                count++;
        }
        return count;
    }

    /// <summary>
    /// Returns the total number of narrative items being tracked.
    /// </summary>
    public int GetTotalNarrativeCount()
    {
        return trackedItemIDs.Count;
    }
}
