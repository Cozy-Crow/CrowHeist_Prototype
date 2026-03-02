using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class BreakablePiecesSFX : MonoBehaviour
{

    [SerializeField] private EventReference piecesSFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("BreakVase"))
        {
         AudioManager.Instance?.PlayOneShot3D(piecesSFX, transform.localPosition); 
        }
    }
}
