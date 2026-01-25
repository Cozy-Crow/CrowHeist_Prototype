using UnityEngine;

public class BreakableObjectFramework : MonoBehaviour
{
    [Header("Breakable Settings")]
    public GameObject[] brokenPieces;
    public ParticleSystem breakEffect;
    public AudioClip breakSound;
    public float BreakForce = 5f;
    
    private bool isBroken = false;
    
    void OnCollisionEnter(Collision collision)
    {
        if (!isBroken && (collision.gameObject.CompareTag("Player") || collision.relativeVelocity.magnitude > 3f))
        {
            Break();
        }
    }
    
    void Break()
    {
        if (isBroken) return;
        isBroken = true;
        
        if (breakEffect) breakEffect.Play();
        if (breakSound) AudioSource.PlayClipAtPoint(breakSound, transform.position);
        
        foreach (var piece in brokenPieces)
        {
            var spawnedPiece = Instantiate(piece, transform.position, Random.rotation);
            var rb = spawnedPiece.GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddForce(Random.insideUnitSphere * BreakForce);
                rb.AddTorque(Random.insideUnitSphere * BreakForce);
            }
            Destroy(spawnedPiece, 5f);
        }
        
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, 1f);
    }
}
