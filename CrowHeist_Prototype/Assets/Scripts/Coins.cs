using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    [SerializeField] private int _coinValue = 1;
    [SerializeField] private float _rotateSpeed = 1.0f;
    [SerializeField] private AudioClip _coinSound;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.Score += _coinValue;
            SoundManager.instance.PlaySFXByClip(_coinSound);
            SoundManager.instance.PlaySFX();
            Destroy(gameObject);
        }
    }
}
