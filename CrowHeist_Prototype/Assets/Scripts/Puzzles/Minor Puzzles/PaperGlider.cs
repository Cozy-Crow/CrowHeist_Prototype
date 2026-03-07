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
            if (controller.rb.velocity.y < 0)
            {
                // Reduced gravity for gliding
                float gravityForce = Physics.gravity.y * controller.gravityMultiplier * 0.1f;
                controller.rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);
                
                // Set glide fall speed
                float glideFallSpeed = -1f;
                if (controller.rb.velocity.y < glideFallSpeed)
                {
                    controller.rb.velocity = new Vector3(controller.rb.velocity.x, glideFallSpeed, controller.rb.velocity.z);
                }
            }
        }
    }
}