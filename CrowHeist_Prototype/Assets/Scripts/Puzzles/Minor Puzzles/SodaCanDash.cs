using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

namespace KinematicCharacterController.Examples
{
    public class SodaCanDash : MonoBehaviour
    {
        [SerializeField] private float dashForce = 10f;
        private float lastDashTime = -1f;

        public EventReference dashUse = FMODUnity.EventReference.Find("event:/SFX/PlayerMovement/Dash/Dash Use");

        private Controller2Point5D controller;

        private void Start()
        {
            // Find the player controller in the scene
            controller = FindObjectOfType<Controller2Point5D>();
        }

        public void HandleDash()
        {
            if (controller != null && controller.heldObject == this.GetComponent<Rigidbody>() && Input.GetKeyDown(KeyCode.E) && !controller.isDashing && controller.canDash)
            {
                StartCoroutine(Dash());
            }
        }

        private bool CanDash(int dashCount)
        {
            // return statement meant for limited dashes
            //return dashCount > 0 && Time.time >= lastDashTime + controller._dashCooldown;

            return Time.time >= lastDashTime + controller.dashCooldown;
        }

        private IEnumerator Dash()
        {
            lastDashTime = Time.time;
            controller.canDash = false;
            controller.isDashing = true;

            AudioManager.Instance?.PlayOneShot(dashUse);

            float dashDirection;
            Vector3 force;

            Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            if (inputDirection != Vector3.zero)
            {
                force = inputDirection * dashForce;
            }
            else
            {
                dashDirection = controller.isFacingRight ? 1f : -1f;
                force = new Vector3(dashDirection * dashForce, 0, 0);
            }

            controller.rb.AddForce(force, ForceMode.Impulse);

            float dashTime = 0f;
            while (dashTime < controller.dashDuration)
            {
                controller.rb.velocity = Vector3.Lerp(controller.rb.velocity, new Vector3(0, controller.rb.velocity.y, 0), Time.deltaTime * 2f);
                dashTime += Time.deltaTime;
                yield return null;
            }

            controller.isDashing = false;

            yield return new WaitForSeconds(controller.dashCooldown);
            controller.canDash = true;
        }
    }
}