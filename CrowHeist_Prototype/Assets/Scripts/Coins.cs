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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HeistZone"))
        {
            GameManager.Score += _coinValue;
            UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);
            //SoundManager.instance.PlaySFXByClip(_coinSound);
            //SoundManager.instance.PlaySFX();
            AudioManager.Instance.PlayOneShot(CubeCollectedSound);
            float currentValue;
            MusicManager.Instance.CurrentMusicInstance.getParameterByName("TrinketsCollected", out currentValue);
            float newValue = currentValue += 1;
            MusicManager.SetParameterByName("TrinketsCollected", + newValue);
            MusicManager.Instance.CurrentMusicInstance.getParameterByName("TrinketsCollected", out float value1);
            playerController.Drop();
            Destroy(gameObject);
        }
    }
}
