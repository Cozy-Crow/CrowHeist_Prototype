using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class JackOLanternPuzzle : MonoBehaviour
{
    
    [SerializeField] private GameObject jackOLanternPrefab;
    [SerializeField] private GameObject[] pumpkinPieces; // 0: leftEye, 1: rightEye, 2: nose, 3: mouth, 4: lid
    [SerializeField] private GameObject goldenCoin;
    [SerializeField] private Light candleLight;
    [SerializeField] private float popForce = 5f;



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dart"))
        {
            ChangeToJackOLantern();
        }
    }



    private void ChangeToJackOLantern()
    {
        if (jackOLanternPrefab != null)
        {
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            Instantiate(jackOLanternPrefab, transform.position, rotation);
            gameObject.SetActive(false);
        }
        
        PopOutPieces();
        if (candleLight != null) candleLight.enabled = true;
    }

    private void PopOutPieces()
    {
        foreach (var piece in pumpkinPieces)
        {
            if (piece != null)
            {
                Rigidbody rb = piece.GetComponent<Rigidbody>();
                if (rb == null) rb = piece.AddComponent<Rigidbody>();
                rb.AddForce(piece.transform.forward * popForce, ForceMode.Impulse);
            }
        }
        
        if (goldenCoin != null)
        {
            Rigidbody coinRb = goldenCoin.GetComponent<Rigidbody>();
            if (coinRb == null) coinRb = goldenCoin.AddComponent<Rigidbody>();
            coinRb.AddForce(goldenCoin.transform.forward * popForce, ForceMode.Impulse);
        }
    }
}
