using UnityEngine;

public class PaintProjectile : MonoBehaviour
{
    public GameObject paintSplatterPrefab;
    public float lifeAfterHit = 0.1f;

    void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];

        Quaternion paintRotation = Quaternion.LookRotation(contact.normal);

        
        GameObject splatter = Instantiate(
            paintSplatterPrefab,
            contact.point + contact.normal * 0.01f,
            paintRotation
        );

        float size = Random.Range(0.4f, 0.9f);
        splatter.transform.localScale = Vector3.one * size;

        Destroy(gameObject, lifeAfterHit);
    }
}
