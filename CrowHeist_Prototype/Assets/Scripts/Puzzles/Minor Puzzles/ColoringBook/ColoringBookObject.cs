using System;
using System.Collections;
using Cinemachine;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ColoringBookObject : Interactable
{
    //Created by ZackH 2/4
    //Script handling everything related to the coloring book object!

    [SerializeField] GameObject menu; //menu linked to the puzzle
    [SerializeField] Controller2Point5D crowley; //reference to crowley
    [SerializeField] public Button closeButton; //close button on the menu
    [SerializeField] public GameObject puzzleController; //reference to the script on the ui (controls the actual puzzle)
    bool isInteractable = true; //tells whether or not the book can be interacted with
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform coinSpawnPoint;

    //animation vars
    float animTime = 0.5f; //holds animation time for the ending animation for the puzzle
    [SerializeField] SpriteRenderer spriteRenderer; //sprites
    [SerializeField] Sprite sprite1; //3 sprites showing after each slam
    [SerializeField] Sprite sprite2;
    [SerializeField] Sprite sprite3;
    [SerializeField] GameObject openBook; //open and closed book models
    [SerializeField] GameObject closedBook;
    [SerializeField] CinemachineVirtualCamera playerCam; //cam to swap for the ending anim
    [SerializeField] Transform camPoint;


    // Start is called before the first frame update
    void Start()
    {
        //Get crowley reference
        crowley = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Controller2Point5D>();

        spriteRenderer.enabled = false; //turn of sprite initally

        if(menu == null)
        {
            Debug.LogWarning("Coloring Book UI not connected!");
        }

        if(puzzleController == null)
        {
            Debug.LogWarning("Coloring Book Puzzle not connected!");
        } 

        // add listener for button
        // close menu on click
        closeButton.onClick.AddListener(CloseMenu);

        // hide UI on start
        menu.gameObject.SetActive(false);
        // Debug.Log("Starting");

        // Debug.Log("ClosedBook ref: " + closedBook);
        // Debug.Log("OpenBook ref: " + openBook);

        openBook.SetActive(false);
        closedBook.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CloseMenu()
    {
        //close UI
        menu.gameObject.SetActive(false);
        //enable movement
        crowley.SetCanInput(true);
        //stop the puzzle
        puzzleController.GetComponent<ColoringBookPuzzle>().ClosePuzzle();
    }

    public void EndPuzzle()
    {
        //start end puzzle sequence
        StartCoroutine(EndPuzzleRoutine());
    }

    //handles the End of Puzzle Routine (playing the animation/spitting out coin)
    IEnumerator EndPuzzleRoutine()
    {
        Debug.Log("COLORINGBOOK Starting end Routine");

        //stall closing the menu - pause for 2 seconds
        yield return new WaitForSeconds(1);

        //close UI
        menu.gameObject.SetActive(false);
        //hide crowley
        crowley.GetComponentInChildren<SpriteRenderer>().enabled = false;

        playerCam.Follow = camPoint; //set camera point for the cutscene

        //run through animation (semi scuffed bc of using enabling/disabling physical objects atm)
        yield return new WaitForSeconds(animTime); //.25 sec before it starts

        //open book 1
        OpenBook();
        // yield return new WaitForSeconds(0.05f); //.05 before showing
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite1;

        //close book 2
        yield return new WaitForSeconds(animTime); //.25 before next
        spriteRenderer.enabled = false;
        CloseBook();

        //open book 2
        yield return new WaitForSeconds(animTime); //.25 before next
        OpenBook();
        // yield return new WaitForSeconds(0.05f); //.05 before showing
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite2;

        //close book 3
        yield return new WaitForSeconds(animTime); //.25 before next
        spriteRenderer.enabled = false;
        CloseBook();

        //open book 3
        yield return new WaitForSeconds(animTime); //.25 before next
        OpenBook();
        // yield return new WaitForSeconds(0.05f); //.05 before showing
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite3;

        //close book final
        yield return new WaitForSeconds(animTime); //.25 before next
        spriteRenderer.enabled = false;
        CloseBook();

        //open book final
        yield return new WaitForSeconds(animTime); //.25 before next
        OpenBook();
        yield return new WaitForSeconds(0.2f); //.2 before shooting our reward

        //spawn the reward
        SpawnReward();
        yield return new WaitForSeconds(0.5f);

        ////closing sequence - Enable input, disable ability to interact, show crowley
        //enable movement
        crowley.GetComponentInChildren<SpriteRenderer>().enabled = true;
        crowley.SetCanInput(true);
        crowley.CamFocusOnCrowley(); //refocus cam onto crowley
        //play animation, pop reward
        //remove interaction capability
        isInteractable = false;
    }

    //function handling on interaction
    public override void TriggerInteraction(Pickable item)
    {
        if(isInteractable)
        {
            // Debug.Log("COLORINGBOOK inside trigger");
            //show UI
            menu.gameObject.SetActive(true);
            // //disable player movement
            crowley.SetCanInput(false);
            //start the puzzle
            StartCoroutine(startRoutine());
        }
    }

    IEnumerator startRoutine()
    {
        yield return new WaitForSeconds(.005f);
        puzzleController.GetComponent<ColoringBookPuzzle>().StartPuzzle();
    }

    private void OpenBook()
    {
        //normal close book sequence
        closedBook.gameObject.SetActive(false);
        openBook.gameObject.SetActive(true);
    }

    private void CloseBook()
    {
        //normal close book sequence
        openBook.gameObject.SetActive(false);
        closedBook.gameObject.SetActive(true);
        closedBook.transform.eulerAngles = new Vector3(0,-180,-90); //rotate so book is on top

        //play sfx here probably (book slamming closed + crowley hurt?)
    }

    private void SpawnReward()
    {
        //spawn coin
        GameObject spawnedItem = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        //shoot it up
        spawnedItem.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }
}
