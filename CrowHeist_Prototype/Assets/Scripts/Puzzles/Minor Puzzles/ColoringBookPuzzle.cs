using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class ColoringBookPuzzle : MonoBehaviour
{
    //Written by Zack H. 2/8/25
    //Script in charge of the puzzle portion of the coloring book itself

    //list of the anchors
    [SerializeField] public List<Image> anchors;
    //the object that the menu is connected to
    [SerializeField] GameObject coloringBook;
    //line renderer 
    [SerializeField] private LineRenderer lineRender;

    bool puzzleOn = false;

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
        lineRender.positionCount = 0;

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
    }

    public void EndPuzzle()
    {
        //function called by the button to end the puzzle (on close)
        puzzleOn = false;
    }

    public void HandlePuzzle()
    {
        Ray cursorRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        //get correct mouse position
        ConvertMouseToScreenPos();

        //on left click
        if(Input.GetMouseButtonDown(0))
        {
            //raycast from cursor
            if(Physics.Raycast(cursorRay, out RaycastHit hit))
            {
                //check if you hit an anchor
                if(hit.collider.CompareTag("ColoringBookAnchor") )
                {
                    //set inital connection
                    lineRender.positionCount = 2;
                    lineRender.SetPosition(0, hit.transform.position);
                    lineRender.SetPosition(1, worldPos);
                }
            }
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
