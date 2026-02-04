using System.Collections;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ColoringBook : Interactable
{
    //Created by ZackH 2/4
    //Script handling everything related to the coloring book puzzle!

    [SerializeField] GameObject menu;
    [SerializeField] Controller2Point5D crowley;
    [SerializeField] public Button closeButton;

    // Start is called before the first frame update
    void Start()
    {
        if(menu == null)
        {
            Debug.LogWarning("Coloring Book UI not connected!");
        }

        // add listener for button
        closeButton.onClick.AddListener(CloseMenu);

        // hide UI on start
        menu.gameObject.SetActive(false);
        Debug.Log("Starting");
    }

    // Update is called once per frame
    void Update()
    {
        if(menu.gameObject.activeSelf)
        {
            Debug.Log("handling coloring book puzzle");
            HandlePuzzle();
        }

    }

    //function that handles the puzzle logic
    void HandlePuzzle()
    {
        
    }

    void CloseMenu()
    {
        //close UI
        menu.gameObject.SetActive(false);
        //enable movement
        crowley.SetCanInput(true);
    }

    //function handling on interaction
    public override void TriggerInteraction(Pickable item)
    {
        Debug.Log("in coloringbook");
        //show UI
        menu.gameObject.SetActive(true);
        // //disable player movement
        crowley.SetCanInput(false);
    }
}
