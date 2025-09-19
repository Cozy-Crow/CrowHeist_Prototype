using UnityEngine;

public class BubbleBehavior : MonoBehaviour
{
    public float floatSpeed = 0.5f;
    public float fallSpeed = 0.5f;
    public float maxHeight = 5f;
    public float minHeight = 0f;

    private bool exceededWeight = false;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        Vector3 pos = transform.position;

        if (exceededWeight)
        {
            if (pos.y > startPosition.y + minHeight)
            {
                pos.y -= fallSpeed * Time.deltaTime;
            }
        }
        else
        {
            if (pos.y < startPosition.y + maxHeight)
            {
                pos.y += floatSpeed * Time.deltaTime;
            }
        }

        transform.position = pos;
    }

    // Use OnTriggerEnter instead of OnCollisionEnter
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Detected");
        if (other.CompareTag("Player"))
        {
            exceededWeight = true;
            Debug.Log("Player touched bubble - start falling");
        }
    }
}
