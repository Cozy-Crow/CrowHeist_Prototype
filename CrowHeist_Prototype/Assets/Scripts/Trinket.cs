using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trinket : MonoBehaviour,INarrative
{
    [SerializeField] private string trinketName;
    public string Name {get => trinketName;}
}
