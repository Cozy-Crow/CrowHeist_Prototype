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

    [Tooltip("How long the tooltip should stay active")]
    public float tooltipDuration = 2f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void CheckItemTag(string tagName)
    {
        foreach (var item in tooltipItems)
        {
            if (item.tagName == tagName)
            {
                ShowTooltip(item.tooltipUI);
                return;
            }
        }
    }

    private void ShowTooltip(GameObject tooltipUI)
    {
        if (tooltipUI == null) return;

        tooltipUI.SetActive(true);
        CancelInvoke(nameof(HideAllTooltips));
        Invoke(nameof(HideAllTooltips), tooltipDuration);
    }

    private void HideAllTooltips()
    {
        foreach (var item in tooltipItems)
        {
            if (item.tooltipUI != null)
                item.tooltipUI.SetActive(false);
        }
    }
}