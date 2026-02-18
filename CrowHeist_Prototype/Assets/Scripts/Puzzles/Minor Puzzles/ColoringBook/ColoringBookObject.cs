using System;
using System.Collections;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.UI;

public class ColoringBookObject : Interactable
{
    //Created by ZackH 2/4
    //Script handling everything related to the coloring book object!

    [SerializeField] GameObject menu; //menu linked to the puzzle
    [SerializeField] Controller2Point5D crowley; //reference to crowley
    [SerializeField] public Button closeButton; //close button on the menu
    [SerializeField] public GameObject puzzleController; //reference to the script on the ui (controls the actual puzzle)
    bool isInteractable = true; //tells whether or not the book can be interacted with
    
    //animation vars
    float animTime = 5; //holds animation time for the ending animation for the puzzle
    [SerializeField] SpriteRenderer spriteRenderer; //sprites
    [SerializeField] Sprite sprite1; //3 sprites showing after each slam
    [SerializeField] Sprite sprite2;
    [SerializeField] Sprite sprite3;
    [SerializeField] GameObject openBook; //open and closed book models
    [SerializeField] GameObject closedBook;
    [SerializeField] SpriteRenderer sprite; // holds the sprite during the animation



    // Start is called before the first frame update
    void Start()
    {
        sprite.enabled = false; //turn of sprite initally

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
        //start end puzzle sequence
        StartCoroutine(EndPuzzleRoutine());
    }

    //handles 
    IEnumerator EndPuzzleRoutine()
    {
        //stall closing the menu - pause for 2 seconds
        yield return new WaitForSeconds(2);

        //close UI
        menu.gameObject.SetActive(false);

        //run through animation (semi scuffed bc of using enabling/disabling physical objects atm)
        yield return new WaitForSeconds(0.25f); //.25 sec before it starts
        //close book
        closedBook.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f); //.25 before next

        //open book
        closedBook.gameObject.SetActive(false);
        openBook.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f); //.1 before showing
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite1;

        //close book
        closedBook.gameObject.SetActive(true);
        openBook.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f); //.25 before next
        spriteRenderer.enabled = false;


        //time this with animation time
        //pauses the coroutine until the animation is done
        yield return new WaitForSeconds(animTime);

        ////closing sequence - Enable input, disable ability to interact
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
