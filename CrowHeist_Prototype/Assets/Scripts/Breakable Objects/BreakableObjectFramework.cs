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
    
    private bool isBroken = false;
    
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
        else if (other.attachedRigidbody != null && other.attachedRigidbody.velocity.magnitude < 0.1f)
        {
            StartCoroutine(CheckForFall(other.attachedRigidbody));
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
        
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        Destroy(gameObject, 1f);
    }

    private IEnumerator CheckForFall(Rigidbody rb)
    {
        float fallTime = 0f;
        while (rb.velocity.magnitude < 0.1f)
        {
            fallTime += Time.deltaTime;
            if (fallTime >= 0.5f)
            {
                Break();
                yield break;
            }
            yield return null;
        }
    }
}