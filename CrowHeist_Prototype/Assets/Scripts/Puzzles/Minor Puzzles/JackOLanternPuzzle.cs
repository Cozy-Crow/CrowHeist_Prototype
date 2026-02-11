using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class JackOLanternPuzzle : MonoBehaviour
{
    
    [SerializeField] private Mesh jackOLanternPuzzleMesh;
    [SerializeField] private Material jackOLanternPuzzleMaterial;
    [SerializeField] private Vector3 jackOLanternScale;
    
    private MeshRenderer pumpkinMeshRenderer;
    private MeshFilter pumpkinMeshFilter;
    
    // Start is called before the first frame update
    void Start()
    {
        pumpkinMeshRenderer = GetComponent<MeshRenderer>();
        pumpkinMeshFilter = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("'"))
        {
            ChangeToJackOLantern();
        }
    }

    // public void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log(other.gameObject.name);
    //     if (other.CompareTag("Dart"))
    //     {
    //         if (other.gameObject.CompareTag("Dart"))
    //         {
    //             Debug.Log("dart detected");
    //             ChangeToJackOLantern();
    //         }
    //         
    //     }
    // }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.CompareTag("Dart"))
        {
            Debug.Log("dart detected");
            ChangeToJackOLantern();
        }
    }



    private void ChangeToJackOLantern()
    {
        pumpkinMeshRenderer.material = jackOLanternPuzzleMaterial;
        pumpkinMeshFilter.mesh = jackOLanternPuzzleMesh;
        gameObject.transform.localScale = jackOLanternScale;
    }
}
