using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KinematicCharacterController.Examples;
using Unity.VisualScripting;

public class BreakableObjectFramework : MonoBehaviour
{
    [Header("Breakable Settings")]
    public GameObject[] brokenPieces;
    public ParticleSystem breakEffect;
    public AudioClip breakSound;
    public float BreakForce = 5f;
    public GameObject coinPrefab;
    public float minThrowVelocity = 8f;
    public float minFallTime = 0.5f;
    private bool isBroken = false;
    private Rigidbody rb;
    private float fallTime = 0f;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (rb != null && rb.velocity.y < -0.1f)
        {
            fallTime += Time.deltaTime;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (isBroken) return;
        
        // if (other.CompareTag("Player"))
        // {
        //     Controller2Point5D player = other.GetComponent<Controller2Point5D>();
        //     if(player != null && player.fallingTime >= 0.4f)
        //     {
        //         Break();
        //     }
        // }
        
        if (other.CompareTag("Player") && other.attachedRigidbody != null)
        {
            return;
        }
        else if (other.attachedRigidbody != null && other.attachedRigidbody.velocity.magnitude > minThrowVelocity)
        {
            Break();
        }
        else if (rb.velocity.magnitude > minThrowVelocity)
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
        
        Vector3 currentVelocity = rb != null ? rb.velocity : Vector3.zero;
        Vector3 currentAngularVelocity = rb != null ? rb.angularVelocity : Vector3.zero;
        
        foreach (var piece in brokenPieces)
        {
            var spawnedPiece = Instantiate(piece, transform.position, transform.rotation);
            var pieceRb = spawnedPiece.GetComponent<Rigidbody>();
            if (pieceRb)
            {
                pieceRb.velocity = currentVelocity;
                pieceRb.angularVelocity = currentAngularVelocity;
                pieceRb.AddForce(Random.insideUnitSphere * BreakForce, ForceMode.Impulse);
                pieceRb.AddTorque(Random.insideUnitSphere * BreakForce, ForceMode.Impulse);
            }
        }
        
        if (coinPrefab)
        {
            var coin = Instantiate(coinPrefab, transform.position + Vector3.up, Quaternion.identity);
            var coinRb = coin.GetComponent<Rigidbody>();
            if (coinRb) coinRb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }
        
        Destroy(gameObject);
    }
}