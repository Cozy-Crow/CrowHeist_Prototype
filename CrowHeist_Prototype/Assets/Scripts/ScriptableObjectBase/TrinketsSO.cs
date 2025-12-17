using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trinket", menuName = "TrinketSO")]
public class TrinketsSO : ScriptableObject
{
    [SerializeField] private string trinketName;
    [SerializeField] private Sprite lockedIcon;
    [SerializeField] private Sprite unlockedIcon;
    [SerializeField, TextArea(3,5)] private string description;
    private const string DEFAULT_TEXT = "???";
    public bool isUnlocked = false;
    public string Name => trinketName;
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
