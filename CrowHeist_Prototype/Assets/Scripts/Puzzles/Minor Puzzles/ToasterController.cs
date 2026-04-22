using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using FMODUnity;
using Unity.VisualScripting;

public class ToasterController : MonoBehaviour
{
    [SerializeField] GameObject paintTubePrefab;
    [SerializeField] Transform paintTubeSpawnPoint;
    public bool pluggedIn = true;
    [SerializeField] GameObject interactionTrigger;
    [SerializeField] GameObject plugReference;
    [SerializeField] private EventReference coinSparkle;

    // vfx stuff
    [SerializeField] GameObject toasterExplosion;
    void Awake()
    {
        if(plugReference == null)
        {
            // Debug.LogError("ERROR: PLUG not in level or not conncected");
            pluggedIn = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkPlugged();
    }

    void checkPlugged()
    {
        if(pluggedIn)
        {
            //jank way to do it but if its plugged in dont allow it to be picked up
            // GetComponent<Pickable>().enabled = false;
            interactionTrigger.SetActive(false);
        }
        else
        {
            interactionTrigger.SetActive(true);
            // GetComponent<Pickable>().enabled = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals("Wall"))
        {
            Instantiate(paintTubePrefab, paintTubeSpawnPoint.position, paintTubeSpawnPoint.rotation);
            AudioManager.Instance?.PlayOneShot3D(coinSparkle, transform.position);
            toasterExplosion.transform.position = transform.position;
            ParticleSystem[] particleSystems = toasterExplosion.GetComponentsInChildren<ParticleSystem>(true);
            
            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Play();
            }
            gameObject.SetActive(false);

        }

        // if(GetComponent<Pickable>().pickedUp)
        // {
        //     if (paintTubePrefab != null && paintTubeSpawnPoint != null)
        //     {
                
                // Instantiate(
                //     paintTubePrefab,
                //     paintTubeSpawnPoint.position,
                //     paintTubeSpawnPoint.rotation
                // );
        //     }
        // }
    }

   


}
