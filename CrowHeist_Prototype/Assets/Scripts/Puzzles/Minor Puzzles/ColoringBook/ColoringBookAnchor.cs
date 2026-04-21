using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FMODUnity;

public class ColoringBookAnchor : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] public int index;
    [SerializeField] private ColoringBookPuzzle puzzleManager;
    [SerializeField] private EventReference dingSFX;

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

    public void OnPointerClick(PointerEventData eventData)
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
        AudioManager.Instance?.PlayOneShot(dingSFX);
    }

    public void disableAnchor()
    {
        GetComponent<Image>().color = Color.red;
    }
}
