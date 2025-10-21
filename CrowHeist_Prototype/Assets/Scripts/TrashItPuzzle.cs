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

                Instantiate(spawnObject, spawnPoint.position, spawnPoint.rotation);
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