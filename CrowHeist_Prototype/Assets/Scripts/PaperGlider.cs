using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class PaperGlider : MonoBehaviour
    {
        private Controller2Point5D controller;

        private void Start()
        {
            controller = FindObjectOfType<Controller2Point5D>();
        }

        public void HandleGliding()
        {
            ApplyGliding();
        }

        private void ApplyGliding()
        {
            if (controller._rb.velocity.y < 0)
            {
                // Reduced gravity for gliding
                float gravityForce = Physics.gravity.y * controller.gravityMultiplier * 0.1f;
                controller._rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);
                
                // Set glide fall speed
                float glideFallSpeed = -3f;
                if (controller._rb.velocity.y < glideFallSpeed)
                {
                    controller._rb.velocity = new Vector3(controller._rb.velocity.x, glideFallSpeed, controller._rb.velocity.z);
                }
            }
        }
    }
}