using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hanoiDisc : MonoBehaviour
{
    public GameObject disc; 
    void Awake(){
        pullDisc = this.disc;
    }
}
