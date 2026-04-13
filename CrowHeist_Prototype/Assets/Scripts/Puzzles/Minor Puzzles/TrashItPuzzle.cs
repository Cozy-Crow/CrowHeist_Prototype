using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class TrashITPuzzle : MonoBehaviour
{

    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float shootForce = 10f;
    [SerializeField] private GameObject trashBallPrefab;
    [SerializeField] EventReference trashballCollect;
    [SerializeField] EventReference trashPuzzleWin;
    
    [SerializeField] int trashballThreshold;

    private List<Vector3> originalPositions = new List<Vector3>();
    private List<Quaternion> originalRotations = new List<Quaternion>();
    private int collectedCount = 0;
    private bool puzzleCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                StartCoroutine(PushPlayerUp(playerRb));
            }
        }
        else if (other.CompareTag("TrashBall") && !puzzleCompleted)
        {
            originalPositions.Add(other.transform.position);
            originalRotations.Add(other.transform.rotation);
            
            Destroy(other.gameObject);
            AudioManager.Instance?.PlayOneShot3D(trashballCollect, transform.position);
            collectedCount++;

            if (collectedCount == trashballThreshold)
            {
                puzzleCompleted = true;
                AudioManager.Instance?.PlayOneShot3D(trashPuzzleWin, transform.position);
                StartCoroutine(RespawnAndShootBalls());
                GameObject spawnedObject = Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
                Vector3 skewedDirection = new Vector3(0.2f, 1f, 0.2f).normalized;
                spawnedObject.GetComponent<Rigidbody>().AddForce(skewedDirection * 10f, ForceMode.Impulse);
            }
        }
    }

    private IEnumerator PushPlayerUp(Rigidbody playerRb)
    {
        yield return new WaitForSeconds(0.2f);
        if (playerRb != null)
        {
            playerRb.AddForce(Vector3.up * 8f, ForceMode.Impulse);
        }
    }

    private IEnumerator RespawnAndShootBalls()
    {
        yield return new WaitForSeconds(0.2f);

        for (int i = 0; i < originalPositions.Count; i++)
        {
            GameObject newBall = Instantiate(trashBallPrefab, spawnPoint.position, originalRotations[i]);
            Vector3 skewedDirection = new Vector3(Random.Range(-0.5f, 0.5f), 1f, Random.Range(-0.5f, 0.5f)).normalized;
            newBall.GetComponent<Rigidbody>().AddForce(skewedDirection * shootForce, ForceMode.Impulse);
        }
        
    }
}