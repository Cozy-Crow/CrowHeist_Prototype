using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;

public class AltWinCoins : MonoBehaviour
{

    [SerializeField] private int altCoinValue = 1;
    [SerializeField] private float altCoinRotateSpeed = 0.5f;
    private GameObject player;
    private Controller2Point5D playerController;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.AltCoinsScore += altCoinValue;
            UIManager.Instance.AltWinCoinsUI.UpdateScoreUI();
            Destroy(gameObject);
        }
    }
}
