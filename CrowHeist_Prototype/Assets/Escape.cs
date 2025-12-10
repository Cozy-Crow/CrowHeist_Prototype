using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour
{
    [SerializeField] private GameObject Blocker;

    void Update()
    {
        if(GameManager.Score >= 5 || GameManager.AltCoinsScore >= 5)
        {
            Blocker.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (GameManager.Score >= 5 || GameManager.AltCoinsScore >= 5))
        {
            GameManager.Instance.TriggerGameEnd();
        }
    }
}
