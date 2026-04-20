using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TrinketMenu : MonoBehaviour
{
    public static TrinketMenu instance;
    [Header("UI References")]
    [SerializeField] private CanvasGroup container;
    [SerializeField] private Transform displayImageContainer;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI displayDescription;
    [SerializeField] private TextMeshProUGUI locationHint;
    [SerializeField] private TextMeshProUGUI pageNumber;
    [SerializeField] private TrinketImage[] trinketImageArray;

    [Header("UI Events")]
    public UnityEvent open;
    public UnityEvent close;

    [Header("Data References")]
    [SerializeField] private List<ItemDataSO> trinketsDataLst = new();
    [SerializeField] private Sprite defaultSprite;

    [Header("SFX")]
    public EventReference openMenuSFX;
    public EventReference closeMenuSFX;
    public EventReference selectTrinketSFX;
    public EventReference hoverTrinketSFX;
    public EventReference lockTrinketSFX;
    public EventReference pageUpSFX;
    public EventReference pageDownSFX;
    private bool isOpen = false;
    private int lastToggleFrame = -1;
    private int selectedID = 0;
    private int pageIndex = 0;
    private int maxPageIndex = 0;
    public int SelectedID { get => selectedID; }
    public int PageIndex { get => pageIndex; }
    public int MaxPageIndex { get => maxPageIndex; }
    public Sprite DefaultSprite { get => defaultSprite; }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SetUp();
    }

    private void Start()
    {
        if (PickupRegistry.Instance != null)
        {
            PickupRegistry.Instance.OnItemPickedUp += OnItemPickedUp;
            PickupRegistry.Instance.OnNarrativeItemCollected += OnNarrativeItemCollected;
        }
    }

    private void OnDestroy()
    {
        if (PickupRegistry.Instance != null)
        {
            PickupRegistry.Instance.OnItemPickedUp -= OnItemPickedUp;
            PickupRegistry.Instance.OnNarrativeItemCollected -= OnNarrativeItemCollected;
        }
    }

    /// <summary>
    /// Called when any item is picked up for the first time.
    /// For narrative items: marks the matching ItemDataSO as isPickedUp (shows descriptions[0]).
    /// </summary>
    private void OnItemPickedUp(RegistryEntry entry)
    {
        if (entry == null || entry.ObjectType != ObjectType.Narrative) return;

        if (entry.ItemData != null)
        {
            PickupItem(entry.ItemData);
        }
    }

    /// <summary>
    /// Called when a narrative item is collected (deposited at HeistZone).
    /// Marks the matching ItemDataSO as fully unlocked (descriptions[1..n] readable).
    /// </summary>
    private void OnNarrativeItemCollected(RegistryEntry entry)
    {
        if (entry == null) return;

        if (entry.ItemData != null)
        {
            UpdateItem(entry.ItemData);
        }
    }


    private void SetUp()
    {
        container.alpha = 0f;
        container.gameObject.SetActive(false);
        isOpen = false;

        if (trinketsDataLst.Count == 0)
        {
            Debug.LogWarning("Trinket Data List is Empty");
            return;
        }

        for (int i = 0; i < trinketImageArray.Length; i++)
        {
            trinketImageArray[i].IconSprite = null;
        }

        displayImage.sprite = defaultSprite;
        displayName.text = "";
        displayDescription.text = "";

        selectedID = 0;
        maxPageIndex = Mathf.CeilToInt(trinketsDataLst.Count / 8f) - 1;
    }

    private void UpdateMenu()
    {
        for (int i = 0; i < trinketImageArray.Length; i++)
        {
            if (i + pageIndex * 8 >= trinketsDataLst.Count)
            {
                trinketImageArray[i].IconSprite = defaultSprite;
                continue;
            }
            ItemDataSO data = trinketsDataLst[i + pageIndex * 8];
            trinketImageArray[i].IconSprite = data.DisplayIcon;
            trinketImageArray[i].IsUnlocked = data.isPickedUp || data.HasBeenRead;
        }

        Debug.Log($"Selected Display: {trinketsDataLst[selectedID].DisplayIcon}");
        displayImage.sprite = trinketsDataLst[selectedID].DisplayIcon;
        displayName.text = trinketsDataLst[selectedID].DisplayName;
        displayDescription.text = trinketsDataLst[selectedID].DisplayDescription;

        pageNumber.text = $"Page {pageIndex + 1} / {maxPageIndex + 1}";
    }

    /// <summary>
    /// Immediately closes the trinket menu without animation.
    /// Called by PauseManager when the pause menu opens.
    /// </summary>
    public void ForceClose()
    {
        if (!isOpen) return;
        container.DOKill();
        isOpen = false;
        Time.timeScale = 1f;
        container.alpha = 0f;
        container.gameObject.SetActive(false);
        close.Invoke();
    }

    public void ToggleMenu()
    {
        // Prevent multiple toggles on the same frame (e.g. duplicate controllers)
        if (Time.frameCount == lastToggleFrame) return;
        lastToggleFrame = Time.frameCount;

        // Kill any in-flight DOTween animations to prevent stale onComplete callbacks
        container.DOKill();

        if (!isOpen)
        {
            // Don't open if the pause menu is already visible
            PauseManager pauseManager = FindObjectOfType<PauseManager>();
            if (pauseManager != null &&
                (pauseManager.isGamePaused ||
                 (pauseManager.PauseMenu != null && pauseManager.PauseMenu.activeSelf)))
                return;

            //Open Menu
            isOpen = true;
            Time.timeScale = 0f;
            container.alpha = 0f;
            container.gameObject.SetActive(true);
            container.DOFade(1, 0.2f).SetUpdate(true);

            Debug.Log("Opening Trinket Menu");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(openMenuSFX);
            }

            UpdateMenu();

            open.Invoke();
        }
        else
        {
            //Close Menu
            isOpen = false;
            Time.timeScale = 1f;
            container.DOFade(0, 0.2f).SetUpdate(true).onComplete = () =>
            {
                container.gameObject.SetActive(false);
            };

            Debug.Log("Closing Trinket Menu");

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayOneShot(closeMenuSFX);
            }

            close.Invoke();
        }
    }

    /// <summary>
    /// Stage 1: Marks item as picked up (first time Crowley grabs it).
    /// Shows descriptions[0] only.
    /// </summary>
    public void PickupItem(ItemDataSO item)
    {
        if (item != null)
        {
            if (!item.isPickedUp && !item.HasBeenRead)
            {
                item.isPickedUp = true;
                Debug.Log($"Trinket Picked Up (stage 1): {item.DisplayName}");
            }
        }
        else
        {
            Debug.Log("Trinket to pick up is null");
        }
    }

    /// <summary>
    /// Stage 2: Marks item as collected (deposited at HeistZone).
    /// descriptions[0] hidden, descriptions[1..n] now readable.
    /// </summary>
    public void UpdateItem(ItemDataSO item)
    {
        if (item != null)
        {
            item.MarkAsRead();
            Debug.Log($"Trinket Collected (stage 2): {item.DisplayName}");
        }
        else
        {
            Debug.Log("Trinket to unlock is null");
        }
    }

    public void SelectItem(int index)
    {
        int arrayIndex = index + pageIndex * 8;
        Debug.Log($"Selected Item Index: {arrayIndex}");

        if (selectedID == index && trinketsDataLst[arrayIndex].HasBeenRead)
        {
            trinketsDataLst[arrayIndex].CycleDescription();
        }

        selectedID = index;
        displayImage.sprite = trinketsDataLst[arrayIndex].DisplayIcon;
        displayName.text = trinketsDataLst[arrayIndex].DisplayName;
        displayDescription.text = trinketsDataLst[arrayIndex].DisplayDescription;
        locationHint.text = trinketsDataLst[arrayIndex].LocationHint;
    }

    public void PageUp()
    {
        if (pageIndex >= maxPageIndex) return;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(pageUpSFX);
        }

        pageIndex++;
        UpdateMenu();
    }

    public void PageDown()
    {
        if (pageIndex <= 0) return;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(pageDownSFX);
        }
        pageIndex--;
        UpdateMenu();
    }
}