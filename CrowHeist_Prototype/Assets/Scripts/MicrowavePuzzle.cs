using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MicrowavePuzzle : MonoBehaviour
{
    bool containsKnife = false;
    
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(containsKnife)
            GoBoom();
    }

    void GoBoom()
    {
        
    }

    
}
