using UnityEngine;
using KinematicCharacterController.Examples;

public class BranchBounce : MonoBehaviour
{
    [SerializeField] private float bounceForce = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        Controller2Point5D player = other.GetComponent<Controller2Point5D>();
        if (player != null) player.ApplyBounce(bounceForce);
    }
}
