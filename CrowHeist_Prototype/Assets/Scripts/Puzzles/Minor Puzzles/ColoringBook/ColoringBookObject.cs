using System;
using System.Collections;
using Cinemachine;
using KinematicCharacterController.Examples;
using UnityEngine;
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
    [SerializeField] Transform crowleyEndPoint;

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

    //Camera shake
	[SerializeField] float shakeDuration = 0f;
    float shakeDurationInitial; 
	[SerializeField] float shakeAmount = 0.7f;
	[SerializeField] float decreaseFactor = 1.0f;
    Vector3 originalPos;
    bool isShaking;
    bool shakeOnThisFrame = true;
    int shakeFrameCounter; //count the frames
    [SerializeField] int shakeAfterFrames; //every __ frames the camera should shake

    //other vars
    Collider[] boxColliders;
    Collider mainCollider;


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

        // openBook.SetActive(false);
        // closedBook.SetActive(true);
        openBook.gameObject.GetComponent<MeshRenderer>().enabled = false;
        originalPos = camPoint.localPosition;

        boxColliders = GetComponents<BoxCollider>();
        GetMainCollider();

        shakeDurationInitial = shakeDuration;
    }

    void GetMainCollider()
    {
        foreach (Collider collider in boxColliders)
        {
            if(!collider.isTrigger)
                mainCollider = collider;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if the camera should shake
        if(isShaking)
        {
            //every ___ frames shake the camera
            if(shakeAfterFrames < shakeFrameCounter)
            {
                shakeFrameCounter = 0;
                StartShake();
            }
            else
                shakeFrameCounter++;

            // //alternate what frames we shake the camera to make it less jarring
            // if(shakeOnThisFrame)
            // {
            //     shakeOnThisFrame = false;
            //     StartShake();
            // }
            // else
            //     shakeOnThisFrame = true;
        }

        //make the sprite look at the camera so its easier to see
        spriteRenderer.gameObject.transform.LookAt(playerCam.transform);
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
        mainCollider.enabled = false;
        
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
        yield return new WaitForSeconds(0.3f); //.3 before shooting our reward

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

        crowley.transform.position = crowleyEndPoint.position;
        mainCollider.enabled = true;
    }

    //function handling on interaction
    public override void TriggerInteraction(Pickable item)
    {
        if(isInteractable && menu.gameObject.activeSelf == false)
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
        closedBook.gameObject.GetComponent<MeshRenderer>().enabled = false;
        openBook.gameObject.GetComponent<MeshRenderer>().enabled = true;
        isShaking = false;
		camPoint.localPosition = originalPos;
        // closedBook.gameObject.SetActive(false);
        // openBook.gameObject.SetActive(true);
    }

    private void CloseBook()
    {
        //normal close book sequence
        closedBook.gameObject.GetComponent<MeshRenderer>().enabled = true;
        openBook.gameObject.GetComponent<MeshRenderer>().enabled = false;
        isShaking = true;
        shakeDuration = shakeDurationInitial;
        // openBook.gameObject.SetActive(false);
        // closedBook.gameObject.SetActive(true);
        
        // --- play sfx here probably (book slamming closed + crowley hurt?) ---

        closedBook.transform.eulerAngles = new Vector3(0,-180,-90); //rotate so book is on top

    }

    private void SpawnReward()
    {
        //spawn coin
        GameObject spawnedItem = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        //shoot it up
        spawnedItem.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

	void StartShake()
	{
		if (shakeDuration > 0)
		{
			camPoint.localPosition = originalPos + UnityEngine.Random.insideUnitSphere/2 * shakeAmount;
			shakeDuration -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shakeDuration = 0f;
			camPoint.localPosition = originalPos;
		}
	}


}
