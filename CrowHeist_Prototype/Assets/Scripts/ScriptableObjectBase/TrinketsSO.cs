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
    //For displaying trinket
    public string DispayName {
        get
        {
            if (isUnlocked)
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
            if (isUnlocked)
            {
                return unlockedIcon;
            }
            else
            {
                return lockedIcon;
            }
        } 
    }
    public string DisplayDescritpion
    {
        get
        {
            if (isUnlocked)
            {
                return Description;
            }
            else
            {
                return DEFAULT_TEXT;
            }
        }
    }
}
