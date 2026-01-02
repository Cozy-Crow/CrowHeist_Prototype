using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrinketMenu : MonoBehaviour
{
    public static TrinketMenu instance;
    [SerializeField] private Animator animator;

    [Header("UI References")]
    [SerializeField] private Transform container;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI displayDescription;
    [SerializeField] private TextMeshProUGUI pageNumber;
    [SerializeField] private Image[] trinketImageArray;

    [Header("Data References")]
    [SerializeField] private List<TrinketsSO> trinketsDataLst = new();
    [SerializeField] private Sprite defaultSprite;

    [Header("SFX")]
    public EventReference openMenu;
    public EventReference closeMenu;
    public EventReference selectTrinket;
    public EventReference hoverTrinket;
    public EventReference lockTrinket;
    public EventReference pageUp;
    public EventReference pageDown;
    private int selectedID = 0;
    private int pageIndex = 0;
    private int maxPageIndex = 0;
    public int SelectedID { get => selectedID; }
    public int PageIndex { get => pageIndex; }
    public int MaxPageIndex { get => maxPageIndex; }
    public Sprite DefaultSprite { get => defaultSprite; }
    public Animator Animator { get => animator; }
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

        animator = GetComponent<Animator>();
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
            trinketImageArray[i].sprite = null;
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
                trinketImageArray[i].sprite = defaultSprite;
                continue;
            }
            trinketImageArray[i].sprite = trinketsDataLst[i + pageIndex * 8].DisplayIcon;
            trinketImageArray[i].GetComponent<TrinketImage>().IsUnlocked = trinketsDataLst[i + pageIndex * 8].isUnlocked;
        }

        Debug.Log($"Selected Display: {trinketsDataLst[selectedID].DisplayIcon}");
        displayImage.sprite = trinketsDataLst[selectedID].DisplayIcon;
        displayName.text = trinketsDataLst[selectedID].DispayName;
        displayDescription.text = trinketsDataLst[selectedID].DisplayDescritpion;

        pageNumber.text = $"Page {pageIndex + 1} / {maxPageIndex + 1}";
    }

    public void ToggleMenu()
    {
        if (container.gameObject.activeSelf)
        {
            AudioManager.Instance.PlayOneShot(closeMenu);
            container.gameObject.SetActive(false);
        }
        else
        {  
            AudioManager.Instance.PlayOneShot(openMenu);
            container.gameObject.SetActive(true);
            UpdateMenu();
            animator.Play("Open");
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
    }

    public void PageUp()
    {
        if (pageIndex >= maxPageIndex) return;
        AudioManager.Instance.PlayOneShot(pageUp);
        pageIndex++;
        UpdateMenu();
    }

    public void PageDown()
    {
        if (pageIndex <= 0) return;
        AudioManager.Instance.PlayOneShot(pageDown);
        pageIndex--;
        UpdateMenu();
    }
}
