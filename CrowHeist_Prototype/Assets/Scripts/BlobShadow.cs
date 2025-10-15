using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BlobShadow : MonoBehaviour
{
    [SerializeField] GameObject shadow;
    public RaycastHit hit;
    [SerializeField] float offset = 0;

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //raycast below crowley to check for where to put shadow
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y - offset, transform.position.z), -Vector3.up);

        if(Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //move shadow to position
            Vector3 hitPosition = hit.point;
            shadow.transform.position = hitPosition;
        }
    }
}
