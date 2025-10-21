using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintingPuzzle : MonoBehaviour
{
    [SerializeField] private Material paintMaterial;
    [SerializeField] private float lineWidth;
    [SerializeField] private float xOffset;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject coinSpawnPoint;
    [SerializeField] private GameObject coinPrefab;



    private LineRenderer currentLine;
    private List<Vector3> currentPoints = new List<Vector3>();
    private int index;
    private Bounds myBounds = new Bounds();
    private Vector3 paintingWidth;
    private Vector3 paintingHeight;
    private Vector3 coinSpawnPosition;
    private bool hasCompletedPuzzle = false;
    
    // Start is called before the first frame update
    void Start()
    {
        myBounds = GetComponent<Renderer>().bounds;
        paintingWidth = myBounds.max - myBounds.min;
        paintingHeight = myBounds.max - myBounds.min;
        player = GameObject.FindWithTag("Player");
        coinSpawnPosition = coinSpawnPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Paintbrush"))
        {
            //Vector3 point = new Vector3(transform.position.x + xOffset, other.contacts[0].point.y, other.contacts[0].point.z);
            Vector3 point = other.contacts[0].point;
            foreach (ContactPoint contact in other.contacts)
            {
                if (Vector3.Distance(player.transform.position, contact.point) < Vector3.Distance(player.transform.position, point))
                {
                    point = contact.point;
                }
            }
            
            point.x += xOffset;
            
            if (myBounds.Contains(point))
            {
                // if (Input.GetMouseButton(1))
                // {
                //     if
                //         (currentLine == null)
                //     {
                //         index = 0;
                //         currentLine = new GameObject().AddComponent<LineRenderer>();
                //         currentLine.alignment = LineAlignment.TransformZ;
                //         currentLine.startWidth = lineWidth;
                //         currentLine.endWidth = lineWidth;
                //         //currentLine.material = paintMaterial;   
                //         currentLine.positionCount = 1;
                //         currentLine.SetPosition(0, point);
                //     }
                //     else
                //     {
                //         Vector3 currentPoint = currentLine.GetPosition(index);
                //         if (Vector3.Distance(currentPoint, other.contacts[0].point) > 0.1f)
                //         {
                //             index++;
                //             currentLine.positionCount = index + 1;
                //             currentLine.alignment = LineAlignment.TransformZ;
                //             currentLine.SetPosition(index, point);
                //         }
                //     }
                // }
                // else if (currentLine != null)
                // {
                //     currentLine = null;
                // }
            }
            
            if (Input.GetMouseButton(1))
            {
                // if
                //     (currentLine == null)
                // {
                //     index = 0;
                //     currentLine = new GameObject().AddComponent<LineRenderer>();
                //     currentLine.alignment = LineAlignment.TransformZ;
                //     currentLine.startWidth = lineWidth;
                //     currentLine.endWidth = lineWidth;
                //     //currentLine.material = paintMaterial;   
                //     currentLine.positionCount = 1;
                //     currentLine.SetPosition(0, point);
                // }
                // else
                // {
                //     Vector3 currentPoint = currentLine.GetPosition(index);
                //     if (Vector3.Distance(currentPoint, other.contacts[0].point) > 0.1f)
                //     {
                //         index++;
                //         currentLine.positionCount = index + 1;
                //         currentLine.alignment = LineAlignment.TransformZ;
                //         currentLine.SetPosition(index, point);
                //     }
                // }
            }
            
            if
                (currentLine == null)
            {
                index = 0;
                currentLine = new GameObject().AddComponent<LineRenderer>();
                //currentLine.alignment = LineAlignment.TransformZ;
                //currentLine.alignment = LineAlignment.View;
                currentLine.useWorldSpace = true;
                currentLine.startWidth = lineWidth;
                currentLine.endWidth = lineWidth;
                currentLine.material = paintMaterial;   
                currentLine.positionCount = 1;
                currentLine.SetPosition(0, point);
            }
            else
            {
                Vector3 currentPoint = currentLine.GetPosition(index);
                if (Vector3.Distance(currentPoint, other.contacts[0].point) > 0.1f)
                {
                    index++;
                    currentLine.positionCount = index + 1;
                    //currentLine.alignment = LineAlignment.TransformZ;
                    currentLine.SetPosition(index, point);
                }
            }
            

            
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Paintbrush"))
        {
            if (currentLine != null)
            {
                currentLine = null;
            }

            if (!hasCompletedPuzzle)
            {
                SpawnCoin();
                hasCompletedPuzzle = true;
            }
        }
    }

    private void SpawnCoin()
    {
        Instantiate(coinPrefab, coinSpawnPosition, Quaternion.identity);
    }
}
