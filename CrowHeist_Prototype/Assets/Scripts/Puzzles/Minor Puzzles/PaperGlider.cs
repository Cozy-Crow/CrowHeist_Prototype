using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class PaperGlider : MonoBehaviour
    {
        private Controller2Point5D controller;
        private Rigidbody rb;
        private Pickable pickable;
        private Collider col;

        [Header("Paper Fall Settings")]
        [SerializeField] private float driftForce = 0.4f;
        [SerializeField] private float driftFrequency = 0.65f;
        [SerializeField] private float maxFallSpeed = 2f;
        [SerializeField] private float tiltAmount = 25f;
        [SerializeField] private float settleTime = 3f;
        [SerializeField] private float settleRotationSpeed = 2f;

        private float driftTimer;
        private float settleTimer;
        private int paperLayer;
        private bool isColliding;

        private void Start()
        {
            controller = FindObjectOfType<Controller2Point5D>();
            rb = GetComponent<Rigidbody>();
            pickable = GetComponent<Pickable>();
            col = GetComponent<Collider>();
            paperLayer = ~(1 << gameObject.layer);
        }

        private bool IsGrounded()
        {
            float rayLength = col.bounds.extents.y + 0.05f;
            return Physics.Raycast(transform.position, Vector3.down, rayLength, paperLayer, QueryTriggerInteraction.Ignore);
        }

        private void FixedUpdate()
        {
            bool isHeld = pickable != null && pickable.pickedUp;
            bool isFalling = rb.velocity.y < -0.1f;
            bool isGrounded = IsGrounded();

            if (isHeld || isGrounded || isColliding)
            {
                settleTimer = 0f;
                driftTimer = 0f;
                rb.angularVelocity = Vector3.zero;
                return;
            }

            if (!isFalling)
            {
                driftTimer = 0f;
                return;
            }

            if (settleTimer > 0.3f)
                rb.angularVelocity = Vector3.zero;

            if (rb.velocity.y < -maxFallSpeed)
                rb.velocity = new Vector3(rb.velocity.x, -maxFallSpeed, rb.velocity.z);

            settleTimer += Time.fixedDeltaTime;
            float settleT = Mathf.Clamp01(settleTimer / settleTime);

            driftTimer += Time.fixedDeltaTime * driftFrequency;
            float drift = Mathf.Sin(driftTimer) * driftForce * settleT;
            rb.AddForce(new Vector3(drift, 0f, drift), ForceMode.Acceleration);

            float tiltAngle = Mathf.Sin(driftTimer) * tiltAmount * settleT;
            Quaternion targetRot = Quaternion.Euler(0f, rb.rotation.eulerAngles.y, tiltAngle);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, settleRotationSpeed * Time.fixedDeltaTime * 60f));
        }

        private void OnCollisionStay(Collision _) => isColliding = true;
        private void OnCollisionExit(Collision _) => isColliding = false;

        public void HandleGliding()
        {
            ApplyGliding();
        }

        private void ApplyGliding()
        {
            if (controller.rb.velocity.y < 0)
            {
                float gravityForce = Physics.gravity.y * controller.gravityMultiplier * 0.1f;
                controller.rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);

                float glideFallSpeed = -1f;
                if (controller.rb.velocity.y < glideFallSpeed)
                    controller.rb.velocity = new Vector3(controller.rb.velocity.x, glideFallSpeed, controller.rb.velocity.z);
            }
        }
    }
}
