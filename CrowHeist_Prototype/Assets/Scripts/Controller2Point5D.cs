using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace KinematicCharacterController.Examples
{
    [RequireComponent(typeof(CharacterController))]
    public class Controller2Point5D : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 50f;
        [SerializeField] private float _smoothTime = 0.05f;
        [SerializeField] private float _jumpForce = 40f;
        [SerializeField] private float _gravityMultiplier = 2f; // Extra gravity when falling add later for airtime

        //Falling
        public float _fallingTime = 0f;


        [Header("PickUP")]
        [SerializeField] private Transform _pickUpPoint;
        [SerializeField] private Transform _handPoint;
        [SerializeField] private Transform _dropPoint;
        public bool _isDirty = false;


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
        public bool _isThrowing = false;
        private bool _canJump = true;


        private Vector2 _input;
        private Vector3 _direction;
        private Vector3 _velocity;
        private float _velocitY;
        private float _gravity = 7f;

        private List<IPickupable> _pickUpsList = new List<IPickupable>();
        private Equipable _equipped;

        private Animator _animator;

        public Vector3 throwDirection;
        public float maxThrowForce = 50f;
        public float chargeTime = 2f;
        private float throwForce = 0f;
        private bool isCharging = false;
        private bool isCanceled = false;
        private float chargeStartTime;
        private LineRenderer lineRenderer;  // LineRenderer to draw trajectory
        public Rigidbody heldObject;
        private Vector3 storedThrowDirection = Vector3.zero;

        //Jack in the Box

        private GameObject _touchingObject;
        private GameObject _currentGroundObject;
        private GameObject _currentHeadbuttObject;

        private bool isWindingUp = false;
        private float windUpTime = 1f; // Time player needs to hold 'F'
        private float windUpTimer = 0f;
        private bool isTimerActive = false;
        public float bounceDelay = 2f; // Delay before bounce is applied
        private float bounceTimer = 0f;
        public bool canBounce = false;
        private bool isInTrigger = false;
        public int pointCount;
        public Vector3 startPoint;
        public Vector3 endPoint;

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

        // Roomba knockback

        public float knockbackDuration = 0.3f;
        private Vector3 knockbackVelocity;
        private float knockbackTimer = 0f;

        // Fan Force
        private Vector3 externalForce;
        [SerializeField] private float externalForceDecay = 5f;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _animator = GetComponentInChildren<Animator>();

        }
        void Start()
        {
            AIEventManager aiEventManager = FindObjectOfType<AIEventManager>();
            if (aiEventManager != null)
            {
                aiEventManager.e_makedirty.AddListener(OnObjectDirty);
            }
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
            HandleExternalForces();
            HandleMove();
            HandleRotation();
            HandleWindUp();
            HandleBounce();

        }

        private void HandleMove()
        {
            if (knockbackTimer > 0)
            {
                _characterController.Move(knockbackVelocity * Time.deltaTime);
                knockbackTimer -= Time.deltaTime;
            }
            if (_isDashing) return; // Don't allow movement input during dash

            _input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _direction = new Vector3(_input.x, _direction.y, _input.y);

            if (IsGrounded && Input.GetButton("Jump") && _canJump)
            {
                _direction.y = 0f;
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
        private void HandleExternalForces()
        {
            if (externalForce.magnitude > 0.01f)
            {
                _characterController.Move(externalForce * Time.deltaTime);
                externalForce = Vector3.Lerp(externalForce, Vector3.zero, externalForceDecay * Time.deltaTime);
            }
        }

        private IEnumerator Dash()
        {
            GameObject coffeeDrink = null;
            GameObject coffee = null;
            if (heldObject == null || !heldObject.CompareTag("Dashable"))
            {
                yield break; // Exit the coroutine if the object is not dashable
            }
            if(heldObject.CompareTag("Dashable"))
            {
                coffeeDrink = heldObject.gameObject;
            }
            foreach (Transform child in GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "CoffeeLiquid")
                {
                    coffee = child.gameObject;
                    break;
                }
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
            coffee.transform.localPosition = new Vector3(0, 0.0076f, 0);
            coffeeDrink.tag = "Mug";
            Drop();
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
            // Headbutt check — looking for objects the player hits from below
            if (Vector3.Dot(hit.normal, Vector3.down) > 0.5f)
            {
                _currentHeadbuttObject = hit.gameObject;

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

            _velocitY = _jumpForce;
        }
        void HandleWindUp()
        {
            if (IsGrounded && _touchingObject != null && _touchingObject.CompareTag("JackInTheBox"))
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

                if (Input.GetKey(KeyCode.F))
                {
                    windUpTimer += Time.deltaTime;

                    if (windUpTimer >= windUpTime)
                    {
                        jackInTheBox.SetActive(false);
                        isTimerActive = true; // Start bounce delay timer
                        windUpTimer = 0f;
                        Debug.Log("Jack-in-the-Box wound up! Waiting for launch...");
                        jack = _touchingObject;

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

                jackInTheBox.gameObject.SetActive(true);
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
            if(other.CompareTag("OffSwitch") && !IsGrounded)
            {
                Debug.Log("Off Switch");
                var offSwitchToOn = other.GetComponentInParent<FanSwitch>();
                offSwitchToOn.ToggleSwitchOn();
            }
            if(other.CompareTag("OnSwitch") && IsGrounded && _currentGroundObject != null && _currentGroundObject.CompareTag("OnSwitch"))
            {
                Debug.Log("On Switch");
                var onSwitchToOff = other.GetComponentInParent<FanSwitch>();
                onSwitchToOff.ToggleSwitchOff();
            }
            if(other.CompareTag("FanBase"))
            {
                Debug.Log("Hit Base");
                Destroy(other.gameObject);
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
                if (_equipped != null || heldObject != null)
                    return;

                AIEventManager.instance.e_pickup.Invoke();
                LayerMask interactable = LayerMask.GetMask("Interactable");
                Collider[] interactableColliders = Physics.OverlapSphere(transform.position, 2, interactable);

                // Filter to only valid pickup targets
                var validTargets = interactableColliders
                    .Where(col => col.TryGetComponent<Equipable>(out _) || col.TryGetComponent<IPickupable>(out _))
                    .OrderBy(col => Vector3.Distance(transform.position, col.transform.position))
                    .ToArray();

                if (validTargets.Length > 0)
                {
                    Collider closest = validTargets[0];

                    if (closest.TryGetComponent(out Equipable equipable))
                    {
                        equipable.Equip(_handPoint);
                        _equipped = equipable;
                        return;
                    }

                    if (closest.TryGetComponent(out IPickupable pickUp))
                    {
                        pickUp.PickUP(_pickUpPoint);
                        _pickUpsList.Add(pickUp);
                        heldObject = closest.GetComponent<Rigidbody>();
                    }
                }
            }


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

                        // Final throw direction calculation
                        Vector3 mousePosition = Input.mousePosition;
                        mousePosition.z = 10f; // Adjust based on your setup
                        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

                        Vector3 playerPosition = transform.position;  // Assuming this is the player's position
                        Vector3 throwDirection = (worldMousePos - playerPosition);
                        throwDirection.y = Mathf.Clamp(throwDirection.y, -0.2f, 0.2f); // Limit vertical influence
                        throwDirection = throwDirection.normalized;

                        Vector3 startPoint = lineRenderer.GetPosition(pointCount - 2);
                        Vector3 endPoint = lineRenderer.GetPosition(pointCount - 1);

                        Vector3 rotationDirection = (endPoint - startPoint).normalized;


                        rigidbody.AddForce(storedThrowDirection * throwForce, ForceMode.Impulse); // Use stored direction
                        heldObject.transform.rotation = Quaternion.LookRotation(new Vector3(rotationDirection.x, -90, rotationDirection.z));
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
        
        public void Drop()
        {
            foreach (IPickupable pickUp in _pickUpsList)
            {
                pickUp.Drop(_dropPoint.position);
                new WaitForSeconds(0.1f);
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
            int resolution = 20; // More points = smoother curve
            float timeStep = 0.1f; // Time increment per point
            Vector3 startPosition = transform.position;
            Vector3 velocity = direction * throwForce; // Use the same throw force
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

        void OnObjectDirty()
        {
            if (heldObject != null)
            {
                Debug.Log("not null");

            }

        }
    }
    
}