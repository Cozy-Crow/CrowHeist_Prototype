using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrinketMenu : MonoBehaviour
{
    public static TrinketMenu instance;

    [Header("UI References")]
    [SerializeField] private Transform container;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI displayDescription;
    [SerializeField] private Image[] trinketImageArray;

    [Header("Data References")]
    [SerializeField]private List<TrinketsSO> trinketsDataLst = new();
    private Dictionary<string, TrinketsSO> trinketsDataDict = new();
    private int selectedID = 0;
    private int minIndex = 0;
    private int maxIndex = 8;

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

        for(int i = 0; i < trinketImageArray.Length; i++)
        {
            trinketImageArray[i].sprite = null;
        }

        for (int i = 0; i < trinketsDataLst.Count; i++)
        {
            trinketsDataDict.Add(trinketsDataLst[i].Name, trinketsDataLst[i]);
        }

        Debug.Log($"Trinket Dict Count: {trinketsDataDict.Count}");
        foreach (var item in trinketsDataDict)
        {
            Debug.Log($"Trinket Dict Item: {item.Key}");
        }

        displayImage.sprite = null;
        displayName.text = "";
        displayDescription.text = "";

        selectedID = 0;
    }

    private void UpdateMenu()
    {
        for (int i = 0; i <= trinketsDataLst.Count - 1; i++)
        {
            trinketImageArray[i].sprite = trinketsDataLst[i].DisplayIcon;
        }

        displayImage.sprite = trinketsDataLst[selectedID].DisplayIcon;
        displayName.text = trinketsDataLst[selectedID].DispayName;
        displayDescription.text = trinketsDataLst[selectedID].DisplayDescritpion;
    }

    public void ToggleMenu()
    {
        if (container.gameObject.activeSelf)
        {
            container.gameObject.SetActive(false);
        }
        else
        {  
            container.gameObject.SetActive(true);
            UpdateMenu();
        }
    }

    public void UpdateItem(string name)
    {
        if(trinketsDataDict.TryGetValue(name, out TrinketsSO trinketData))
        {
            trinketData.isUnlocked = true;
            Debug.Log($"Trinket {name} {trinketData.isUnlocked}!");
        }else
        {
            Debug.Log($"Trinket {name} not found in dict!");
        }
    }


    /// <summary>
    /// Scroll the narrative menu x amount of page
    /// - -> move left
    /// + -> move right
    /// </summary>
    /// <param name="page">Number of pages to scroll</param>
    public void ScrollPage(int page)
    {
        minIndex += page*8;
        maxIndex += page*8;

        if (minIndex < 0)
        {
            minIndex = 0;
        }
        if (maxIndex > trinketsDataLst.Count - 1)
        {
            minIndex = trinketsDataLst.Count - 8;
            maxIndex = trinketsDataLst.Count - 1;
        }
    }
}
