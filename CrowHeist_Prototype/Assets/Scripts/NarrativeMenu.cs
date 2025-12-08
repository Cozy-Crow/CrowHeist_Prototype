using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NarrativeMenu : MonoBehaviour
{
    [SerializeField] private GameObject NarrativeMenuFirst;
    [SerializeField] private GameObject NarrativeMenuSecond;
    [SerializeField] private Button ItemSelectButton;
    
    private bool isFirstMenuActive = false;
    private bool isSecondMenuActive = false;

    // Start is called before the first frame update
    void Start()
    {
        if (ItemSelectButton != null)
        {
            ItemSelectButton.onClick.AddListener(ItemSelected);
        }
        else
        {
            Debug.Log("Item Button is not assigned!");
        }
        
        isFirstMenuActive = false;
        isSecondMenuActive = false;
        NarrativeMenuFirst.SetActive(false);
        NarrativeMenuSecond.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        // If second menu is active, turn it off
        if (isSecondMenuActive)
        {
            isSecondMenuActive = false;
            NarrativeMenuSecond.SetActive(false);
        }
        // If first menu is active, turn it off
        else if (isFirstMenuActive)
        {
            isFirstMenuActive = false;
            NarrativeMenuFirst.SetActive(false);
        }
        // If both are off, turn on the first menu
        else
        {
            isFirstMenuActive = true;
            NarrativeMenuFirst.SetActive(true);
        }
    }

    public void ItemSelected()
    {
        // Switch from first menu to second menu
        isFirstMenuActive = false;
        isSecondMenuActive = true;
        NarrativeMenuFirst.SetActive(false);
        NarrativeMenuSecond.SetActive(true);
    }
}