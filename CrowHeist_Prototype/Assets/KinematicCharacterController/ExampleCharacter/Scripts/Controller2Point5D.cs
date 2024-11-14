using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    [RequireComponent(typeof(CharacterController))]
    public class Controller2Point5D : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _sprintSpeed = 10f;
        [SerializeField] private float _smoothTime = 0.05f;
        [SerializeField] private float _jumpForce = 5f;
        [SerializeField] private float _gravityMultiplier = 2f; // Extra gravity when falling add later for airtime

        [Header("PickUP")]
        [SerializeField] private Transform _pickUpPoint;
        [SerializeField] private Transform _dropPoint;

        private CharacterController _characterController;
        private Vector2 _input;
        private Vector3 _direction;
        //private Vector3 _velocity;            //Todo: Use to access the velocity of the character controller
        private float _dampingVelocity;
        private float _velocitY;
        private float _gravity = 10F;

        private List<IPickupable> _pickUpsList = new List<IPickupable>();

        public bool IsGrounded => _characterController.isGrounded;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            // Handle movement
            HandlePickUP();
            HandleGravity();
            HandleMove();
            HandleRotation();
        }

        private void HandleMove()
        {
            // Get horizontal input (e.g., A/D keys or arrow keys)
            _input = new Vector2(Input.GetAxis("Horizontal") ,Input.GetAxis("Vertical"));
            _direction = new Vector3(_input.x, _direction.y, _input.y);

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                Jump();
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 moveDir = new Vector3(_direction.x * _sprintSpeed, _direction.y * _moveSpeed, _direction.z * _sprintSpeed);
                _characterController.Move(moveDir * Time.deltaTime);
            }
            else
            {
                Vector3 moveDir = new Vector3(_direction.x * _moveSpeed, _direction.y * _moveSpeed, _direction.z * _moveSpeed);
                _characterController.Move(moveDir * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if(_input != Vector2.zero)
            {
                // Rotate the character to face the direction of movement
                float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _dampingVelocity, _smoothTime);
                transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }

        private void HandleGravity()
        {
            if (IsGrounded && _velocitY < 0)
            {
                // Reset the y velocity if grounded
                _velocitY = -1f;
            }
            //else if (!IsGrounded && _velocitY < 0)
            //{
            //    // Apply extra gravity when not grounded and falling
            //    _velocitY -= _gravity * _gravityMultiplier * Time.deltaTime;
            //}
            else
            {
                // Apply gravity to the CharacterController
                _velocitY -= _gravity * Time.deltaTime;
            }

            _direction.y = _velocitY;
        }

        private void Jump()
        {
            if (!IsGrounded)
            {
                return;
            }

            // Apply an upward force to the Rigidbody for jumping
            _velocitY += _jumpForce;
        }

        private void HandlePickUP()
        {
            if (Input.GetKeyDown(KeyCode.E) && _pickUpsList.Count <= 0)
            {

                LayerMask pickupMask = LayerMask.GetMask("PickUp");
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2, pickupMask);

                foreach (Collider hitCollider in hitColliders)
                {
                    Debug.Log(hitCollider.name);
                    if (hitCollider.TryGetComponent(out IPickupable pickUp))
                    {
                        pickUp.PickUP(_pickUpPoint);
                        _pickUpsList.Add(pickUp);
                    }
                }
            }else if (Input.GetKeyDown(KeyCode.E) && _pickUpsList.Count > 0)
            {
                foreach (IPickupable pickUp in _pickUpsList)
                {
                    pickUp.Drop(_dropPoint.position);
                }
                _pickUpsList.Clear();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
    }
}