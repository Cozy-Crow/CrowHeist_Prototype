using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trinket", menuName = "TrinketSO")]
public class TrinketsSO : ScriptableObject
{
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
    [SerializeField, TextArea(3,5)] private string description;

    public string Name => name;
    public Sprite Icon => icon;
    public string Descritpion => description;
}
