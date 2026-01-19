using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ToasterPlug : MonoBehaviour
{
    [SerializeField] GameObject connectedToaster;
    Pickable pickableComponent;

    void Awake()
    {
        if(connectedToaster == null)
        {
            Debug.LogError("ERROR: TOASTER not ln level or not conncected");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        pickableComponent = GetComponent<Pickable>();
    }

    // Update is called once per frame
    void Update()
    {
        checkInteracted();
    }

    void checkInteracted()
    {
        if(pickableComponent.pickedUp && connectedToaster != null)
        {
            connectedToaster.GetComponent<ToasterController>().pluggedIn = false;
            pickableComponent.player.Drop(); //drop item to clear buffer for player
            Destroy(gameObject); //destroy the item once it's dropped
        }
    }

}
