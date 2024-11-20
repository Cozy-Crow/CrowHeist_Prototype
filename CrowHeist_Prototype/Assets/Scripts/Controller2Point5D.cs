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
        [SerializeField] private float _jumpForce = 25f;
        [SerializeField] private float _gravityMultiplier = 2f; // Extra gravity when falling add later for airtime

        [Header("PickUP")]
        [SerializeField] private Transform _pickUpPoint;
        [SerializeField] private Transform _handPoint;
        [SerializeField] private Transform _dropPoint;

        private CharacterController _characterController;
        private string _currentAnim;
        private bool _isFacingRight = true;
        private bool _isFlipped = true;

        private Vector2 _input;
        private Vector3 _direction;
        private Vector3 _velocity;            //Todo: Use to access the velocity of the character controller
        private float _dampingVelocity;
        private float _velocitY;
        private float _gravity = 10f;

        private List<IPickupable> _pickUpsList = new List<IPickupable>();
        private Equipable _equipped;

        private Animator _animator;

        #region Properties
        public bool IsGrounded => _characterController.isGrounded;
        public bool IsFlipped => _isFlipped;
        public bool IsFacingRight => _isFacingRight;
        #endregion

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            // Handle movement
            HandleAnimation();
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

            //Handles Cancel movement
            if(_input == Vector2.zero)
            {
                Vector3 moveDir = new Vector3(0, _direction.y, 0);
                _velocity = moveDir;
                _characterController.Move(_velocity * Time.deltaTime);
            }   

            //Handles Move the character and apply sprint speed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Vector3 moveDir = new Vector3(_direction.x * _sprintSpeed, _direction.y * _moveSpeed, _direction.z * _sprintSpeed);
                _velocity = moveDir;
                _characterController.Move(_velocity * Time.deltaTime);
            }
            else
            {
                Vector3 moveDir = new Vector3(_direction.x * _moveSpeed, _direction.y * _moveSpeed, _direction.z * _moveSpeed);
                _velocity = moveDir;
                _characterController.Move(_velocity * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            // Handle rotation
            if (_input.x > 0 && !_isFlipped)
            {
                _isFlipped = true;
                _isFacingRight = true;
            }
            else if (_input.x < 0 && _isFlipped)
            {
                _isFlipped = false;
                _isFacingRight = false;
            }

            Flip(_isFlipped);
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

                LayerMask interactable = LayerMask.GetMask("Interactable");
                Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2, interactable);

                foreach (Collider hitCollider in hitColliders)
                {
                    Debug.Log(hitCollider.name);

                    if (hitCollider.TryGetComponent(out Equipable equipable))
                    {
                        if (_equipped != null && equipable == _equipped)
                        {
                            _equipped.UnEquip(_dropPoint.position);
                            _equipped = null;
                        }

                        equipable.Equip(_handPoint);
                        _equipped = equipable;
                        continue;
                    }

                    if (hitCollider.TryGetComponent(out IPickupable pickUp))
                    {
                        pickUp.PickUP(_pickUpPoint);
                        _pickUpsList.Add(pickUp);
                    }
                }
            }else if (Input.GetKeyDown(KeyCode.E) && Input.GetKey(KeyCode.LeftShift) && _pickUpsList.Count > 0)
            {
                foreach (IPickupable pickUp in _pickUpsList)
                {
                    pickUp.Drop(_dropPoint.position);
                }
                _pickUpsList.Clear();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                _equipped?.Interact();
            }
        }

        private void HandleAnimation()
        {
            if(_velocitY > -1)
            {
                if (_isFacingRight)
                {
                    ChangeAnimation("JumpRight");
                }
                else
                {
                    ChangeAnimation("JumpLeft");
                }
            }

            else if(_velocity.x != 0 || _velocity.z != 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _animator.speed = 2f;
                }else
                {
                    _animator.speed = 1f;
                }

                if (_isFacingRight)
                {
                    ChangeAnimation("RunRight");
                }
                else
                {
                    ChangeAnimation("RunLeft");
                }
            }
            else if(_velocity.x == 0 && _velocity.z == 0)
            {
                if (_isFacingRight)
                {
                    ChangeAnimation("IdleRight");
                }
                else
                {
                    ChangeAnimation("IdleLeft");
                }
            }
        }

        private void ChangeAnimation(string animation, float crossfade = 0.2f)
        {
            if (_currentAnim == animation)
            {
                return;
            }

            _currentAnim = animation;
            _animator.CrossFade(animation, crossfade);
        }

        private void Flip(bool doFlip)
        {
            if (doFlip)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 5);
            }
            else if (!doFlip)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, -180, 0), Time.deltaTime * 5);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 2);
        }
    }
}