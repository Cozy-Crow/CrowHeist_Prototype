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

    [Header("Audio")]
    [SerializeField] private EventReference _collectSound;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _collectParticlePrefab;
    [SerializeField] private GameObject _popupPrefab;

    public int CoinValue { get => _coinValue; set => _coinValue = value; }
    
    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();
        pickableUpScript = GetComponent<Pickable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HeistZone"))
        {
            GameManager.Score += _coinValue;
            UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);
            //SoundManager.instance.PlaySFXByClip(_coinSound);
            //SoundManager.instance.PlaySFX();

            #warning starting from TinkerfestScene causes the music manager to not be loaded causing the below code to bug out
            // AudioManager.Instance.PlayOneShot(CubeCollectedSound);
            // float currentValue;

            
            // MusicManager.Instance.CurrentMusicInstance.getParameterByName("trinketsCollected", out currentValue);
            // float newValue = currentValue += 1;
            // MusicManager.SetParameterByName("TrinketsCollected", + newValue);
            // MusicManager.Instance.CurrentMusicInstance.getParameterByName("trinketsCollected", out float value1);

            //works if player runs into heist zone
            //if player throws coin and picks up another item, it will drop what they pick up
            
            //if the item is picked up, ONLY THEN drop the item
            if(pickableUpScript.pickedUp)
            {
                playerController.Drop();
            }
            

            // playerController.ItemWasDestroyed();
            // Spawn particle effect
            if (_collectParticlePrefab != null)
            {
                Instantiate(_collectParticlePrefab, transform.position, Quaternion.identity);
            }

            //run in a function to allow the item to drop first
            KillObject();
        }
    }
    
    void KillObject()
    {
        Destroy(gameObject);
    }
}
