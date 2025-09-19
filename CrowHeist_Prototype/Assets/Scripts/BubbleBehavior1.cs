using UnityEngine;

public class BubbleBehaviorTopCheck : MonoBehaviour
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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                // Check if the contact point is near the top of the bubble
                Vector3 bubbleTop = transform.position + new Vector3(0, GetComponent<Collider>().bounds.extents.y, 0);
                if (contact.point.y >= bubbleTop.y - 0.1f) 
                {
                    exceededWeight = true;
                    Debug.Log("Player is on top - bubble falls");
                    break;
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            exceededWeight = false;
            Debug.Log("Player left - bubble floats");
        }
    }
}
