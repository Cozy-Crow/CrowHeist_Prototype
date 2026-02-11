using UnityEngine;

public class BreakableObjectFramework : MonoBehaviour
{
    [Header("Breakable Settings")]
    public GameObject[] brokenPieces;
    public ParticleSystem breakEffect;
    public AudioClip breakSound;
    public float BreakForce = 5f;
    public GameObject coinPrefab;
    public float minThrowVelocity = 3f;
    
    private bool isBroken = false;
    
    void OnCollisionEnter(Collision collision)
    {
        if (isBroken) return;
        
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.relativeVelocity.y < -2f)
            {
                Break();
            }
        }
        else if (collision.relativeVelocity.magnitude > minThrowVelocity)
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
        }
        
        if (coinPrefab)
        {
            var coin = Instantiate(coinPrefab, transform.position + Vector3.up, Quaternion.identity);
            var coinRb = coin.GetComponent<Rigidbody>();
            if (coinRb) coinRb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
        
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, 1f);
    }
}
