using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.Serialization;

public class AltWinCoins : MonoBehaviour
{

    [SerializeField] private int altCoinValue = 1;
    [SerializeField] private float altCoinRotateSpeed = 0.5f;
    [FormerlySerializedAs("altCoinFloatUpDistance")] [SerializeField] private float altCoinFloatDistance = 0.001f;
    [SerializeField] private float altCoinFloatSpeed = 1.0f;
    private GameObject player;
    private Controller2Point5D playerController;
    private bool moveUp = false;
    private bool changeDirection = true;
    
    
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
        //transform.Rotate(Vector3.up, altCoinRotateSpeed * Time.deltaTime);
        if (changeDirection)
        {
            moveUp = !moveUp;
            StartCoroutine(FloatCoolDown());
        }
        else
        {
            if (moveUp)
            {
                transform.Translate(0, altCoinFloatDistance, 0);
            }
            else
            {
                transform.Translate(0, -altCoinFloatDistance, 0);
            }
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //GameManager.AltCoinsScore += altCoinValue;
            //UIManager.Instance.AltWinCoinsUI.UpdateScoreUI();
            GameManager.Score += altCoinValue;
            UIManager.Instance.CoinsUI.UpdateCoinsSilent(GameManager.Score);
            Destroy(gameObject);
        }
    }

    private IEnumerator FloatCoolDown()
    {
        changeDirection = false;
        yield return new WaitForSeconds(altCoinFloatSpeed);
        changeDirection = true;
    }
}
