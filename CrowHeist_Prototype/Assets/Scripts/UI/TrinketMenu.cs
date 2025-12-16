using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TrinketMenu : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private Image displayImage;
    [SerializeField] private TextMeshProUGUI displayName;
    [SerializeField] private TextMeshProUGUI displayDescription;
    [SerializeField] private Image[] trinketImageArray;
    private TrinketsSO[] trinketsDataArr;
    private int selectedID = 0;
    private static TrinketMenu instance;
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

        for(int i = 0; i <= trinketImageArray.Length - 1; i++)
        {
            trinketImageArray[i].sprite = null;
        }

        displayImage.sprite = null;
        displayName.text = "";
        displayDescription.text = "";

        selectedID = 0;
    }

    public void UpdateMenu()
    {
        for (int i = 0; i <= trinketsDataArr.Length - 1; i++)
        {
            trinketImageArray[i].sprite = trinketsDataArr[i].Icon;
        }
    }
}
