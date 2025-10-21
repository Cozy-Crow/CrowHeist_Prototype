using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Paintbrush : MonoBehaviour
{
    
    private Camera mainCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if (Input.GetMouseButton(1))
        {
            // Ray ray = new Ray(mainCamera.transform.position, mainCamera.ScreenToWorldPoint(Input.mousePosition));
            // RaycastHit hit;
            //
            // if (Physics.Raycast(ray, out hit))
            // {
            //     Vector3 aimPoint = hit.point;
            //     transform.rotation = Quaternion.LookRotation(aimPoint - mainCamera.transform.position);
            // }
            
            //transform.rotation = Quaternion.LookRotation(mainCamera.ScreenToWorldPoint(Input.mousePosition));

            
        }
    }
    
    
    
    
}
