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
    [SerializeField] private List<TrinketsSO> trinketsDataLst = new();
    [SerializeField] private Sprite defaultSprite;

    [Header("SFX")]
    public EventReference openMenuSFX;
    public EventReference closeMenuSFX;
    public EventReference selectTrinketSFX;
    public EventReference hoverTrinketSFX;
    public EventReference lockTrinketSFX;
    public EventReference pageUpSFX;
    public EventReference pageDownSFX;
    private int selectedID = 0;
    private int pageIndex = 0;
    private int maxPageIndex = 0;
    public int SelectedID { get => selectedID; }
    public int PageIndex { get => pageIndex; }
    public int MaxPageIndex { get => maxPageIndex; }
    public Sprite DefaultSprite { get => defaultSprite; }
    private void Awake()
    {
        if (instance != null )
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SetUp();
    }


    private void SetUp()
    {
        container.gameObject.SetActive(false);

        if(trinketsDataLst.Count == 0)
        {
            Debug.LogWarning("Trinket Data List is Empty");
            return;
        }

        for(int i = 0; i < trinketImageArray.Length; i++)
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
            trinketImageArray[i].IconSprite = trinketsDataLst[i + pageIndex * 8].DisplayIcon;
            trinketImageArray[i].IsUnlocked = trinketsDataLst[i + pageIndex * 8].isUnlocked;
        }

        Debug.Log($"Selected Display: {trinketsDataLst[selectedID].DisplayIcon}");
        displayImage.sprite = trinketsDataLst[selectedID].DisplayIcon;
        displayName.text = trinketsDataLst[selectedID].DispayName;
        displayDescription.text = trinketsDataLst[selectedID].DisplayDescritpion;

        pageNumber.text = $"Page {pageIndex + 1} / {maxPageIndex + 1}";
    }

    public void ToggleMenu()
    {
        if (container.gameObject.activeSelf == false)
        {
            //Open Menu
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

    public void UpdateItem(TrinketsSO trinket)
    {
        if(trinket != null)
        {
            trinket.isUnlocked = true;
            Debug.Log($"Trinket Unlocked: {trinket.DispayName}");
        }
        else
        {
            Debug.Log("Trinket to unlock is null");
        }
    }

    public void SelectItem(int index)
    {
        selectedID = index;
        int arrayIndex = selectedID + pageIndex * 8;
        Debug.Log($"Selected Item Index: {arrayIndex}");

        displayImage.sprite = trinketsDataLst[arrayIndex].DisplayIcon;
        displayName.text = trinketsDataLst[arrayIndex].DispayName;
        displayDescription.text = trinketsDataLst[arrayIndex].DisplayDescritpion;
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
