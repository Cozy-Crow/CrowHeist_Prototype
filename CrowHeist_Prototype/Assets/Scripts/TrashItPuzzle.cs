using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashITPuzzle : MonoBehaviour
{

    [SerializeField] private GameObject spawnObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float shootForce = 10f;

    private List<GameObject> trashBalls = new List<GameObject>();
    private bool puzzleCompleted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TrashBall") && !puzzleCompleted)
        {
            trashBalls.Add(other.gameObject);

            if (trashBalls.Count == 4)
            {
                puzzleCompleted = true;
                
                foreach (GameObject ball in trashBalls)
                {
                    ball.GetComponent<Rigidbody>().AddForce(Vector3.up * shootForce, ForceMode.Impulse);
                }

                GameObject spawnedObject = Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
                Vector3 skewedDirection = new Vector3(0.2f, 1f, 0.2f).normalized;
                spawnedObject.GetComponent<Rigidbody>().AddForce(skewedDirection * 10f, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("TrashBall"))
        {
            trashBalls.Remove(other.gameObject);
        }
    }
}