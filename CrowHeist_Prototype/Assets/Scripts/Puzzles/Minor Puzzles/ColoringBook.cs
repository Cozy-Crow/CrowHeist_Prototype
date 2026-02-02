using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.UIElements;

public class ColoringBook : MonoBehaviour
{
    [SerializeField] Canvas menu;
    [SerializeField] Controller2Point5D crowley;

    // Start is called before the first frame update
    void Start()
    {
        if(menu == null)
        {
            Debug.LogWarning("Coloring Book UI not connected!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnInteract()
    {
        //show UI, Disable player movement
        menu.enabled = true;
        crowley.ToggleInput();
    }
}
