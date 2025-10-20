using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using FMODUnity;
using FMOD.Studio;
using Unity.VisualScripting;
using JetBrains.Annotations;
using UnityEngine.TextCore.Text;

namespace KinematicCharacterController.Examples
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Controller2Point5D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] public float _moveSpeed = 50f;
        [SerializeField] private float _smoothTime = 0.05f;
        [SerializeField] private float _jumpForce = 40f;
        [SerializeField] public float _gravityMultiplier = 2f;
        [SerializeField] private LayerMask _groundLayer = -1; // Set in inspector for ground detection
        [SerializeField] private float _groundCheckDistance = 0.15f;
        [SerializeField] private float _skinWidth = 0.02f; // Smaller value to prevent bouncing

        //Sprite
        [SerializeField] GameObject playerSprite;
        [SerializeField] Transform spriteRotator;
        
        // Soda Variables
        public bool _isSpeedBoosted = false;
        public float _speedBoostDuration = 5f;
        public float _normalMoveSpeed;
        public float _speedBoostMultiplier = 5f;

        //Falling
        public float _fallingTime = 0f;
        private float _maxFallSpeed = 20f;

        [Header("PickUP")]
        [SerializeField] private Transform _pickUpPoint;
        [SerializeField] private Transform _handPoint;
        [SerializeField] private Transform _dropPoint;
        public bool _isDirty = false;

        [Header("Dash")]
        [SerializeField] public float _dashSpeed = 40f;
        [SerializeField] public float _dashDuration = 0.2f;
        [SerializeField] public float _dashForce = 10f;
        public float _dashCooldown = 1f;
        public bool _canDash = true;
        public bool _isDashing = false;

        [Header("Added Jump Features")]
        [SerializeField] private float _coyoteTime = 0.15f; // Time after leaving ground where jump is still allowed
        [SerializeField] private float _jumpBufferTime = 0.2f; // Time before landing where jump input is remembered
        private float _coyoteTimeCounter = 0f;
        private float _jumpBufferCounter = 0f;

        //Physics/Direction
        public Rigidbody _rb;
        private CapsuleCollider _capsuleCollider;
        private string _currentAnim;
        private Quaternion targetRotation = Quaternion.identity;
        public bool _isFacingRight = true;
        public bool _isMovingForward = false;
        public bool _isMovingBackward = false;
        private bool _isFlipped = true;
        public bool _isThrowing = false;
        private bool _canJump = true;
        private bool _isJumping = false;
        private Vector2 _input;
        private Vector2 _lastMovementInput;
        private Vector3 _moveVelocity;
        private bool _isGrounded = false;
        private bool _wasGroundedLastFrame = false;
        private List<IPickupable> _pickUpsList = new List<IPickupable>();
        private Animator _animator;

        //Charged Throw
        public Vector3 throwDirection;
        public float maxThrowForce = 50f;
        public float chargeTime = 2f;
        private float throwForce = 0f;
        private bool isCharging = false;
        private bool isCanceled = false;
        private float chargeStartTime;
        private LineRenderer lineRenderer;
        public Rigidbody heldObject;
        private Vector3 storedThrowDirection = Vector3.zero;
        private List<Interactable> nearbyInteractables = new List<Interactable>();
        private int currentTargetIndex = 0;
        private int previousTargetIndex = 0;

        //Jack in the Box
        private GameObject _touchingObject;
        private GameObject _currentGroundObject;
        private GameObject _currentHeadbuttObject;
        private bool isWindingUp = false;
        private float windUpTime = 1f;
        private float windUpTimer = 0f;
        private bool isTimerActive = false;
        
        //Bouncing
        public float bounceDelay = 2f;
        private float bounceTimer = 0f;
        public bool canBounce = false;
        private bool isInTrigger = false;
        public int pointCount;
        public Vector3 startPoint;
        public Vector3 endPoint;

        #region Properties
        public bool IsGrounded => _isGrounded;
        public bool IsFlipped => _isFlipped;
        public bool IsFacingRight => _isFacingRight;
        #endregion

        // Knockback
        public float knockbackDuration = 0.3f;
        private Vector3 knockbackVelocity;
        private float knockbackTimer = 0f;

        // Fan Force
        private Vector3 externalForce;
        [SerializeField] private float externalForceDecay = 5f;
        [SerializeField] private float externalForceDamping = 0.9f;

        //Audio
        [SerializeField] private EventReference playerFootsteps;
        private EventInstance footstepInstance;

        private void Awake()
        {
            _normalMoveSpeed = _moveSpeed;
            _rb = GetComponent<Rigidbody>();
            _capsuleCollider = GetComponent<CapsuleCollider>();
            _animator = GetComponentInChildren<Animator>();
            
            // Configure Rigidbody for character movement
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rb.drag = 0f; // Increased drag for stability
            _rb.angularDrag = 0f;
            _rb.constraints = RigidbodyConstraints.FreezeRotation;
            
            // Set a reasonable mass
            _rb.mass = 1f;
            
            // Configure solver iterations for better collision
            _rb.solverIterations = 10;
            _rb.solverVelocityIterations = 10;
        }

        public void Start()
        {
            AIEventManager aiEventManager = FindObjectOfType<AIEventManager>();
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;

            footstepInstance = AudioManager.Instance.CreateInstance(playerFootsteps);
        }

        void Update()
        {
            UpdateCoyoteTime();
            // Input and state checks in Update
            HandleInput();
            // Handle item-specific mechanics
            if (heldObject != null)
            {
                if (heldObject.CompareTag("Soda"))
                {
                    SodaCanDash sodaDash = heldObject.GetComponent<SodaCanDash>();
                    if (sodaDash != null)
                    {
                        sodaDash.HandleDash();
                    }
                }
                else if (heldObject.CompareTag("Dashable"))
                {
                    CoffeeConsumption coffee = heldObject.GetComponent<CoffeeConsumption>();
                    if (coffee != null)
                {
                        coffee.TryConsumeCoffee();
                    }
                }
                else if (heldObject.CompareTag("Glider"))
                {
                    PaperGlider glider = heldObject.GetComponent<PaperGlider>();
                    if (glider != null)
                    {
                        glider.HandleGliding();
                    }
                }
            }
            HandleAnimation();
            HandlePickUP();
            HandleWindUp();
            HandleBounce();
        }

        void FixedUpdate()
        {
            // Physics in FixedUpdate
            CheckGrounded();
            HandleMove();
            HandleGravity();
            HandleExternalForces();
            HandleKnockback();

        }

        void LateUpdate()
        {
            HandleRotation(); 
        }


        private void CheckGrounded()
        {
            _wasGroundedLastFrame = _isGrounded;

            // Cast from the center of the character downward
            Vector3 origin = transform.position;
            float radius = _capsuleCollider.radius * 0.9f;

            // Start the cast from just below the center
            Vector3 castOrigin = origin;

            RaycastHit hit;
            // Cast distance should reach just below the feet
            float castDistance = (_capsuleCollider.height * 0.5f) + _groundCheckDistance;

            // Main ground check using a raycast for more precision
            _isGrounded = Physics.Raycast(castOrigin, Vector3.down, out hit, castDistance, _groundLayer);

            // Additional check with spherecast for better edge detection
            //------this block of code is responsible for phasing through colliders)
            if (!_isGrounded)
            {
                _isGrounded = Physics.SphereCast(castOrigin, radius * 0.5f, Vector3.down, out hit, castDistance, _groundLayer);
            }

            if (_isGrounded && hit.collider != null)
            {
                _currentGroundObject = hit.collider.gameObject;

                // Keep player at proper height above ground
                float targetHeight = hit.point.y + (_capsuleCollider.height * 0.5f);
                float currentHeight = transform.position.y;

                // Only apply correction if significantly below target height (actually sinking)
                if (currentHeight < targetHeight - 0.01f && _rb.velocity.y <= 0)
                {
                    // Use position-based correction with physics
                    Vector3 targetPos = new Vector3(transform.position.x, targetHeight, transform.position.z);
                    _rb.MovePosition(Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * 10f));
                }
            }
            else
            {
                _currentGroundObject = null;
            }
        }

        private void HandleInput()
        {
            _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            if (Input.GetButtonDown("Jump"))
            {
                _jumpBufferCounter = _jumpBufferTime;
            }

            // Handle jump buffering and coyote time
            if (_jumpBufferCounter > 0f)
            {
                if ((_isGrounded || _coyoteTimeCounter > 0f) && !_isJumping)
                {
                    Jump();
                    _jumpBufferCounter = 0f;
                }
            }

            if (_jumpBufferCounter > 0f)
            {
                _jumpBufferCounter -= Time.deltaTime;
            }
        }

        private void HandleMove()
        {
            if (_isDashing) return;

            Vector3 moveDirection = new Vector3(_input.x, 0, _input.y);
            _moveVelocity = moveDirection * _moveSpeed;

            // Set velocity directly instead of using MovePosition
            Vector3 targetVelocity = new Vector3(_moveVelocity.x, _rb.velocity.y, _moveVelocity.z);
            _rb.velocity = targetVelocity;

            // Get the velocity
            Vector3 horizontalMove = _rb.velocity;
            // Don't use the vertical velocity
            horizontalMove.y = 0;
            // // Calculate the approximate distance that will be traversed
            // float distance =  horizontalMove.magnitude * Time.fixedDeltaTime;
            // // Normalize horizontalMove since it should be used to indicate direction
            // horizontalMove.Normalize();
            // RaycastHit hit;

            // // Check if the body's current velocity will result in a collision
            // if(_rb.SweepTest(horizontalMove, out hit, distance))
            // {
            //     // If so, stop the movement
            //     _rb.velocity = new Vector3(0, _rb.velocity.y, 0);
            // }
        }

        private void HandleGravity()
        {
            if (!_isGrounded)
            {
                // Apply normal gravity
                float gravityForce = Physics.gravity.y * _gravityMultiplier;
                _rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);

                // Clamp fall speed
                if (_rb.velocity.y < -_maxFallSpeed)
                {
                    _rb.velocity = new Vector3(_rb.velocity.x, -_maxFallSpeed, _rb.velocity.z);
                }

                _fallingTime += Time.fixedDeltaTime;
            }
            else
            {
                _fallingTime = 0f;

                // When grounded, stop excessive downward velocity
                if (_rb.velocity.y < -0.5f)
                {
                    _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
                }

                _isJumping = false;
            }
        }

        private void Jump()
        {
            // Prevent jumping if already airborne
            if (_isJumping || !_canJump)
            {
                return;
            }
            _isJumping = true;
            _isGrounded = false;
            _coyoteTimeCounter = 0f;
            _canJump = false;

            // Reset vertical velocity before jump
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);

            // Re-enable jumping only when grounded again
            StartCoroutine(ResetJumpFlag());
        }

        private IEnumerator ResetJumpFlag()
        {
            // Adds small delay so that we do not re-enable jump mid-air
            yield return new WaitUntil(() => _isGrounded);
            _isJumping = false;
            _canJump = true;
        }        

        private void UpdateCoyoteTime()
        {
            // If grounded, reset coyote time
            if (_isGrounded)
            {
                _coyoteTimeCounter = _coyoteTime;
            }
            else
            {
                // Count down coyote time when not grounded
                if (_coyoteTimeCounter > 0)
                {
                    _coyoteTimeCounter -= Time.deltaTime;
                }
            }
        }

        private void HandleExternalForces()
        {
            if (externalForce.magnitude > 0.01f)
            {
                _rb.AddForce(externalForce, ForceMode.Force);
                externalForce *= externalForceDamping;

                if (externalForce.magnitude < 0.01f)
                {
                    externalForce = Vector3.zero;
                }
            }
        }

        private void HandleKnockback()
        {
            if (knockbackTimer > 0)
            {
                _rb.AddForce(knockbackVelocity, ForceMode.Force);
                knockbackTimer -= Time.fixedDeltaTime;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollisionLogic(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            // Check for ground contact
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.5f)
                {
                    _currentGroundObject = collision.gameObject;
                }
            }
        }

        private void HandleCollisionLogic(Collision collision)
        {
            if (_isDashing)
            {
                Rigidbody otherRb = collision.rigidbody;
                if (otherRb != null && !otherRb.isKinematic)
                {
                    Vector3 forceDirection = collision.contacts[0].point - transform.position;
                    forceDirection = forceDirection.normalized;
                    
                    float forceAmount = 20f;
                    otherRb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
                    
                    _isDashing = false;
                }
            }
            
            // Headbutt check
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.down) > 0.5f)
                {
                    _currentHeadbuttObject = collision.gameObject;
                }
            }
        }
        
        private void HandleRotation()
        {
            _isMovingForward = (_input.y > 0);
            _isMovingBackward = (_input.y < 0);
            
            // Only update facing direction if there's actual horizontal input
            if (_input.x > 0.01f)
            {
                _isFlipped = true;
                _isFacingRight = true;
            }
            else if (_input.x < -0.01f)
            {
                _isFlipped = false;
                _isFacingRight = false;
            }
            // If no input, _isFlipped and _isFacingRight retain their previous values
            
            Flip(_isFlipped);
        }


        void HandleWindUp()
        {
            if (_isGrounded && _touchingObject != null && _touchingObject.CompareTag("JackInTheBox"))
            {
                GameObject jack = _touchingObject;
                GameObject jackInTheBox = null;
                
                foreach (Transform child in jack.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "SpringFunction")
                    {
                        jackInTheBox = child.gameObject;
                        break;
                    }
                }

                if (Input.GetKey(KeyCode.E))
                {
                    windUpTimer += Time.deltaTime;

                    if (windUpTimer >= windUpTime)
                    {
                        if (jackInTheBox != null)
                        {
                            jackInTheBox.SetActive(false);
                        }
                        isTimerActive = true;
                        windUpTimer = 0f;
                        Debug.Log("Jack-in-the-Box wound up! Waiting for launch...");
                    }
                }
                else
                {
                    windUpTimer = 0f;
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
            if (canBounce && _isGrounded && _currentGroundObject != null && _currentGroundObject.CompareTag("JackInTheBox"))
            {
                GameObject jack = _currentGroundObject;
                GameObject jackInTheBox = null;
                
                foreach (Transform child in jack.GetComponentsInChildren<Transform>(true))
                {
                    if (child.name == "SpringFunction")
                    {
                        jackInTheBox = child.gameObject;
                        break;
                    }
                }

                if (jackInTheBox != null)
                {
                    jackInTheBox.SetActive(true);
                }
                
                ApplyBounce(5f);
                canBounce = false;
            }
        }

        public void ApplyBounce(float bounceStrength)
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
            _rb.AddForce(Vector3.up * bounceStrength, ForceMode.VelocityChange);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Interactable interactable))
            {
                if (!nearbyInteractables.Contains(interactable))
                {
                    Debug.Log("Added Interactable: " + interactable.gameObject.name);
                    nearbyInteractables.Add(interactable);
                    Debug.Log("Interactables: ");
                    foreach (Interactable item in nearbyInteractables)
                    {
                        Debug.Log(item.gameObject.name + "\n");
                    }

                    UpdateHighlightedInteractable();
                }
            }
            
            if (other.CompareTag("JackInTheBox"))
            {
                isInTrigger = true;
                _touchingObject = other.gameObject;
                Debug.Log("Entered Jack In The Box trigger.");
            }
            
            if (other.CompareTag("OffSwitch") && !_isGrounded)
            {
                Debug.Log("Off Switch");
                var offSwitchToOn = other.GetComponentInParent<FanSwitch>();
                offSwitchToOn.ToggleSwitchOn();
            }
            
            if (other.CompareTag("OnSwitch") && _isGrounded && _currentGroundObject != null && _currentGroundObject.CompareTag("OnSwitch"))
            {
                Debug.Log("On Switch");
                var onSwitchToOff = other.GetComponentInParent<FanSwitch>();
                onSwitchToOff.ToggleSwitchOff();
            }
            
            if (other.CompareTag("FanBase"))
            {
                Transform parent = other.transform.parent;
                if (parent != null)
                {
                    GameObject parentObj = parent.gameObject;
                    Debug.Log("Hit Base");
                    var disassembler = parentObj.GetComponent<SpawnFanOnDestroy>();
                    if (disassembler != null)
                    {
                        disassembler.Disassemble();
                    }
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Interactable interactable))
            {
                if (nearbyInteractables.Contains(interactable))
                {
                    Debug.Log("Removed " + interactable.gameObject.name);
                    nearbyInteractables.Remove(interactable);
                    Debug.Log("Interactables: ");
                    foreach (Interactable item in nearbyInteractables)
                    {
                        Debug.Log(item.gameObject.name + "\n");
                    }

                    UpdateHighlightedInteractable();
                }
            }
            
            if (other.CompareTag("JackInTheBox"))
            {
                isInTrigger = false;
                Debug.Log("Exited Jack In The Box trigger.");
            }
        }

        private void UpdateHighlightedInteractable()
        {
            if (nearbyInteractables.Count == 0) return;
            
            if (currentTargetIndex >= nearbyInteractables.Count)
            {
                currentTargetIndex = 0;
            }

            foreach (var interactable in nearbyInteractables)
            {
                if (interactable != null)
                {
                    interactable.SetOutline(false);
                }
            }
            
            Interactable target = nearbyInteractables[currentTargetIndex];
            if (target != null)
            {
                target.SetOutline(true);
            }
        }

        private void HandlePickUP()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {

                AIEventManager.instance.e_pickup.Invoke();

                if (_pickUpsList.Count > 0) return;

                Debug.Log("NearbyInteractablesCount: " + nearbyInteractables.Count + "\n CurrentTargetIndex: " + currentTargetIndex);


                if (nearbyInteractables.Count > 0 && currentTargetIndex < nearbyInteractables.Count)
                {

                    Interactable selected = nearbyInteractables[currentTargetIndex];
                    if (selected != null && selected.realObject != null)
                    {
                        if (selected.realObject.TryGetComponent(out IPickupable pickUp))
                        {
                            pickUp.PickUP(_pickUpPoint);
                            _pickUpsList.Add(pickUp);
                            nearbyInteractables.Remove(selected);
                            foreach (var interactable in nearbyInteractables)
                            {
                                Debug.Log("Item: " + interactable.transform.name);
                            }
                            heldObject = selected.realObject.GetComponent<Rigidbody>();
                        }
                    }
                }
            }

            // Paint bucket logic
            if (heldObject != null && heldObject.CompareTag("PaintBucket"))
            {
                if (Input.GetKey(KeyCode.E))
                {
                    Paint paintBucket = heldObject.GetComponent<Paint>();
                    if (paintBucket != null && paintBucket._paintInBucket > 0)
                    {
                        Debug.Log("Painting! " + paintBucket._paintInBucket);
                        paintBucket.Spill();
                        paintBucket._paintInBucket -= Time.deltaTime;
                    }

                    if (paintBucket._paintInBucket <= 0)
                    {
                        Debug.Log("Out of paint, Dropping bucket");
                        Drop();
                    }
                }
            }

            if (nearbyInteractables.Count > 1 && Input.GetKeyDown(KeyCode.R))
            {
                previousTargetIndex = currentTargetIndex;
                currentTargetIndex++;
                if (currentTargetIndex >= nearbyInteractables.Count)
                {
                    currentTargetIndex = 0;
                }
                UpdateHighlightedInteractable();
            }

            // Charged Throwing
            if (heldObject != null)
            {
                Collider heldCollider = heldObject.GetComponent<Collider>();
                if (heldCollider != null)
                {
                    heldCollider.enabled = true;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    isCharging = true;
                    isCanceled = false;
                    chargeStartTime = Time.time;
                }

                if (Input.GetMouseButton(0) && !isCanceled)
                {
                    throwForce = Mathf.Clamp((Time.time - chargeStartTime) / chargeTime * maxThrowForce, 0, maxThrowForce);

                    Vector3 mousePosition = Input.mousePosition;
                    
                    //mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z + 5f;
                    mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z + 5f;
                    Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

                    //Vector3 playerPosition = transform.position;
                    Vector3 playerPosition = _handPoint.position;
                    storedThrowDirection = (worldMousePos - playerPosition).normalized;

                    DrawThrowTrajectory(storedThrowDirection);
                }

                if (Input.GetMouseButtonUp(0) && !isCanceled)
                {
                    isCharging = false;
                    Rigidbody rigidbody = heldObject.GetComponent<Rigidbody>();
                    if (rigidbody != null)
                    {
                        rigidbody.isKinematic = false;

                        Vector3 mousePosition = Input.mousePosition;
                        mousePosition.z = 10f;
                        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

                        Vector3 playerPosition = transform.position;
                        Vector3 throwDirection = (worldMousePos - playerPosition);
                        throwDirection.y = Mathf.Clamp(throwDirection.y, -0.2f, 0.2f);
                        throwDirection = throwDirection.normalized;

                        Vector3 startPoint = lineRenderer.GetPosition(pointCount - 2);
                        Vector3 endPoint = lineRenderer.GetPosition(pointCount - 1);
                        Vector3 rotationDirection = (endPoint - startPoint).normalized;

                        rigidbody.AddForce(storedThrowDirection * throwForce, ForceMode.Impulse);
                        if (heldObject.CompareTag("Glider"))
                        {
                            //static falling motion
                            //heldObject.transform.rotation = Quaternion.LookRotation(new Vector3(rotationDirection.x, 0, rotationDirection.z));

                            //rotating falling motion
                            Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                            if (rb != null)
                            {
                                Vector3 spinForce = new Vector3(-15f, 0, 0);
                                rb.angularVelocity = spinForce;
                                heldObject.transform.rotation = Quaternion.LookRotation(rotationDirection);
                            }
                        }
                        else
                            heldObject.transform.rotation = Quaternion.LookRotation(new Vector3(rotationDirection.x, -90, rotationDirection.z));
                    }

                    Drop();
                    throwForce = 0f;
                    lineRenderer.positionCount = 0;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    isCharging = false;
                    isCanceled = true;
                    throwForce = 0f;
                    lineRenderer.positionCount = 0;
                    storedThrowDirection = Vector3.zero;
                }
            }
        }
        public void Drop()
        {
            
            foreach (IPickupable pickUp in _pickUpsList)
            {
                pickUp.Drop(_dropPoint.position);
            }
            
            _pickUpsList.Clear();
            heldObject = null;
        }

        public void ApplyKnockback(Vector3 direction, float force)
        {
            knockbackVelocity = direction.normalized * force;
            knockbackTimer = knockbackDuration;
        }

        public void ApplyExternalForce(Vector3 force)
        {
            externalForce += force;
        }

        void DrawThrowTrajectory(Vector3 direction)
        {
            int resolution = 20;
            float timeStep = 0.1f;
            //Vector3 startPosition = transform.position;
            Vector3 startPosition = _handPoint.position;
            Vector3 velocity = direction * throwForce;
            throwDirection = direction;
            pointCount = lineRenderer.positionCount;

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

            if (_rb.velocity.y > 0.1f || !_isGrounded)
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
            else if (_moveVelocity.magnitude > 0.1f)
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
            else
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
            if (_currentAnim == animation) return;
            
            _currentAnim = animation;
            _animator.CrossFade(animation, crossfade);
        }

        private void Flip(bool doFlip)
        {
            float rotationSpeed = 10f;

            // Only rotate if there's movement input
            if (_input.magnitude < 0.01f) return;

            //input:
            // 1 - right away from camera
            // -1 - left and towards camera
            //slightly edited rotation system so crowley better faces which direction he's moving
            //Rotate Sprite (1) vs rotate Player RB (2)

            if (_input.x >= 0 && _input.x <= 1 && _input.y == 0) // right
            {
                playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * rotationSpeed);
            }
            else if (_input.x >= 0 && _input.x <= 1 && _input.y >= 0 && _input.y <= 1) //back right
            {
                playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, Quaternion.Euler(0, -45, 0), Time.deltaTime * rotationSpeed);
            }
            else if (_input.x >= 0 && _input.x <= 1 && _input.y <= 0 && _input.y >= -1) //front right
            {
                playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, Quaternion.Euler(0, -305, 0), Time.deltaTime * rotationSpeed);
            }
            else if (_input.x >= -1 && _input.x <= 0 && _input.y == 0) //left
            {
                playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, Quaternion.Euler(0, -180, 0), Time.deltaTime * rotationSpeed);
            }
            else if (_input.x >= -1 && _input.x <= 0 && _input.y >= 0 && _input.y <= 1) //back left
            {
                playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, Quaternion.Euler(0, -135, 0), Time.deltaTime * rotationSpeed);
            }
            else if (_input.x >= -1 && _input.x <= 0 && _input.y <= 0 && _input.y >= -1) //front left
            {
                playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, Quaternion.Euler(0, -215, 0), Time.deltaTime * rotationSpeed);
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 2);
            
            // Draw ground check
            if (_capsuleCollider != null)
            {
                Gizmos.color = _isGrounded ? Color.green : Color.yellow;
                Vector3 origin = transform.position + Vector3.up * (_capsuleCollider.height * 0.5f);
                Gizmos.DrawWireSphere(origin - Vector3.up * ((_capsuleCollider.height * 0.5f) + _groundCheckDistance), _capsuleCollider.radius * 0.9f);
            }
        }

    }
}