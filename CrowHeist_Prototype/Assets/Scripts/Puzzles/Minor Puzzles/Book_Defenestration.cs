using System.Collections;
using UnityEngine;

public class Book_Defenestration : MonoBehaviour
{
    [SerializeField] private Mesh openBookMesh;
    [SerializeField] private GameObject coin;
    [SerializeField] private string windowTag = "Window";
    [SerializeField] private float pauseDuration = 1f;
    [SerializeField] private float coinPopDelay = 0.5f;
    
    private bool puzzleSolved = false;
    private Rigidbody rb;
    private MeshFilter meshFilter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!puzzleSolved && other.CompareTag(windowTag))
        {
            StartCoroutine(SolvePuzzle());
        }
    }

    private IEnumerator SolvePuzzle()
    {
        puzzleSolved = true;
        
        Vector3 velocity = rb.velocity;
        rb.isKinematic = true;
        
        yield return new WaitForSeconds(pauseDuration);
        
        if (coin != null) coin.SetActive(true);
        
        yield return new WaitForSeconds(coinPopDelay);
        
        if (meshFilter != null && openBookMesh != null) meshFilter.mesh = openBookMesh;
        
        rb.isKinematic = false;
        rb.velocity = velocity;
    }

    // alternate animation method:
    // [SerializeField] private Animator bookAnimator;
    // [SerializeField] private GameObject coin;
    // bookAnimator.SetTrigger("Open");
    // if (coin != null) coin.SetActive(true);}
}
