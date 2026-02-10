using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using KinematicCharacterController.Examples;

public class Coins : MonoBehaviour
{
    [SerializeField] private int _coinValue = 1;
    [SerializeField] private float _rotateSpeed = 1.0f;
    private GameObject player;
    private Controller2Point5D playerController;
    private Pickable pickableUpScript;

     [Header("Audio")]
     [SerializeField] private EventReference coinCollect;
     [SerializeField] private EventReference coinEmitter;
    private EventInstance coinInstance;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _collectParticlePrefab;
    [SerializeField] private GameObject _popupPrefab;

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
        //coinInstance = AudioManager.Instance.CreateInstance(coinEmitter);
        //coinInstance.start();
    }
    private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("HeistZone"))
    {
        GameManager.Score += _coinValue;
        UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);     
    
        if (UIManager.Instance.CollectionZoneCameraUI != null)
        {
            UIManager.Instance.CollectionZoneCameraUI.ShowCollectionZone();
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

        AudioManager.Instance?.PlayOneShot(coinCollect);   

        KillObject();
    }
}
    
    void KillObject()
    {
        //Destroy(gameObject);
        gameObject.SetActive(false);
    }
}
