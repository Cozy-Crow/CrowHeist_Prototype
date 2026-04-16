using UnityEngine;
using KinematicCharacterController.Examples;

public class HeistZoneBlocker : MonoBehaviour
{
    private Controller2Point5D playerController;

    void Start()
    {
        playerController = FindObjectOfType<Controller2Point5D>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        
        RespawnObject respawnObject = other.GetComponentInParent<RespawnObject>();
        if (respawnObject != null)
        {
            Pickable pickup = respawnObject.gameObject.GetComponent<Pickable>();
            if(pickup != null && pickup.pickedUp)
            {
                playerController.Drop();
            }
            respawnObject.Respawn();
        }
    }
}
