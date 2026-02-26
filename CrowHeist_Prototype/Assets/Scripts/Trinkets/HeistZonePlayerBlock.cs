using UnityEngine;

public class HeistZoneBlocker : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger fired by: " + other.gameObject.name + " on layer: " + other.gameObject.layer);
        
        RespawnObject respawnObject = other.GetComponentInParent<RespawnObject>();
        if (respawnObject != null)
        {
            respawnObject.Respawn();
        }
    }
}