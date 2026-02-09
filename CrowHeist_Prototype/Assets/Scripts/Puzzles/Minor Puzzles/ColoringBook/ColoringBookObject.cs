using System.Collections;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ColoringBookObject : Interactable
{
    //Created by ZackH 2/4
    //Script handling everything related to the coloring book puzzle!

    [SerializeField] GameObject menu;
    [SerializeField] Controller2Point5D crowley;
    [SerializeField] public Button closeButton;
    [SerializeField] public GameObject puzzleController;

    bool isInteractable = true;


    // Start is called before the first frame update
    void Start()
    {
        if(menu == null)
        {
            Debug.LogWarning("Coloring Book UI not connected!");
        }

        if(puzzleController == null)
        {
            Debug.LogWarning("Coloring Book Puzzle not connected!");
        } 

        // add listener for button
        // close menu on click
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
            // Debug.Log("handling coloring book puzzle");
            
            //start the puzzle
            puzzleController.GetComponent<ColoringBookPuzzle>().StartPuzzle();
        }

    }

    void CloseMenu()
    {
        //close UI
        menu.gameObject.SetActive(false);
        //enable movement
        crowley.SetCanInput(true);
        //stop the puzzle
        puzzleController.GetComponent<ColoringBookPuzzle>().ClosePuzzle();
    }

    public void EndPuzzle()
    {
        //close UI
        menu.gameObject.SetActive(false);
        //enable movement
        crowley.SetCanInput(true);

        //play animation, pop reward
        
        //remove interactioncapability
        isInteractable = false;
    }

    //function handling on interaction
    public override void TriggerInteraction(Pickable item)
    {
        if(isInteractable)
        {
            //show UI
            menu.gameObject.SetActive(true);
            // //disable player movement
            crowley.SetCanInput(false);
            
        }
    }
}
