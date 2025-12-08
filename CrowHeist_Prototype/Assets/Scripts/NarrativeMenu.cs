using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class NarrativeMenu : MonoBehaviour
{
    [SerializeField] private GameObject NarrativeMenuFirst;
    [SerializeField] private GameObject NarrativeMenuSecond;
    private bool on = false;

    // Start is called before the first frame update
    void Start()
    {
        on = false;
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
        on = !on; // Flip the boolean
        NarrativeMenuFirst.SetActive(on);
        // Also toggle the second menu if needed
        // NarrativeMenuSecond.SetActive(on);
    }
}