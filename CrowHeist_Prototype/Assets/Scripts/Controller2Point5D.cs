using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    [RequireComponent(typeof(CharacterController))]
    public class Controller2Point5D : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 50f;
        //[SerializeField] private float _sprintSpeed = 10f;
        [SerializeField] private float _smoothTime = 0.05f;
        [SerializeField] private float _jumpForce = 40f;
        [SerializeField] private float _gravityMultiplier = 2f; // Extra gravity when falling add later for airtime


        //Falling
        public float _fallingTime = 0f;


        [Header("PickUP")]
        [SerializeField] private Transform _pickUpPoint;
        [SerializeField] private Transform _handPoint;
        [SerializeField] private Transform _dropPoint;


        [Header("Dash")]
        [SerializeField] private float _dashSpeed = 40f;
        [SerializeField] private float _dashDuration = 0.2f;
        [SerializeField] private float _dashForce = 10f;
        public float _dashCooldown = 1f;
        private bool _canDash = true;
        public bool _isDashing = false;


        private CharacterController _characterController;
        private string _currentAnim;
        private bool _isFacingRight = true;
        private bool _isMovingForward = false;
        private bool _isMovingBackward = false;
        private bool _isFlipped = true;
        private bool _isThrowing = false;
        private bool _canJump = true;


        private Vector2 _input;
        private Vector3 _direction;
        private Vector3 _velocity;
        private float _velocitY;
        private float _gravity = 7f;

        private List<IPickupable> _pickUpsList = new List<IPickupable>();
        private Equipable _equipped;

        private Animator _animator;

        public Transform throwPoint;
        public float maxThrowForce = 50f;
        public float chargeTime = 2f;
        private float throwForce = 0f;
        private bool isCharging = false;
        private bool isCanceled = false;
        private float chargeStartTime;
        private LineRenderer lineRenderer;  // LineRenderer to draw trajectory
        private Rigidbody heldObject;
        private Vector3 storedThrowDirection = Vector3.zero;


        private GameObject _touchingObject;
        private GameObject _currentGroundObject;
        private bool isWindingUp = false;
        private float windUpTime = 1f; // Time player needs to hold 'F'
        private float windUpTimer = 0f;
        private bool isTimerActive = false;
        public float bounceDelay = 2f; // Delay before bounce is applied
        private float bounceTimer = 0f;
        private bool canBounce = false;
        private bool isInTrigger = false;
        public GameObject jack;


        #region Properties
        public bool IsGrounded => _characterController.isGrounded;
        public bool IsFlipped => _isFlipped;
        public bool IsFacingRight => _isFacingRight;
        public string Equipped
        {
            get
            {
                if (_equipped != null)
                {
                    return _equipped.name;
                }
                return "null";
            }
        }
        #endregion



        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();

        }
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0; // Initial no line
        }

        void Update()
        {
            // Handle movement
            HandleDash();
            HandleAnimation();
            HandlePickUP();
            HandleGravity();
            HandleMove();
            HandleRotation();
            HandleWindUp();
            HandleBounce();
        }


        //private void HandleMove()
        //{
        //    // Get horizontal input (e.g., A/D keys or arrow keys)
        //    _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        //    _direction = new Vector3(_input.x, _direction.y, _input.y);

        //    // Jump automatically if the player is grounded and still holding jump, but only if they are allowed to jump
        //    if (IsGrounded && Input.GetButton("Jump") && _canJump)
        //    {
        //        Jump();
        //        _canJump = false; // Prevent instant repeated jumps
        //        StartCoroutine(JumpCooldown()); // Start cooldown
        //    }

        //    // Handles Cancel movement
        //    if (_input == Vector2.zero)
        //    {
        //        Vector3 moveDir = new Vector3(0, _direction.y, 0);
        //        _velocity = moveDir;
        //        _characterController.Move(_velocity * Time.deltaTime);
        //    }

        //    // Handles Move the character and apply sprint speed
        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        Vector3 moveDir = new Vector3(_direction.x * _sprintSpeed, _direction.y * _moveSpeed, _direction.z * _sprintSpeed);
        //        _velocity = moveDir;
        //        _characterController.Move(_velocity * Time.deltaTime);
        //    }
        //    else
        //    {
        //        Vector3 moveDir = new Vector3(_direction.x * _moveSpeed, _direction.y * _moveSpeed, _direction.z * _moveSpeed);
        //        _velocity = moveDir;
        //        _characterController.Move(_velocity * Time.deltaTime);
        //    }
        //}
        private void HandleMove()
        {
            if (_isDashing) return; // Don't allow movement input during dash

            _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _direction = new Vector3(_input.x, _direction.y, _input.y);

            if (IsGrounded && Input.GetButton("Jump") && _canJump)
            {
                Jump();
                _canJump = false;
                StartCoroutine(JumpCooldown());
            }
            Vector3 moveDir = new Vector3(_direction.x * _moveSpeed, _direction.y * _moveSpeed, _direction.z * _moveSpeed);
            _velocity = moveDir;
            _characterController.Move(_velocity * Time.deltaTime);
            
        }


        private IEnumerator JumpCooldown()
        {
            yield return new WaitForSeconds(0.1f); // Small delay before allowing another jump
            _canJump = true;
        }

        private void HandleDash()
        {
            if (_canDash && Input.GetKeyDown(KeyCode.LeftShift))
            {
                StartCoroutine(Dash());
            }
        }


        private IEnumerator Dash()
        {
            if (heldObject == null || !heldObject.CompareTag("Dashable"))
            {
                yield break; // Exit the coroutine if the object is not dashable
            }

            _canDash = false;
            _isDashing = true;
            float dashDirection;
            Vector3 dashVelocity;

            // Get dash direction (only horizontal movement)
            if (_isMovingForward || _isMovingBackward)
            {
                dashDirection = _isMovingForward ? 1f : -1f;
                dashVelocity = new Vector3(0, 0, dashDirection * _dashSpeed);
            }
            else
            {
                dashDirection = _isFacingRight ? 1f : -1f;
                dashVelocity = new Vector3(dashDirection * _dashSpeed, 0, 0);
            }
            

            float dashTime = 0f;
            while (dashTime < _dashDuration && _isDashing)
            {
                _characterController.Move(dashVelocity * Time.deltaTime);
                dashTime += Time.deltaTime;
                yield return null;
            }

            _isDashing = false;
            yield return new WaitForSeconds(_dashCooldown);
            _canDash = true;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (_isDashing) // Only apply force if dashing
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null && !rb.isKinematic)
                {
                    Vector3 forceDirection = hit.point - transform.position;
                    forceDirection = forceDirection.normalized;

                    float forceAmount = 20f; // Adjust this value
                    rb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);

                    _isDashing = false;
                }
            }
            // Check if the player is standing on something
            if (Vector3.Dot(hit.normal, Vector3.up) > 0.5f) // Ensures it's a mostly horizontal surface
            {
                _currentGroundObject = hit.gameObject;
            }
        }
    



    private void HandleRotation()
        {
            _isMovingForward = (_input.y > 0);
            _isMovingBackward = (_input.y < 0);
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

        //private void HandleGravity()
        //{
        //    if (IsGrounded && _velocitY < 0)
        //    {
        //        // Reset the y velocity if grounded
        //        _velocitY = -1f;
        //    }
        //    //else if (!IsGrounded && _velocitY < 0)
        //    //{
        //    //    // Apply extra gravity when not grounded and falling
        //    //    _velocitY -= _gravity * _gravityMultiplier * Time.deltaTime;
        //    //}
        //    else
        //    {
        //        // Apply gravity to the CharacterController
        //        _velocitY -= _gravity * Time.deltaTime;
        //    }

        //    _direction.y = _velocitY;
        //}
        private void HandleGravity()
        {
            if (IsGrounded && _velocitY < 0)
            {
                // Keep character slightly grounded
                _velocitY = -2f;
                _fallingTime = 0f;
            }
            else
            {
                if (heldObject != null && heldObject.CompareTag("Glider") && _velocitY < 0)
                {
                    // Apply a controlled glide by setting a max fall speed
                    float glideFallSpeed = -3f; // Adjust this value for a smoother glide
                    _velocitY = Mathf.Max(_velocitY - (_gravity * 0.1f * Time.deltaTime), glideFallSpeed);
                    Debug.Log("Gliding");
                }
                else
                {
                    // Apply normal gravity if not gliding
                    _velocitY -= _gravity * Time.deltaTime;
                }
            }

            // Clamp to prevent extreme fall speeds
            _velocitY = Mathf.Clamp(_velocitY, -20f, float.MaxValue);

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
        void HandleWindUp()
        {
            if (IsGrounded && _touchingObject != null && _touchingObject.CompareTag("JackInTheBox"))
            {
                if (Input.GetKey(KeyCode.F))
                {
                    windUpTimer += Time.deltaTime;
                    Debug.Log("Winding up: " + windUpTimer);

                    if (windUpTimer >= windUpTime)
                    {
                        jack.SetActive(false);
                        isTimerActive = true; // Start bounce delay timer
                        windUpTimer = 0f;
                        Debug.Log("Jack-in-the-Box wound up! Waiting for launch...");
                    }
                }
                else
                {
                    windUpTimer = 0f; // Reset if F is released
                }
            }

            if (isTimerActive)
            {
                bounceTimer += Time.deltaTime;
                if (bounceTimer >= bounceDelay)
                {
                    canBounce = true;
                    isTimerActive = false;
                    bounceTimer = 0f;
                }
            }
        }


        void HandleBounce()
        {
            if (canBounce && IsGrounded && _currentGroundObject != null && _currentGroundObject.CompareTag("JackInTheBox"))
            {
                jack.gameObject.SetActive(true);
                ApplyBounce(5f); // Change 10f to your desired bounce strength
                canBounce = false;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("JackInTheBox"))
            {
                isInTrigger = true;
                _touchingObject = other.gameObject;
                Debug.Log("Entered Jack In The Box trigger.");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("JackInTheBox"))
            {
                isInTrigger = false;
                Debug.Log("Exited Jack In The Box trigger.");
            }
        }


        public void ApplyBounce(float bounceStrength)
        {
            _velocitY = bounceStrength;
            Debug.Log("BOING! Bounce applied: " + bounceStrength);

        }


        private void HandlePickUP()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                LayerMask interactable = LayerMask.GetMask("Interactable");
                Collider[] interactableColliders = Physics.OverlapSphere(transform.position, 2, interactable);

                if (interactableColliders.Length > 0)
                {
                    Collider equipCollider = interactableColliders[0];

                    if (equipCollider.TryGetComponent(out Equipable equipable))
                    {
                        if (_equipped != null)
                        {
                            _equipped.UnEquip(_dropPoint.position);
                        }

                        equipable.Equip(_handPoint);
                        _equipped = equipable;
                    }

                    foreach (Collider hitCollider in interactableColliders)
                    {
                        if (hitCollider.TryGetComponent(out IPickupable pickUp))
                        {
                            pickUp.PickUP(_pickUpPoint);
                            _pickUpsList.Add(pickUp);
                            heldObject = hitCollider.GetComponent<Rigidbody>(); // Store held object
                        }
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                Rigidbody rigidbody = heldObject.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;

                    // Fixed velocity similar to charged throw
                    float fixedThrowForce = maxThrowForce * 0.7f; // Adjust as needed

                    // Convert 20 degrees to a direction vector
                    float angle = 20f * Mathf.Deg2Rad;
                    Vector3 throwDirection;

                    if (_isMovingForward || _isMovingBackward)
                    {
                        // Ensure the throw is always in the player's forward direction
                        throwDirection = new Vector3(0, Mathf.Tan(angle), (_isMovingForward ? 1 : -1)).normalized;
                    }
                    else
                    {
                        // Throw left or right based on facing direction
                        throwDirection = new Vector3((_isFacingRight ? 1 : -1), Mathf.Tan(angle), 0).normalized;
                    }

                    // Apply force
                    rigidbody.AddForce(throwDirection * fixedThrowForce, ForceMode.Impulse);

                    // If the object is a knife, set its spin speed
                    KnifeStick knife = heldObject.GetComponent<KnifeStick>();
                    if (knife != null && _isMovingForward)
                    {
                        heldObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                        //float spinSpeed = throwForce * 50f; // Adjust multiplier for desired effect
                        //knife.SetRotationSpeed(spinSpeed);
                    }
                }

                foreach (IPickupable pickUp in _pickUpsList)
                {
                    pickUp.Drop(_dropPoint.position);
                }
                _pickUpsList.Clear();
                heldObject = null;
            }



            // Charged Throwing mechanism
            //if (heldObject != null)
            //{
            //    if (Input.GetKeyDown(KeyCode.G))
            //    {
            //        isCharging = true;
            //        chargeStartTime = Time.time;
            //    }

            //    if (Input.GetKey(KeyCode.G))
            //    {
            //        throwForce = Mathf.Clamp((Time.time - chargeStartTime) / chargeTime * maxThrowForce, 0, maxThrowForce);
            //        DrawThrowTrajectory();  // Draw the trajectory while charging
            //    }

            //    if (Input.GetKeyUp(KeyCode.G))
            //    {
            //        isCharging = false;

            //        // Throw object
            //        Rigidbody rigidbody = heldObject.GetComponent<Rigidbody>();
            //        if (rigidbody != null)
            //        {
            //            rigidbody.isKinematic = false;
            //            Vector3 throwDirection = new Vector3(_isFacingRight ? 1 : -1, 1, 0);
            //            rigidbody.AddForce(throwDirection * throwForce, ForceMode.Impulse);

            //            // If the object is a knife, set its spin speed
            //            KnifeStick knife = heldObject.GetComponent<KnifeStick>();
            //            if (knife != null)
            //            {
            //                float spinSpeed = throwForce * 50f; // Adjust multiplier for desired effect
            //                knife.SetRotationSpeed(spinSpeed);
            //            }
            //        }

            //        foreach (IPickupable pickUp in _pickUpsList)
            //        {
            //            pickUp.Drop(_dropPoint.position);
            //        }
            //        _pickUpsList.Clear();
            //        heldObject = null;
            //        throwForce = 0f;

            //        // Clear the line renderer after throw
            //        lineRenderer.positionCount = 0;
            //    }
            //}



            // Charged Throwing mechanism
            if (heldObject != null)
            {

                // Start charging when left mouse button is pressed
                if (Input.GetMouseButtonDown(0)) // Left mouse button
                {
                    isCharging = true;
                    isCanceled = false;
                    chargeStartTime = Time.time;
                }

                // While holding the left mouse button, update throw force and aim direction
                
                if (Input.GetMouseButton(0) && !isCanceled)
                {
                    throwForce = Mathf.Clamp((Time.time - chargeStartTime) / chargeTime * maxThrowForce, 0, maxThrowForce);

                    Vector3 mousePosition = Input.mousePosition;
                    mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z + 5f;
                    Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

                    Vector3 playerPosition = transform.position;
                    storedThrowDirection = (worldMousePos - playerPosition).normalized; // Always assign value

                    DrawThrowTrajectory(storedThrowDirection);
                }

                // Release the left mouse button to throw
                if (Input.GetMouseButtonUp(0) && !isCanceled)
                {
                    isCharging = false;

                    // Throw object
                    Rigidbody rigidbody = heldObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.isKinematic = false;

                        // Final throw direction calculation (same as above)
                        Vector3 mousePosition = Input.mousePosition;
                        mousePosition.z = 10f; // Adjust based on your setup
                        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

                        Vector3 playerPosition = transform.position;  // Assuming this is the player's position
                        Vector3 throwDirection = (worldMousePos - playerPosition);
                        throwDirection.y = Mathf.Clamp(throwDirection.y, -0.2f, 0.2f); // Limit vertical influence
                        throwDirection = throwDirection.normalized;


                        rigidbody.AddForce(storedThrowDirection * throwForce, ForceMode.Impulse); // Use stored direction

                        // If the object is a knife, set its spin speed
                        KnifeStick knife = heldObject.GetComponent<KnifeStick>();
                        if (knife != null && _isMovingForward)
                        {
                            heldObject.transform.rotation = Quaternion.Euler(90, 0, 0);
                            //float spinSpeed = throwForce * 50f; // Adjust multiplier for desired effect
                            //knife.SetRotationSpeed(spinSpeed);
                        }
                    }

                    // Drop the held object
                    foreach (IPickupable pickUp in _pickUpsList)
                    {
                        pickUp.Drop(_dropPoint.position);
                    }
                    _pickUpsList.Clear();
                    heldObject = null;
                    throwForce = 0f;

                    // Clear the trajectory visualization
                    lineRenderer.positionCount = 0;
                }

                // Right-click to cancel the throw
                if (Input.GetMouseButtonDown(1)) // Right mouse button
                {
                    isCharging = false;
                    isCanceled = true;
                    throwForce = 0f;
                    lineRenderer.positionCount = 0; // Clear trajectory visualization

                    // Ensure the object remains held but isn't thrown
                    storedThrowDirection = Vector3.zero; // Reset the throw direction
                }
            }



            if (Input.GetKeyDown(KeyCode.F))
            {
                _equipped?.Interact();
            }
        }

        //void DrawThrowTrajectory()
        //{
        //    if (heldObject == null) return;

        //    // Set the number of points in the trajectory
        //    int trajectoryPoints = 30;
        //    lineRenderer.positionCount = trajectoryPoints;

        //    // Initial position of the throw (held object's current position)
        //    Vector3 startPos = heldObject.transform.position;

        //    // Initial velocity based on the charge and direction
        //    Vector3 throwDirection = new Vector3(_isFacingRight ? 1 : -1, 1, 0);
        //    Vector3 velocity = throwDirection.normalized * throwForce;

        //    // Calculate the trajectory
        //    for (int i = 0; i < trajectoryPoints; i++)
        //    {
        //        float t = i * 0.1f;  // Time increment for each trajectory point
        //        Vector3 point = startPos + velocity * t + 0.5f * Physics.gravity * t * t;  // Standard projectile motion equation

        //        lineRenderer.SetPosition(i, point);  // Set the position in the LineRenderer
        //    }
        //}

        void DrawThrowTrajectory(Vector3 direction)
        {
            int resolution = 20; // More points = smoother curve
            float timeStep = 0.1f; // Time increment per point
            Vector3 startPosition = transform.position;
            Vector3 velocity = direction * throwForce; // Use the same throw force

            lineRenderer.positionCount = resolution;

            for (int i = 0; i < resolution; i++)
            {
                float time = i * timeStep;
                Vector3 point = startPosition + velocity * time + 0.5f * Physics.gravity * time * time;
                lineRenderer.SetPosition(i, point);
            }
        }





        private void HandleAnimation()
        {
            if (_isThrowing)
            {
                if (_isFacingRight)
                {
                    Debug.Log("ThrowRight");
                    ChangeAnimation("ThrowRight");
                }
                else
                {
                    Debug.Log("ThrowLeft");
                    ChangeAnimation("ThrowLeft");
                }
                return;
            }

            if (_velocitY > -1)
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

            else if (_velocity.x != 0 || _velocity.z != 0)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    _animator.speed = 2f;
                }
                else
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
            else if (_velocity.x == 0 && _velocity.z == 0)
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