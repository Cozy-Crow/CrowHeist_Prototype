using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.UIElements;

public class ColoringBook : Interactable
{
    [SerializeField] GameObject menu;
    [SerializeField] Controller2Point5D crowley;
    [SerializeField] PauseManager pauseManager;

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
        if(menu.gameObject.activeSelf)
        {
            Debug.Log("handling coloring book puzzle");
            HandlePuzzle();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("closing book puzzle");
            CloseMenu();
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
        //unpause game
        pauseManager.SetIsGamePaused(false);
        Time.timeScale = 1;
    }

    //function handling on interaction
    public override void TriggerInteraction(Pickable item)
    {
        Debug.Log("in coloringbook");
        //show UI
        menu.gameObject.SetActive(true);
        // //disable player movement
        // crowley.ToggleInput();
        //pause game
        pauseManager.SetIsGamePaused(true);
        Time.timeScale = 0;
    }
}
