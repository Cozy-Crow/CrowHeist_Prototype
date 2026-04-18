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
        Debug.Log("coloringbook Starting Puzzle");

        //function called by the coloring book to start the puzzle
        puzzleOn = true;
        puzzleComplete = false;
        currentIndex = 0;

        lineRenderer.points.Clear();
        lineRenderer.SetAllDirty();

        //make sure all anchors are off at the start
        foreach(ColoringBookAnchor a in anchors)
            a.disableAnchor();
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
        
        //get correct mouse position
        ConvertMouseToCanvas();

        //on left click
        if(Input.GetMouseButtonDown(0))
            isDrawing = true;         

        if(Input.GetMouseButtonUp(0))
            isDrawing = false;

        if (lineRenderer.points.Count >= 1 && !puzzleComplete && isDrawing)
        {
            // Only move the cursor (last point)
            lineRenderer.points[lineRenderer.points.Count - 1] = localMousePosition;
            lineRenderer.SetAllDirty();
        }

    }

    public void HitAnchor(ColoringBookAnchor anchor)
    {
        //if the puzzle is complete, or the puzzle is turned off
        //return
        if (puzzleComplete || !puzzleOn)
            return;

        //get the index of the anchor passes
        int anchorListIndex = anchors.IndexOf(anchor);

        //if its not there, return
        if (anchorListIndex == -1)
            return; // not in list

        //get anchor pos
        currentAnchorLocal = panelRect.InverseTransformPoint(anchor.transform.position);

        //hitting first anchor in list
        if (currentIndex == 0)
        {
            if (anchorListIndex != 0)
                return;

            //add to line renderer
            lineRenderer.points.Add(currentAnchorLocal);
            lineRenderer.points.Add(localMousePosition); // cursor

            //trigger it
            anchor.triggerAnchor();
            //increment index
            currentIndex = 1;

            //refresh the graphic
            lineRenderer.SetAllDirty();
            return;
        }

        //next anchor
        if (anchorListIndex == currentIndex)
        {
            //remove cursor (temp, to add next anchor)
            lineRenderer.points.RemoveAt(lineRenderer.points.Count - 1);

            //add anchor
            lineRenderer.points.Add(currentAnchorLocal);

            //re add cursor
            lineRenderer.points.Add(localMousePosition);

            //trigger anchor
            anchor.triggerAnchor();
            currentIndex++; //increment

            //refresh
            lineRenderer.SetAllDirty();
            return;
        }

        //closing the shape, hitting the final anchor again
        if (currentIndex == anchors.Count && anchorListIndex == 0)
        {
            lineRenderer.points.RemoveAt(lineRenderer.points.Count - 1); // remove cursor
            lineRenderer.points.Add(lineRenderer.points[0]); // close loop

            lineRenderer.SetAllDirty();
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
