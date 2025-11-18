using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoombaDispense : MonoBehaviour
{
    // Script created by Mark D. 11/17/2025
    // Handles the roomba dispensing objects when it breaks
    // add prefabs into the dispenseObjects in the inspector and adjust the upwards/sideways forces

    public List<GameObject> dispenseObjects;
    public Transform dispensePoint;
    public float sideForce = 2.5f;
    public float upwardForce = 5f;

    public void Dispense()
    {
        StartCoroutine(DispenseRoutine());
    }

    private IEnumerator DispenseRoutine()
    {
        foreach (GameObject obj in dispenseObjects)
        {
            GameObject spawned = Instantiate(obj, dispensePoint.position, dispensePoint.rotation);

            Rigidbody rb = spawned.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // random outward direction on the horizontal plane
                Vector3 randomDir = new Vector3(
                    Random.Range(-1f, 1f),
                    0f,
                    Random.Range(-1f, 1f)
                ).normalized;

                rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                rb.AddForce(randomDir * sideForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
