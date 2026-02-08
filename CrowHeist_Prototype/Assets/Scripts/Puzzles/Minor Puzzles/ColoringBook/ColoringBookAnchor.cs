using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ColoringBookAnchor : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] public int index;
    [SerializeField] public bool triggered;

    [SerializeField] private ColoringBookPuzzle puzzleManager;

    //function handling if the mouse enters the image
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(puzzleManager.isDrawing)
        {
            puzzleManager.HitAnchor(this);
            Debug.Log("HIT! - " + name);
        }

    }
}
