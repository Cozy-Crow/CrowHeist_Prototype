using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackOLanternPuzzle : MonoBehaviour
{
    
    [SerializeField] private Mesh jackOLanternPuzzleMesh;
    [SerializeField] private Material jackOLanternPuzzleMaterial;
    
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

    private void ChangeToJackOLantern()
    {
        pumpkinMeshRenderer.material = jackOLanternPuzzleMaterial;
        pumpkinMeshFilter.mesh = jackOLanternPuzzleMesh;
    }
}
