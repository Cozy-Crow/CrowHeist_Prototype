using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class SodaCanDash : MonoBehaviour
    {
        [SerializeField] private float dashForce = 10f;
        private float lastDashTime = -1f;

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

        private bool CanDash(int dashCount)
        {
            // return statement meant for limited dashes
            //return dashCount > 0 && Time.time >= lastDashTime + controller._dashCooldown;

            return Time.time >= lastDashTime + controller._dashCooldown;
        }

        private IEnumerator Dash()
        {
            lastDashTime = Time.time;
            controller._canDash = false;
            controller._isDashing = true;

            float dashDirection;
            Vector3 force;

            Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            if (inputDirection != Vector3.zero)
            {
                force = inputDirection * dashForce;
            }
            else
            {
                dashDirection = controller._isFacingRight ? 1f : -1f;
                force = new Vector3(dashDirection * dashForce, 0, 0);
            }

            controller._rb.AddForce(force, ForceMode.Impulse);

            float dashTime = 0f;
            while (dashTime < controller._dashDuration)
            {
                controller._rb.velocity = Vector3.Lerp(controller._rb.velocity, new Vector3(0, controller._rb.velocity.y, 0), Time.deltaTime * 2f);
                dashTime += Time.deltaTime;
                yield return null;
            }

            controller._isDashing = false;

            yield return new WaitForSeconds(controller._dashCooldown);
            controller._canDash = true;
        }
    }
}