using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PaintTube : MonoBehaviour
{
    [Header("Spill Settings")]
    public GameObject paintSpillPrefab;           
    public Transform spillSpawnPoint;             
    public float spillRadius = 0.5f;              
    public float spillInterval = 0.2f;            
    public float spillForce = 1f;                 

    [Header("Tube Capacity")]
    [SerializeField] private float _paintInBucket = 100f;  
    public float paintPerSecond = 20f;                     
    public float minPaintToSpill = 1f;                     

    private bool _isSpilling = false;
    private Coroutine _spillRoutine;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && _paintInBucket >= minPaintToSpill)
        {
            StartSpill();
        }
        if (Input.GetMouseButtonUp(0) || _paintInBucket < minPaintToSpill)
        {
            StopSpill();
        }

    }

    private void StartSpill()
    {
        if (_isSpilling) return;
        _isSpilling = true;
        _spillRoutine = StartCoroutine(SpillRoutine());
    }

    private void StopSpill()
    {
        if (!_isSpilling) return;
        _isSpilling = false;
        if (_spillRoutine != null)
            StopCoroutine(_spillRoutine);
            StopCoroutine(_spillRoutine);
    }

    private IEnumerator SpillRoutine()
    {
        while (_paintInBucket > 0f)
        {
            SpawnPuddle();
            _paintInBucket -= paintPerSecond * spillInterval;
            if (_paintInBucket <= 0f)
            {
                _paintInBucket = 0f;
                StopSpill();
                break;
            }
            yield return new WaitForSeconds(spillInterval);
        }
    }

    private void SpawnPuddle()
    {
        if (paintSpillPrefab == null || spillSpawnPoint == null) return;

        Vector3 spawnPos = spillSpawnPoint.position + (Random.insideUnitSphere * spillRadius);
        spawnPos.y = spillSpawnPoint.position.y;  

        Quaternion spawnRot = Quaternion.Euler(90f, Random.Range(0f, 360f), 0f);

        GameObject puddle = Instantiate(paintSpillPrefab, spawnPos, spawnRot);

        
        Rigidbody rb = puddle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDir = spillSpawnPoint.forward + Vector3.up * 0.2f;
            rb.AddForce(forceDir.normalized * spillForce, ForceMode.VelocityChange);
        }
    }

    public float GetPaintRemainingFraction()
    {
        return Mathf.Clamp01(_paintInBucket / 100f);
    }

    public void Refill(float amount)
    {
        _paintInBucket += amount;
        _paintInBucket = Mathf.Min(_paintInBucket, 100f);
    }
}
