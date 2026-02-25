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
    public float minThrowVelocity = 3f;
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
        
        if (other.CompareTag("Player"))
        {
            Controller2Point5D player = other.GetComponent<Controller2Point5D>();
            if(player != null && player.fallingTime >= 0.4f)
            {
                Break();
            }
        }
        else if (other.attachedRigidbody != null && other.attachedRigidbody.velocity.magnitude > minThrowVelocity)
        {
            Break();
        }
        else if (rb != null && rb.velocity.magnitude > minThrowVelocity)
        {
            Break();
        }
        else if (fallTime >= minFallTime)
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
            var spawnedPiece = Instantiate(piece, transform.position, transform.rotation);
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
        
        Destroy(gameObject);
    }

}