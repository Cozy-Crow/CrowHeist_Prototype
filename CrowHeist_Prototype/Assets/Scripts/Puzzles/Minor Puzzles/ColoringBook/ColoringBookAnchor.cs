using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColoringBookAnchor : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] public int index;

    [SerializeField] private ColoringBookPuzzle puzzleManager;


    void Update()
    {
        
    }

    //function handling if the mouse enters the image
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(puzzleManager.isDrawing)
        {
            puzzleManager.HitAnchor(this);
            // Debug.Log("HIT! - " + name);
        }

    }

    public void triggerAnchor()
    {
        GetComponent<Image>().color = Color.green;
        //play anchor hit sound here
    }

    public void disableAnchor()
    {
        GetComponent<Image>().color = Color.white;
        //play anchor hit sound here
    }
}
