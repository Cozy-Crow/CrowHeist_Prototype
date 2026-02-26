using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeistZonePlayerBlock : MonoBehaviour
{
    [SerializeField]private Collider zoneCollider;
    // Start is called before the first frame update
    void Start()
    {
        zoneCollider = GetComponent<Collider>();
    }

    void OnCollisionStay(Collision collision)
    {
        // If it's NOT the player, ignore the collision
        if (!collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(collision.collider, zoneCollider);
        }
    }

}
