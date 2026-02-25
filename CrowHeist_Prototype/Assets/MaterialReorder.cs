using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialReorder : MonoBehaviour
{

    private void Awake()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material[] materials = renderer.materials;
        Material temp = materials[0];
        materials[0] = materials[1];
        materials[1] = temp;
        renderer.materials = materials;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
