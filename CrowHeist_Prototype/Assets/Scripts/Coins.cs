using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [SerializeField] private int _coinValue = 1;
    [SerializeField] private float _rotateSpeed = 1.0f;

    public int CoinValue { get => _coinValue; set => _coinValue = value; }

    //[SerializeField] private AudioClip _coinSound;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Score += _coinValue;
            UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);
            //SoundManager.instance.PlaySFXByClip(_coinSound);
            //SoundManager.instance.PlaySFX();
            Destroy(gameObject);
        }
    }
}
