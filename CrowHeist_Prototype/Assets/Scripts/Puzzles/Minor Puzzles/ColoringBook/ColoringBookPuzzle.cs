using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class ColoringBookPuzzle : MonoBehaviour
{
    //Written by Zack H. 2/8/25
    //Script in charge of the puzzle portion of the coloring book itself

    //list of the anchors
    [SerializeField] public List<ColoringBookAnchor> anchors;
    //the object that the menu is connected to
    [SerializeField] GameObject coloringBook;
    //line renderer 
    [SerializeField] private LineRenderer lineRender;
    [SerializeField] private RectTransform panelRect;
    //bool holding whether or not the puzzle is going
    bool puzzleOn = false;

    private int currentIndex = 0;
    public bool isDrawing = false;
    [SerializeField] GameObject drawingCursor;
    public Camera uiCamera; // if using Screen Space - Camera, otherwise null


    //raycasting
    [SerializeField] float RaycastDistance = 10f;
    RaycastHit hit;

    //mouseInput
    Vector3 screenPos;
    Vector3 worldPos;

    //line renderer vars
    int positionCount; //how many verticies

    // Start is called before the first frame update
    void Start()
    {
        //inital positions is 0 (not showing)
        // lineRender.positionCount = 0;

        //make sure puzzle is disabled
        puzzleOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(puzzleOn)
        {
            HandlePuzzle();
        }
    }

    public void StartPuzzle()
    {
        //function called by the coloring book to start the puzzle
        puzzleOn = true;
        Debug.Log("Starting Puzzle");
    }

    public void ClosePuzzle()
    {
        //function called by the button to close the puzzle (not finished)
        puzzleOn = false;
        Debug.Log("Closing Puzzle");
    }

    public void EndPuzzle()
    {
        //function called by the button to end the puzzle (finish)
        puzzleOn = false;
        Debug.Log("Ending Puzzle");
    }

    public void HandlePuzzle()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        //get correct mouse position
        ConvertMouseToScreenPos();

        //on left click
        if(Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            // lineRender.positionCount = 0;
            // //raycast from cursor
            // if(Physics.Raycast(cursorRay, out RaycastHit hit))
            // {
            //     //check if you hit an anchor
            //     if(hit.collider.CompareTag("ColoringBookAnchor") )
            //     {
            //         //set inital connection
            //         lineRender.positionCount = 2;
            //         lineRender.SetPosition(0, hit.transform.position);
            //         lineRender.SetPosition(1, worldPos);
            //     }
            // }
            Debug.Log("Starting Draw- HandlePuzzle");
        }

        if(Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
            Debug.Log("Ending Drawn- HandlePuzzle");
        }

        // if(isDrawing)
        // {
        //     Vector2 mousePos = Input.mousePosition;
        //     lineRender.positionCount++;
        //     lineRender.SetPosition(lineRender.positionCount - 1, worldPos);
        //     Debug.Log("Drawing- HandlePuzzle");
        // }

    }

    public bool IsDragging() => isDrawing;

    public void HitAnchor(ColoringBookAnchor anchor)
    {
        //function is called when an anchor is hit

        //check if its the current index
        if(anchor.index == currentIndex)
        {
            //tell the anchor it was hit (change color and whatnot)
            anchor.triggered = true;
            currentIndex++;

            Debug.Log("here");

            if(currentIndex >= anchors.Count)
                EndPuzzle();
        }
        else
        {
            //reset?
            Debug.Log("IN ELSE");
        }
    }

    private void ConvertMouseToScreenPos()
    {
        //get mouse pos
        screenPos = Input.mousePosition;
        //convert to world position
        worldPos = Camera.main.ScreenToWorldPoint(screenPos);
    }
}
