using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ColoringBookAnchor : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] public int index;
    [SerializeField] public bool triggered;

    [SerializeField] private ColoringBookPuzzle puzzleManager;


    void Update()
    {
        if(triggered)
        {
            GetComponent<Image>().color = Color.green;
        }
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
}
