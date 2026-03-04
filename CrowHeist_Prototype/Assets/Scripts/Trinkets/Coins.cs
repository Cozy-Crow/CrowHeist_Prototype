using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using KinematicCharacterController.Examples;

public class Coins : MonoBehaviour
{
    [SerializeField] private int _coinValue = 1;
    [SerializeField] private float _rotateSpeed = 1.0f;
    private GameObject player;
    private Controller2Point5D playerController;
    private Pickable pickableUpScript;
    private UniqueID uniqueID;

    [Header("Audio")]
    [SerializeField] private EventReference collectSound;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _collectParticlePrefab;
    [SerializeField] private GameObject _popupPrefab;

    [Header("Narrative Item")]
    public bool isNarrativeItem = false;

    [Header("Collection Animation")]
    [SerializeField] private Transform teleportPoint; // Where item teleports to before arc
    [SerializeField] private Transform sackTarget; // The sack object
    [SerializeField] private float arcHeight = 3f; // Height of the arc
    [SerializeField] private float arcDuration = 1f; // Time to reach sack

    public int CoinValue { get => _coinValue; set => _coinValue = value; }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);

        // Prevent coin from becoming dirty
        if (pickableUpScript != null)
        {
            pickableUpScript._isDirty = false;
        }
        if (pickableUpScript.pickedUp == true)
        {
            GetComponent<FMODUnity.StudioEventEmitter>().Stop(); //stops emitter audio when picked up
        }
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();
        pickableUpScript = GetComponent<Pickable>();
        uniqueID = GetComponent<UniqueID>();

        if (this.isNarrativeItem == true)
        {
            _coinValue = 3;
        }

        // Auto-find teleport point and sack if not set
        if (teleportPoint == null)
        {
            GameObject tp = GameObject.Find("CollectionTeleportPoint");
            if (tp != null) teleportPoint = tp.transform;
        }

        if (sackTarget == null)
        {
            GameObject sack = GameObject.Find("CollectionSack");
            if (sack != null) sackTarget = sack.transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HeistZone"))
        {
            GameManager.Score += _coinValue;
            UIManager.Instance.CollectionZoneCameraUI.UpdateCoinCounter(GameManager.Score);

            // Show collection zone with narrative popup if applicable
            if (UIManager.Instance.CollectionZoneCameraUI != null)
            {
                // Get sprite from SpriteRenderer if this is a narrative item
                Sprite itemSprite = null;
                if (isNarrativeItem)
                {
                    SpriteRenderer sr = GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        itemSprite = sr.sprite;
                    }
                }

                UIManager.Instance.CollectionZoneCameraUI.ShowCollectionZone(isNarrativeItem, itemSprite);
            }

            // Narrative item: fire collection event to unlock in narrative menu
            if (isNarrativeItem && PickupRegistry.Instance != null && uniqueID != null)
            {
                PickupRegistry.Instance.MarkNarrativeAsCollected(uniqueID);
            }
            // MusicManager.Instance.CurrentMusicInstance.getParameterByName("trinketsCollected", out float currentValue);
             // float newValue = currentValue + 1;
             // MusicManager.SetParameterByName("TrinketsCollected", newValue);

            // Drop item if picked up
            if (pickableUpScript.pickedUp)
            {
                playerController.Drop();
            }

            // Start collection sequence
            StartCoroutine(CollectionSequence());
        }
    }

    private IEnumerator CollectionSequence()
    {
        // Disable physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        // Disable collider so player can't interact during animation
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Show camera UI
        if (UIManager.Instance.CollectionZoneCameraUI != null)
        {
            Sprite itemSprite = null;
            if (isNarrativeItem)
            {
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                if (sr != null) itemSprite = sr.sprite;
            }
            UIManager.Instance.CollectionZoneCameraUI.ShowCollectionZone(isNarrativeItem, itemSprite);
        }

        // Teleport to start position
        if (_collectParticlePrefab != null)
        {
            Instantiate(_collectParticlePrefab, transform.position, Quaternion.identity);
        }

        // Briefly hide coin during "teleport"
        Renderer coinRenderer = GetComponent<Renderer>();
        if (coinRenderer != null) coinRenderer.enabled = false;

        // Teleport to teleport point position
        if (teleportPoint != null)
        {
            transform.position = teleportPoint.position;
            AudioManager.Instance?.PlayOneShot(collectSound);
        }

        // Small delay before reappearing 
        yield return new WaitForSeconds(0.1f);

        if (_collectParticlePrefab != null)
        {
            Instantiate(_collectParticlePrefab, teleportPoint.position, Quaternion.identity);
        }

        // Reshow coin
        if (coinRenderer != null) coinRenderer.enabled = true;

        // Small delay before arc starts
        yield return new WaitForSeconds(0.2f);

        // Arc to sack
        if (sackTarget != null)
        {
            yield return StartCoroutine(ArcToTarget(sackTarget.position));
        }

        // Spawn particle effect at sack
        if (_collectParticlePrefab != null)
        {
            Instantiate(_collectParticlePrefab, sackTarget.position, Quaternion.identity);
        }

        // Update score
        //GameManager.Score += _coinValue;
        UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);

        // MusicManager code (if you want to re-enable it)
       // MusicManager.Instance.CurrentMusicInstance.getParameterByName("trinketsCollected", out float currentValue);
        //float newValue = currentValue + 1;
        //MusicManager.SetParameterByName("TrinketsCollected", newValue);
        // Destroy or hide the item
        MusicManager.Instance.triggerMusicSolo();
        KillObject();
    }

    private IEnumerator ArcToTarget(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < arcDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / arcDuration;

            // Linear interpolation for horizontal movement
            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, t);

            // Add arc (parabola) for vertical movement
            float height = arcHeight * (1 - Mathf.Pow(2 * t - 1, 2)); // Parabolic arc
            currentPos.y += height;

            transform.position = currentPos;

            // Optional: rotate item during flight
            transform.Rotate(Vector3.up, 360f * Time.deltaTime * 2f);

            yield return null;
        }

        // Ensure we end exactly at target
        transform.position = targetPosition;
    }

    void KillObject()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}