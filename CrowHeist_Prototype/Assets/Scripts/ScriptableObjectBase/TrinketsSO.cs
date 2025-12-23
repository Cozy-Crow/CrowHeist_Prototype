using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trinket", menuName = "TrinketSO")]
public class TrinketsSO : ScriptableObject
{
    [SerializeField] private string trinketName;
    [SerializeField, TextArea(3,5)] private string description;
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private Sprite unlockedIcon;
    private const string DEFAULT_TEXT = "???";
    public bool isUnlocked = false;
    public string TrinketName {get => trinketName; set => trinketName = value; }
    //For creating trinket
    public Sprite LockedIcon {get => lockedIcon; set => lockedIcon = value; }
    public Sprite UnlockedIcon {get => unlockedIcon; set => unlockedIcon = value; }
    public string Description {get => description; set => description = value; }
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
                return description;
            }
            else
            {
                return DEFAULT_TEXT;
            }
        }
    }
}
