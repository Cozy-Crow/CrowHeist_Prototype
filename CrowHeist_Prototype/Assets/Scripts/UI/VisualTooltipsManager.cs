using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    [System.Serializable]
    public class TooltipItem
    {
        public string tagName;
        public GameObject tooltipUI;
    }

    [Header("Tracked Tooltips")]
    public List<TooltipItem> tooltipItems = new List<TooltipItem>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Called when an item is picked up
    public void ShowTooltip(string tagName)
    {
        foreach (var item in tooltipItems)
        {
            if (item.tagName == tagName && item.tooltipUI != null)
            {
                item.tooltipUI.SetActive(true);
                return;
            }
        }
    }

    // Called when an item is dropped
    public void HideTooltip(string tagName)
    {
        foreach (var item in tooltipItems)
        {
            if (item.tagName == tagName && item.tooltipUI != null)
            {
                item.tooltipUI.SetActive(false);
                return;
            }
        }
    }
}