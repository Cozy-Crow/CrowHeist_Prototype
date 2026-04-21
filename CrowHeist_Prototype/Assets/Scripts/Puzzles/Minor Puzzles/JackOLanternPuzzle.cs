using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

public class JackOLanternPuzzle : MonoBehaviour
{
    
    [SerializeField] private GameObject jackOLanternPrefab;
    [SerializeField] private GameObject[] pumpkinPiecePrefabs; // 0: leftEye, 1: rightEye, 2: nose, 3: mouth, 4: lid
    [SerializeField] private GameObject goldenCoinPrefab;
    public GameObject confettiEffect;
    [SerializeField] private float popForce = 2f;
    [SerializeField] private Vector3 spawnOffset = new Vector3(0, -0.5f, 0);
    [SerializeField] private EventReference coinSparkleSFX;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dart"))
        {
            ChangeToJackOLantern();
        }
    }



    private void ChangeToJackOLantern()
    {
        PopOutPieces();
        
        if (jackOLanternPrefab != null)
        {
            Quaternion rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
            Instantiate(jackOLanternPrefab, transform.position + spawnOffset, rotation);
        }
        
        gameObject.SetActive(false);
    }

    private void PopOutPieces()
    {
        List<GameObject> spawnedObjects = new List<GameObject>();
        
        foreach (var piecePrefab in pumpkinPiecePrefabs)
        {
            if (piecePrefab != null)
            {
                Quaternion rotation = transform.rotation * Quaternion.Euler(0, 180, 0);
                GameObject piece = Instantiate(piecePrefab, transform.position, rotation);
                spawnedObjects.Add(piece);
            }
        }
        
        if (goldenCoinPrefab != null)
        {
            GameObject coin = Instantiate(goldenCoinPrefab, transform.position, transform.rotation);
            spawnedObjects.Add(coin);
        }
        
        foreach (var obj in spawnedObjects)
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) rb = obj.AddComponent<Rigidbody>();
            rb.AddForce(obj.transform.forward * popForce, ForceMode.Impulse);
        }

        AudioManager.Instance?.PlayOneShot3D(coinSparkleSFX, transform.position);

        if (confettiEffect)
        {
            Debug.Log(confettiEffect.GetComponent<ParticleSystem>());
            confettiEffect.GetComponent<ParticleSystem>().time = 0f;
            Instantiate(confettiEffect, transform.position, Quaternion.identity);
            confettiEffect.GetComponent<ParticleSystem>().Play();
        }
    }
}
