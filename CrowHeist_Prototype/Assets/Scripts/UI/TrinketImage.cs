using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TrinketImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TrinketMenu trinketMenu;
    [SerializeField] private int index;
    [SerializeField] private bool isUnlocked;    
    private Image image;

    public bool IsUnlocked {get => isUnlocked; set => isUnlocked = value;}

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(image.sprite == trinketMenu.DefaultSprite) return;

        AudioManager.Instance.PlayOneShot(trinketMenu.hoverTrinket);
        gameObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(image.sprite == trinketMenu.DefaultSprite) return;
        gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(image.sprite == trinketMenu.DefaultSprite) return;

        if(!isUnlocked)
        {
            AudioManager.Instance.PlayOneShot(trinketMenu.lockTrinket);
        }
        else
        {
            AudioManager.Instance.PlayOneShot(trinketMenu.selectTrinket);
        }
        trinketMenu.SelectItem(index);
        trinketMenu.Animator.Play("Select");
    }
}
