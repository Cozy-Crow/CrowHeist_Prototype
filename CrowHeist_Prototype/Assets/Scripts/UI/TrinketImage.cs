using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TrinketImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private TrinketMenu trinketMenu;
    [SerializeField] private Image image;
    [SerializeField] private Image selection;   //border
    [SerializeField] private int index;
    [SerializeField] private bool isUnlocked;

    [Header("Tweening Settings")]
    [SerializeField] private Vector3 scaleUp = new Vector3(1.1f, 1.1f, 1.1f);    
    [SerializeField] private float scaleDuration = 0.2f;
    [SerializeField] private Ease scaleUpEase = Ease.OutBack;
    [SerializeField] private Ease scaleDownEase = Ease.InBack;

    public Sprite IconSprite { get => image.sprite; set => image.sprite = value;}
    public bool IsUnlocked {get => isUnlocked; set => isUnlocked = value;}

    void Awake()
    {
        selection.color = new Color(selection.color.r, selection.color.g, selection.color.b, 0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(image.sprite == trinketMenu.DefaultSprite) return;

        AudioManager.Instance.PlayOneShot(trinketMenu.hoverTrinketSFX);
        image.transform.DOScale(scaleUp, scaleDuration).SetEase(scaleUpEase).SetUpdate(true);

        selection.DOColor(
            new Color(selection.color.r, 
            selection.color.g, 
            selection.color.b, 
            1), 0.1f).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(image.sprite == trinketMenu.DefaultSprite) return;
        image.transform.DOScale(Vector3.one, scaleDuration).SetEase(scaleDownEase).SetUpdate(true);

        selection.DOColor(
            new Color(selection.color.r, 
            selection.color.g, 
            selection.color.b, 
            0), 0.1f).SetUpdate(true);

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(image.sprite == trinketMenu.DefaultSprite) return;

        if(!isUnlocked)
        {
            AudioManager.Instance.PlayOneShot(trinketMenu.lockTrinketSFX);
        }
        else
        {
            AudioManager.Instance.PlayOneShot(trinketMenu.selectTrinketSFX);
        }
        trinketMenu.SelectItem(index);
    }
}
