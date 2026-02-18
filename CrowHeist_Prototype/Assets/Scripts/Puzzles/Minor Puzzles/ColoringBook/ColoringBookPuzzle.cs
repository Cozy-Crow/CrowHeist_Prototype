using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ColoringBookPuzzle : MonoBehaviour
{
    //Written by Zack H. 2/8/25
    //Script in charge of the puzzle portion of the coloring book itself

    //list of the anchors
    [SerializeField] public List<ColoringBookAnchor> anchors;
    //the object that the menu is connected to
    [SerializeField] ColoringBookObject coloringBook;
    //line renderer 
    [SerializeField] private UILineRenderer lineRenderer;
    //panel RectTransform of the object its on
    [SerializeField] private RectTransform panelRect;
    //bool holding whether or not the puzzle is going
    bool puzzleOn = false;
    bool puzzleComplete = false;

    //index that we are on for the puzzle
    private int currentIndex = 0;
    
    //index that the cursor will be in the lineRenderer
    private int cursorIndex = 1;

    public bool isDrawing = false;
    [SerializeField] GameObject drawingCursor;

    ColoringBookAnchor currentAnchor;

    //mouseInput
    Vector2 localMousePosition; //mouse position on canvas
    Vector2 currentAnchorLocal;


    //line renderer vars
    int positionCount; //how many verticies

    // Start is called before the first frame update
    void Start()
    {
        //make sure puzzle is disabled
        puzzleOn = false;

        lineRenderer.points = new List<Vector2>();
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
        // Debug.Log("Starting Puzzle");
    }

    public void ClosePuzzle()
    {
        //function called by the button to close the puzzle (not finished)
        puzzleOn = false;
        // Debug.Log("Closing Puzzle");
    }

    public void EndPuzzle()
    {
        //function called by the button to end the puzzle (finish)
        puzzleComplete = true;
        
        //remove the last index (the mouse)
        lineRenderer.points.RemoveAt(lineRenderer.points.Count-1);
        lineRenderer.SetAllDirty(); //refesh the graphic

        //stop allowing drawing
        puzzleOn = false;

        //hide close button (to prevent menuing errors)
        coloringBook.closeButton.enabled = false;
        
        
        Debug.Log("Ending puzzle");

        //end puzzle
        coloringBook.EndPuzzle();
    }

    public void HandlePuzzle()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        //get correct mouse position
        ConvertMouseToCanvas();

        //on left click
        if(Input.GetMouseButtonDown(0))
        {
            isDrawing = true;         
        }

        if(Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        //updates the linerenderer every frame 
        if(currentAnchorLocal != null && lineRenderer.points.Count >= 2 && !puzzleComplete && isDrawing)
        {
            //set the last 2 most recent to be able to move (last hit)
            lineRenderer.points[lineRenderer.points.Count-2] = currentAnchorLocal;
            //cursor should always be at the end
            lineRenderer.points[lineRenderer.points.Count-1] = localMousePosition;
            lineRenderer.SetAllDirty(); //refesh the graphic
        }

    }

    public void HitAnchor(ColoringBookAnchor anchor)
    {
        currentAnchorLocal = panelRect.InverseTransformPoint(anchor.transform.position);        

        //check if its the current index
        if(anchor.index == currentIndex)
        {
            //add the new point to the graphic
            lineRenderer.points.Insert(currentIndex, currentAnchorLocal);

            Debug.Log("index " + currentIndex);

            //if its the first one added also add the cursor
            if(currentIndex == 0)
                lineRenderer.points.Insert(currentIndex+1, localMousePosition);
            // cursorIndex++; //index the cursor index so we know 

            //tell the anchor it was hit (change color and whatnot)
            anchor.triggered = true;

            //index
            currentIndex++;
            lineRenderer.SetAllDirty(); //refesh the graphic
        }

        //checking to make sure the player went all the way around
        if(currentIndex >= anchors.Count && anchor.CompareTag("ColoringBookFirstAnchor"))
        {
            lineRenderer.points.Insert(currentIndex, lineRenderer.points[0]);
            lineRenderer.SetAllDirty(); //refesh the graphic
            EndPuzzle();
        }
    }


    private void ConvertMouseToCanvas()
    {
        // Convert screen position to local position in the panel
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect, 
            Input.mousePosition, 
            null, // use Camera if canvas is in World Space
            out localMousePosition))
        {
            
        }
    }
}
