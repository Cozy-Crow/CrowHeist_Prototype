using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DecalVisibility : MonoBehaviour
{
    [SerializeField] private List<GameObject> decalLists;

    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                ActiveDecal();
            }
        }
    }

    private void ActiveDecal()
    {
        foreach(GameObject decal in decalLists )
        {
            decal.SetActive(true);
        }
    }
}


