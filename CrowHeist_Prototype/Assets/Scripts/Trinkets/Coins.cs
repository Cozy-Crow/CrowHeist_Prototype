using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using KinematicCharacterController.Examples;

public class Coins : MonoBehaviour
{
    [SerializeField] private int _coinValue = 1;
    [SerializeField] private float _rotateSpeed = 1.0f;
    [SerializeField] private EventReference CubeCollectedSound;
    private GameObject player;
    private Controller2Point5D playerController;
    private Pickable pickableUpScript;
    private UniqueID uniqueID;

    [Header("Audio")]
    [SerializeField] private EventReference _collectSound;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _collectParticlePrefab;
    [SerializeField] private GameObject _popupPrefab;

    [Header("Narrative Item")]
    public bool isNarrativeItem = false;

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
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();
        pickableUpScript = GetComponent<Pickable>();
        uniqueID = GetComponent<UniqueID>();

        if(this.isNarrativeItem == true)
        {
            _coinValue = 3;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HeistZone"))
        {
            GameManager.Score += _coinValue;
            UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);

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

            if(pickableUpScript.pickedUp)
            {
                playerController.Drop();
            }

            if (_collectParticlePrefab != null)
            {
                Instantiate(_collectParticlePrefab, transform.position, Quaternion.identity);
            }

            // Remove the KillObject() call - coin will remain in the scene
            // KillObject();
        }
    }
    
    void KillObject()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}