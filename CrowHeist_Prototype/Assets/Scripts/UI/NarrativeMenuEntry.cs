using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// UI component for a single item slot in the narrative menu grid.
/// Shows a locked icon when the item hasn't been collected, and the item's
/// actual icon + name once unlocked. Clicking an unlocked slot opens the
/// detail view in NarrativeMenu.
/// </summary>
public class NarrativeMenuEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameLabel;
    [SerializeField] private Image selectionBorder;

    [Header("Visual Settings")]
    [SerializeField] private Color lockedTint = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Color unlockedTint = Color.white;

    private int slotIndex;
    private bool isUnlocked;
    private NarrativeMenu menu;

    /// <summary>
    /// Populates this slot with data from the NarrativeMenu.
    /// Called by NarrativeMenu.RefreshItemGrid().
    /// </summary>
    public void SetSlotData(int slotIndex, bool isUnlocked, ItemDataSO itemData, Sprite lockedSprite, NarrativeMenu menu)
    {
        this.slotIndex = slotIndex;
        this.isUnlocked = isUnlocked;
        this.menu = menu;

        if (isUnlocked && itemData != null)
        {
            // Show the unlocked item
            if (iconImage != null)
            {
                iconImage.sprite = itemData.Icon != null ? itemData.Icon : lockedSprite;
                iconImage.color = unlockedTint;
            }
            if (nameLabel != null)
            {
                nameLabel.text = itemData.ItemName;
            }
        }
        else
        {
            // Show locked state
            if (iconImage != null)
            {
                iconImage.sprite = lockedSprite;
                iconImage.color = lockedTint;
            }
            if (nameLabel != null)
            {
                nameLabel.text = "???";
            }
        }

        // Reset selection border
        if (selectionBorder != null)
        {
            Color c = selectionBorder.color;
            c.a = 0f;
            selectionBorder.color = c;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (menu != null)
        {
            menu.SelectItem(slotIndex);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectionBorder != null)
        {
            Color c = selectionBorder.color;
            c.a = 1f;
            selectionBorder.color = c;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (selectionBorder != null)
        {
            Color c = selectionBorder.color;
            c.a = 0f;
            selectionBorder.color = c;
        }
    }
}
