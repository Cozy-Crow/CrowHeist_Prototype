using UnityEngine;
using FMODUnity;

public class CollectableCoin: MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private int _pointValue = 1;
    [SerializeField] private float _rotateSpeed = 25f;
    [SerializeField] private bool _rotateObject = true;

    [Header("Audio")]
    [SerializeField] private EventReference _collectSound;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _collectParticlePrefab;
    [SerializeField] private GameObject _popupPrefab;
    [SerializeField] private bool _spawnUIPopup = true;

    void Start()
    {
    }
    
    void Update()
    {
        if (_rotateObject)
        {
            transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add points to score
            GameManager.Score += _pointValue;

            // Update UI if available
            if (UIManager.Instance != null)
            {
                UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);
            }

            // Play collection sound
            if (AudioManager.Instance != null && !_collectSound.IsNull)
            {
                AudioManager.Instance.PlayOneShot(_collectSound);
            }

            // Spawn particle effect
            if (_collectParticlePrefab != null)
            {
                Instantiate(_collectParticlePrefab, transform.position, Quaternion.identity);
            }

            // Spawn UI popup (no scene setup required!)
            if (_spawnUIPopup && _popupPrefab != null)
            {
                CollectiblePopupManager.ShowPopup(transform.position, _pointValue, _popupPrefab);
            }
            
            // Destroy collectible
            Destroy(gameObject);
        }
    }


}
