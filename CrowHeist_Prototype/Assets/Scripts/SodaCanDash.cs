using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class SodaCanDash : MonoBehaviour
    {

        private Controller2Point5D controller;

        private void Start()
        {
            // Find the player controller in the scene
            controller = FindObjectOfType<Controller2Point5D>();
        }

        public void HandleDash()
        {
            if (controller != null && controller.heldObject == this.GetComponent<Rigidbody>() && Input.GetKeyDown(KeyCode.E) && !controller._isDashing && controller._canDash)
            {
                StartCoroutine(Dash());
            }
        }

        private IEnumerator Dash()
        {
            controller._canDash = false;
            controller._isDashing = true;

            float dashDirection;
            Vector3 dashVelocity;

            Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            if (inputDirection != Vector3.zero)
            {
                dashVelocity = inputDirection * controller._dashSpeed;
            }
            else
            {
                dashDirection = controller._isFacingRight ? 1f : -1f;
                dashVelocity = new Vector3(dashDirection * controller._dashSpeed, 0, 0);
            }

            float dashTime = 0f;
            while (dashTime < controller._dashDuration && controller._isDashing)
            {
                controller._rb.velocity = new Vector3(dashVelocity.x, controller._rb.velocity.y, dashVelocity.z);
                dashTime += Time.deltaTime;
                yield return null;
            }

            controller._isDashing = false;
            yield return new WaitForSeconds(controller._dashCooldown);
            controller._canDash = true;
        }
    }
}