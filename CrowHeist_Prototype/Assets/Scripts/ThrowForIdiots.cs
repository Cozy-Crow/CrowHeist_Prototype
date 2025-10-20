using System.Collections;
using UnityEngine;

public class ThrowForIdiots : MonoBehaviour
{
    [SerializeField] private GameObject spawnObject, bookCover;
    [SerializeField] private Transform spawnPoint;
    // [SerializeField] private Animator bookAnimator; // For animation-based opening

    private Rigidbody rb;
    private bool hasLanded, wasThrown;

    void Start() => rb = GetComponent<Rigidbody>();

    void Update()
    {
        if (rb && !hasLanded)
        {
            if (rb.velocity.magnitude > 1f) wasThrown = true;
            if (wasThrown && rb.velocity.magnitude < 0.1f)
            {
                hasLanded = true;
                rb.isKinematic = true;
                if (bookCover) StartCoroutine(SmoothOpen());

                // Alternative: Animation-based opening
                // if (bookAnimator) bookAnimator.SetTrigger("Open");
            }
        }
    }

    IEnumerator SmoothOpen()
    {
        var startPos = bookCover.transform.localPosition;
        var startRot = bookCover.transform.localEulerAngles;
        var targetPos = startPos + new Vector3(-1f, 0, 0.238f);
        var targetRot = startRot + new Vector3(0, 180f, 0);

        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            float p = t;
            bookCover.transform.localPosition = Vector3.Lerp(startPos, targetPos, p) + Vector3.up * (Mathf.Sin(p * Mathf.PI) * 0.2f);
            bookCover.transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, p);
            yield return null;
        }

        bookCover.transform.localPosition = targetPos;
        bookCover.transform.localEulerAngles = targetRot;

        SpawnItem();
    }

    void SpawnItem()
    {
        if (spawnObject && spawnPoint) Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
    }

    // Animation-based opening (call this from animation event)
    // public void OnBookOpenComplete()
    // {
    //     SpawnItem();
    // }
}
