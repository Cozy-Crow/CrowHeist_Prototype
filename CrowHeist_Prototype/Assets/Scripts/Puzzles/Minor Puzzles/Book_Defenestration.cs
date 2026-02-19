using System.Collections;
using UnityEngine;

public class Book_Defenestration : MonoBehaviour
{
    [SerializeField] private GameObject closedBookPrefab;
    [SerializeField] private GameObject openBookPrefab;
    [SerializeField] private string windowTag = "Window";
    
    private bool puzzleSolved = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!puzzleSolved && other.CompareTag(windowTag))
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;
        if (closedBookPrefab != null) closedBookPrefab.SetActive(false);
        if (openBookPrefab != null) openBookPrefab.SetActive(true);
    }

    // Old animation method:
    // [SerializeField] private Animator bookAnimator;
    // [SerializeField] private GameObject coin;
    // bookAnimator.SetTrigger("Open");
    // if (coin != null) coin.SetActive(true);}
}
