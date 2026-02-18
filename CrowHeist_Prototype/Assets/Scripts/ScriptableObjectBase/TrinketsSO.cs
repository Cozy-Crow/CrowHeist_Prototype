using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trinket", menuName = "TrinketSO")]
public class TrinketsSO : ScriptableObject
{
    [SerializeField] private string trinketName;
    [SerializeField, TextArea(3,5)] private string[] descriptions;
    [SerializeField] string locationHint;
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private Sprite unlockedIcon;
    private const string DEFAULT_TEXT = "???";
    public bool isPickedUp = false;
    public bool isUnlocked = false;
    public string TrinketName {get => trinketName; set => trinketName = value; }
    //For creating trinket
    public Sprite LockedIcon {get => lockedIcon; set => lockedIcon = value; }
    public Sprite UnlockedIcon {get => unlockedIcon; set => unlockedIcon = value; }

    /// <summary>
    /// Gets or sets the full array of descriptions.
    /// </summary>
    public string[] Descriptions {get => descriptions; set => descriptions = value; }

    /// <summary>
    /// Gets or sets a single description. Getter returns a random pick; setter wraps into array.
    /// </summary>
    public string Description
    {
        get
        {
            if (descriptions == null || descriptions.Length == 0)
                return string.Empty;
            if (descriptions.Length == 1)
                return descriptions[0];
            return descriptions[Random.Range(0, descriptions.Length)];
        }
        set => descriptions = new string[] { value };
    }

    public string LocationHint {get => locationHint; set => locationHint = value; }

    /// <summary>
    /// Returns descriptions[0] — the only description shown when first picked up (before collection).
    /// </summary>
    public string FirstDescription
    {
        get
        {
            if (descriptions == null || descriptions.Length == 0)
                return string.Empty;
            return descriptions[0];
        }
    }

    /// <summary>
    /// Returns descriptions[1..n] — the descriptions unlocked after collection.
    /// descriptions[0] is no longer shown once collected.
    /// </summary>
    public string[] CollectedDescriptions
    {
        get
        {
            if (descriptions == null || descriptions.Length <= 1)
                return new string[0];
            string[] collected = new string[descriptions.Length - 1];
            System.Array.Copy(descriptions, 1, collected, 0, collected.Length);
            return collected;
        }
    }

    //For displaying trinket
    public string DispayName {
        get
        {
            if (isPickedUp || isUnlocked)
            {
                return trinketName;
            }
            else
            {
                return DEFAULT_TEXT;
            }
        }
    }
    public Sprite DisplayIcon {
        get
        {
            if (isPickedUp || isUnlocked)
            {
                return unlockedIcon;
            }
            else
            {
                return lockedIcon;
            }
        }
    }
    /// <summary>
    /// Staged description display:
    /// Locked    → "???"
    /// PickedUp  → descriptions[0] only
    /// Collected → descriptions[1..n] joined
    /// </summary>
    public string DisplayDescritpion
    {
        get
        {
            if (isUnlocked)
            {
                string[] collected = CollectedDescriptions;
                return collected.Length > 0
                    ? string.Join("\n\n", collected)
                    : "";
            }
            else if (isPickedUp)
            {
                return FirstDescription;
            }
            else
            {
                return DEFAULT_TEXT;
            }
        }
    }
}
