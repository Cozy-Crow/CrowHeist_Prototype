using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using SHG.AnimatorCoder;
using System;
using Unity.Mathematics;

namespace KinematicCharacterController.Examples
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Controller2Point5D : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Sockets sockets;
        [SerializeField] private CrowleySFX crowleySFX;

        #region Movement Variables
        [Header("Movement")]
        [SerializeField] public float moveSpeed = 5;
        [SerializeField] private float smoothTime = 0.05f;
        [SerializeField] private float jumpForce = 20f;
        [SerializeField] private float rotationSpeed = 10f;         // Speed of sprite rotation slerp
        [SerializeField] private float flipPadding = 1.5f;          // Padding for slerp blur
        [SerializeField] public float gravityMultiplier = 2f;
        [SerializeField] private LayerMask groundLayer = -1;        // Set in inspector for ground detection
        [SerializeField] private float groundCheckDistance = 0.15f;
        [SerializeField] private float skinWidth = 0.02f;           // Smaller value to prevent bouncing
        private Vector2 input;
        private Vector3 faceDirection = Vector3.forward;
        private float faceAngle = 0f;
        private float newFaceAngle = 0f;
        private Vector3 velocity;
        private bool isGrounded = false;

        private bool canInput = true; // Zack H (2/4) used to track if inputs are accepted (mainly within a menu
        private string surfaceTag = "";

        public Vector3 Velocity => velocity;
        public Vector3 FaceDirection => faceDirection;
        public bool IsGrounded => isGrounded;
        public string SurfaceTag => surfaceTag;
        public bool IsThrowing { get => isThrowing; set => isThrowing = value; }
        public bool ChargeThrowing { get => chargingThrow;}
        #endregion

        #region Animation
        //Sprite
        [SerializeField] GameObject playerSprite;
        [SerializeField] AnimatorCoder animatorCoder;
        #endregion
        
        // Soda Variables
        public bool isSpeedBoosted = false;
        public float speedBoostDuration = 5f;
        public float normalMoveSpeed;
        public float speedBoostMultiplier = 5f;

        //Falling
        public float fallingTime = 0f;
        private float maxFallSpeed = 20f;

        [Header("PickUp")]
        [SerializeField] private Transform pickUpPoint;
        [SerializeField] private Transform handPoint;
        [SerializeField] private Transform dropPoint;
        public bool isDirty = false;

        #region Dash Variables
        [Header("Dash")]
        [SerializeField] public float dashSpeed = 40f;
        [SerializeField] public float dashDuration = 0.3f;
        [SerializeField] public float dashForce = 10f;
        public float dashCooldown = 1f;
        public bool canDash = true;
        public bool isDashing = false;
        #endregion

        [Header("Added Jump Features")]
        [SerializeField] private float coyoteTime = 0.15f; // Time after leaving ground where jump is still allowed
        [SerializeField] private float jumpBufferTime = 0.2f; // Time before landing where jump input is remembered
        private float coyoteTimeCounter = 0f;
        private float jumpBufferCounter = 0f;

        //Physics/Direction
        public Rigidbody rb;
        private CapsuleCollider[] capsuleCollider; //stores both colliders
        private CapsuleCollider normalCollider;
        private CapsuleCollider triggerCollider;
        public bool isFacingRight = true;
        public bool isThrowing = false;
        public bool canJump = true;
        private bool isJumping = false;
        private bool onSlope = false;
        private bool wasGroundedLastFrame = false;
        public List<IPickupable> _pickUpsList = new List<IPickupable>();

        [Header("Charged Throw Settings")] 
        [SerializeField] private GameObject targetAssetObject;
        public Vector3 throwDirection;
        public float maxThrowForce = 50f;
        public float chargeTime = 2f;
        private float throwForce = 0f;
        private bool chargingThrow = false;
        private bool cancelThrow = false;
        private float chargeStartTime;
        private Vector3 storedThrowVelocity;
        private LineRenderer lineRenderer;
        public Rigidbody heldObject;
        private Vector3 storedThrowDirection = Vector3.zero;
        private List<Interactable> nearbyInteractables = new List<Interactable>();
        private int currentTargetIndex = 0;
        private int previousTargetIndex = 0;

        //Charged Throw Arc Settings
        [SerializeField] private float minArc = 0.2f;
        [SerializeField] private float maxArc = 0.8f;
        [SerializeField] private AnimationCurve arcCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField] private float throwPowerScaler = 1f;

        //Hit Detection
        private GameObject touchingObject;
        private GameObject currentGroundObject;
        private GameObject currentHeadbuttObject;
        
        //Bouncing
        public float bounceDelay = 2f;
        private float bounceTimer = 0f;
        public bool canBounce = false;
        private bool isInTrigger = false;
        public int pointCount;
        public Vector3 startPoint;
        public Vector3 endPoint;

        #region Properties
        #endregion

        // Knockback
        public float knockbackDuration = 0.3f;
        private Vector3 knockbackVelocity;
        private float knockbackTimer = 0f;

        // External Force
        private Vector3 externalForce;
        [SerializeField] private float externalForceDecay = 5f;
        [SerializeField] private float externalForceDamping = 0.9f;

        [Header("Audio")]
        [SerializeField] private EventReference dashActivate;
        [SerializeField] private EventReference jump;
        [SerializeField] private EventReference land;
        private EventReference ObjThrowAudio;

        [SerializeField] private EventReference charge;
        public EventInstance chargeInstance;


        #region  Trinket Guide
        [Header("Trinket Guide")]
        [SerializeField] private Material trinketGuideMaterial;
        [SerializeField] private float trinketGuideWidth = 0.1f;
        [SerializeField] private Color trinketGuideColor = Color.yellow;
        private LineRenderer trinketGuideLine;
        private bool hasPickedUpTrinket = false;
        private Transform nearestWindow;
        #endregion

        private void Awake()
        {
            normalMoveSpeed = moveSpeed;
            rb = GetComponent<Rigidbody>();
            capsuleCollider = GetComponents<CapsuleCollider>();
            //0 and 1 based on order in inspector
            // - trigger is listed first in inspector 
            triggerCollider = capsuleCollider[0];
            normalCollider = capsuleCollider[1];
            crowleySFX = GetComponent<CrowleySFX>();
        }

        public void Start()
        {
            AIEventManager aiEventManager = FindObjectOfType<AIEventManager>();
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;

            SetupTrinketGuideLine();
            animatorCoder = GetComponentInChildren<AnimatorCoder>();

            //creates audio instances
            AudioManager.Instance?.CreateInstance("land", land);
            if(AudioManager.Instance != null)
            {
             chargeInstance = AudioManager.Instance.CreateInstance("charge", charge);   
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                TrinketMenu.instance.ToggleMenu();
            }
            UpdateCoyoteTime();
            // Input and state checks in Update
            HandleInput();

            HandleMove();
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
                // else if (heldObject.CompareTag("Dashable"))
                // {
                //     CoffeeConsumption coffee = heldObject.GetComponent<CoffeeConsumption>();
                //     if (coffee != null)
                // {
                //         coffee.TryConsumeCoffee();
                //     }
                // }
                else if (heldObject.CompareTag("Glider"))
                {
                    PaperGlider glider = heldObject.GetComponent<PaperGlider>();
                    if (glider != null)
                    {
                        glider.HandleGliding();
                    }
                }
            }
            HandleRotation();
            HandlePickUp();
            HandleBounce();
            if (hasPickedUpTrinket && trinketGuideLine.enabled)
            {
                UpdateTrinketGuideLine();
            }
            RemoveNullItems();
        }

        void FixedUpdate()
        {
            // Physics in FixedUpdate
            CheckGrounded();
            HandleGravity();
            HandleExternalForces();
            HandleKnockback();
        }
    
        private void CheckGrounded()
        {
            wasGroundedLastFrame = isGrounded;

            // Cast from the center of the character downward
            Vector3 origin = transform.position;
            float radius = normalCollider.radius * 0.9f;

            // Cast distance should reach just below the feet
            float castDistance = (normalCollider.height * 0.5f) + groundCheckDistance; //1.215

            // Main ground check using a raycast for more precision
            isGrounded = Physics.Raycast(origin, Vector3.down, out RaycastHit hitMain, castDistance, groundLayer, QueryTriggerInteraction.Ignore);

            if(hitMain.collider != null)
            {
                surfaceTag = hitMain.collider.gameObject.tag;
                if(surfaceTag.Equals("Untagged") || surfaceTag.Equals("Ground"))
                {
                    surfaceTag = "Generic";
                }
                 crowleySFX.SetInstanceLabelParam("Footstep", "Surface", surfaceTag);
                 AudioManager.Instance?.SetInstanceLabelParam("land", "Surface", surfaceTag);
            }
        }
        private void HandleInput()
        {
            //Note: Zack H. 2/4
            // if not allowed input, dont allow input
            // sets input to 0 so that crowley will stop
            if(!canInput)
            {
                // 0 the input vector to stop movement
                input = new Vector2();
                return;
            }

            // Zack H. 1/20:
            // Changed from Input.GetAxis to Input.GetAxisRaw
            // Apparently GetAxis has smoothing to it to slowly progress to 0
            // instead of instantly setting to 0, making you not stop.
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

            if (Input.GetButtonDown("Jump"))
            {
                jumpBufferCounter = jumpBufferTime;
            }

            // Handle jump buffering and coyote time
            if (jumpBufferCounter > 0f)
            {
                if ((isGrounded || coyoteTimeCounter > 0f) && !isJumping)
                {
                    Jump();
                    jumpBufferCounter = 0f;
                }
            }

            if (jumpBufferCounter > 0f)
            {
                jumpBufferCounter -= Time.deltaTime;
            }

            //sets parameter for footstep audio to change from running or walking
            //  if (Input.GetKey(KeyCode.LeftShift))
            //  {
            //      crowleySFX.SetInstanceFloatParam("Footstep", "WalkRun", 1);
            //  }
            //  else
            //  {
            //      crowleySFX.SetInstanceFloatParam("Footstep", "WalkRun", 0);
            //  }
        }

        private void HandleMove()
        {

            if (isDashing) return;
 
            velocity = new Vector3(input.x, 0, input.y) * moveSpeed;

            //Get the last face direction
            // Update face direction only on the axes that have non-zero movement
            
            if(input.x == 0 && input.y != 0)
            {
                faceDirection.z = input.y;
            }
            else if(input.magnitude > 0.1f)
            {
                faceDirection.x = input.x;
                faceDirection.z = input.y;
            }

            // Set velocity directly instead of using MovePosition
            Vector3 targetVelocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);
            
            rb.velocity = targetVelocity;


        }

        private void HandleGravity()
        {
            if (!isGrounded)
            {
                // Apply normal gravity
                float gravityForce = Physics.gravity.y * gravityMultiplier;
                rb.AddForce(Vector3.up * gravityForce, ForceMode.Acceleration);

                // Clamp fall speed
                if (rb.velocity.y < -maxFallSpeed)
                {
                    rb.velocity = new Vector3(rb.velocity.x, -maxFallSpeed, rb.velocity.z);
                }

                fallingTime += Time.fixedDeltaTime;
            }
            else
            {
                fallingTime = 0f;

                // When grounded, stop excessive downward velocity
                if (rb.velocity.y < -0.5f)
                {
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

                    //plays audio when landing
                    AudioManager.Instance?.PlayInstanceOneShot("land");
                    
                }

                isJumping = false;
            }
        }

        private void Jump()
        {
            // Prevent jumping if already airborne
            if (isJumping || !canJump)
            {
                return;
            }
            isJumping = true;
            isGrounded = false;
            coyoteTimeCounter = 0f;
            canJump = false;
            AudioManager.Instance?.PlayOneShot(jump); 

            // Reset vertical velocity before jump
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);

            // Re-enable jumping only when grounded again
            StartCoroutine(ResetJumpFlag());
        }

        private IEnumerator ResetJumpFlag()
        {
            // Adds small delay so that we do not re-enable jump mid-air
            yield return new WaitUntil(() => isGrounded);
            isJumping = false;
            canJump = true;
        }        

        private void UpdateCoyoteTime()
        {
            // If grounded, reset coyote time
            if (isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                // Count down coyote time when not grounded
                if (coyoteTimeCounter > 0)
                {
                    coyoteTimeCounter -= Time.deltaTime;
                }
            }
        }

        private void HandleExternalForces()
        {
            if (externalForce.magnitude > 0.01f)
            {
                rb.AddForce(externalForce, ForceMode.Force);
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
                rb.AddForce(knockbackVelocity, ForceMode.Force);
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
                    currentGroundObject = collision.gameObject;
                }
            }
        }

        private void HandleCollisionLogic(Collision collision)
        {
            if(collision.gameObject.layer == 9) //testing
            {
                Physics.IgnoreCollision(collision.gameObject.GetComponent<Collider>(), this.GetComponent<Collider>());
            }

            if (isDashing)
            {
                Rigidbody otherRb = collision.rigidbody;
                if (otherRb != null && !otherRb.isKinematic)
                {
                    Vector3 forceDirection = collision.contacts[0].point - transform.position;
                    forceDirection = forceDirection.normalized;
                    
                    float forceAmount = 20f;
                    otherRb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
                    
                    isDashing = false;
                }
            }
            
            // Headbutt check
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.down) > 0.5f)
                {
                    currentHeadbuttObject = collision.gameObject;
                }
            }
        }
        
        private void HandleRotation()
        {
            Flip(faceDirection);
        }


        void HandleBounce() //jack in the box
        {
            if (canBounce && isGrounded && currentGroundObject != null && currentGroundObject.CompareTag("JackInTheBox"))
            {
                GameObject jack = currentGroundObject;
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
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * bounceStrength, ForceMode.VelocityChange);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Interactable interactable))
            {
                if (!nearbyInteractables.Contains(interactable) )
                {
                    if (heldObject == null || interactable.realObject != heldObject.gameObject)
                    {
                        nearbyInteractables.Add(interactable);
                        UpdateHighlightedInteractable();
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
                    
                    nearbyInteractables.Remove(interactable);
                    UpdateHighlightedInteractable();
                }
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

        private void HandlePickUp()
        {
            // Notes: Zack H 1/25
            //E for left hand on keyboard
            //U for Right hand on keyboard
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.U)) 
            {
                AIEventManager.instance.e_pickup.Invoke();

                Debug.Log("NearbyInteractablesCount: " + nearbyInteractables.Count + "\n CurrentTargetIndex: " + currentTargetIndex);

                if (nearbyInteractables.Count > 0 && currentTargetIndex < nearbyInteractables.Count)
                {

                    Interactable selected = nearbyInteractables[currentTargetIndex];
                    if (selected != null && selected.realObject != null)
                    {

                        //if the object can be picked up, pick it up
                        if (selected.realObject.TryGetComponent(out IPickupable pickUp))
                        {
                            if (_pickUpsList.Count > 0) return;
                            
                            Pickup(selected, pickUp);
                        }
                        //otherwise interact with it
                        else
                        {
                            Debug.Log("calling on " + selected);
                            selected.TriggerInteraction(heldObject == null? null : heldObject.GetComponent<Pickable>());
                        }
                    }
                }
            }
            
            // rotate to the next nearby item
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
                    chargingThrow = true;
                    cancelThrow = false;
                    chargeStartTime = Time.time;
                    chargeInstance.start();
                }

                if (Input.GetMouseButton(0) && !cancelThrow)
                {
                    throwForce = Mathf.Clamp((Time.time - chargeStartTime) / chargeTime * maxThrowForce, 0, maxThrowForce);

                    Vector3 mousePosition = Input.mousePosition;
                    
                    //mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z + 5f;
                    mousePosition.z = Camera.main.WorldToScreenPoint(transform.position).z + 5f;
                    Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePosition);

                    //Vector3 playerPosition = transform.position;
                    Vector3 playerPosition = handPoint.position;
                    storedThrowDirection = (worldMousePos - playerPosition).normalized;

                    DrawThrowTrajectory(storedThrowDirection);
                    
                }

                if (Input.GetMouseButtonUp(0) && !cancelThrow)
                {
                    isThrowing = true;
                    chargingThrow = false;
                    Rigidbody rigidbody = heldObject.GetComponent<Rigidbody>();

                    print("THROW");
                    chargeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    AudioManager.Instance?.PlayOneShot(ObjThrowAudio);
                    //RuntimeManager.PlayOneShot("event:/SFX/Objects/Coin/CoinCollect");

                    if (rigidbody != null)
                    {
                        rigidbody.isKinematic = false;

                        // Use the stored velocity from the preview
                        rigidbody.velocity = storedThrowVelocity;
                        if (lineRenderer.positionCount >= 2)
                        {
                            Vector3 startPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 2);
                            Vector3 endPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);
                            Vector3 rotationDirection = (endPoint - startPoint).normalized;

                            if (heldObject.CompareTag("Glider"))
                            {
                                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                                if (rb != null)
                                {
                                    Vector3 spinForce = new Vector3(-15f, 0, 0);
                                    rb.angularVelocity = spinForce;
                                    heldObject.transform.rotation = Quaternion.LookRotation(rotationDirection);
                                }
                            }
                            else
                            {
                                heldObject.transform.rotation = Quaternion.LookRotation(
                                    new Vector3(rotationDirection.x, -90, rotationDirection.z)
                                );
                            }
                        }
                    }

                    Drop();
                    throwForce = 0f;
                    lineRenderer.positionCount = 0;
                    targetAssetObject.SetActive(false);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    chargingThrow = false;
                    cancelThrow = true;
                    throwForce = 0f;
                    lineRenderer.positionCount = 0;
                    storedThrowDirection = Vector3.zero;
                    targetAssetObject.SetActive(false);
                }
            }
        }

        public List<IPickupable> GetHeldItems()
        {
            return _pickUpsList;
        }


        public void Pickup(Interactable selected, IPickupable pickUp)
        {
            selected.SetOutline(false);
            Transform transform = sockets.GetSockets(pickUp.SocketType);
            _pickUpsList.Add(pickUp);
            pickUp.PickUp(transform);
            nearbyInteractables.Remove(selected);
            // foreach (var interactable in nearbyInteractables)
            // {
            //     Debug.Log("Item: " + interactable.transform.name);
            // }
            ObjThrowAudio = selected.realObject.GetComponent<Pickable>().ObjThrowAudio;
            heldObject = selected.realObject.GetComponent<Rigidbody>();
            UpdateHighlightedInteractable();

            if (!hasPickedUpTrinket && heldObject.CompareTag("Trinket"))
            {
                hasPickedUpTrinket = true;
                ShowTrinketGuide();
            }
            if(heldObject.CompareTag("Soda"))
            {
              AudioManager.Instance?.PlayOneShot(dashActivate);
            }
        }
        

        public void Drop()
        {
            // Check if dropping a trinket to hide guide
            if (heldObject != null && heldObject.CompareTag("Trinket"))
            {
                trinketGuideLine.enabled = false;
            }

            foreach (IPickupable pickUp in _pickUpsList)
            {
                pickUp.Drop(dropPoint.position);
            }

            _pickUpsList.Clear();
            heldObject = null;

            //reset line renderer
            lineRenderer.positionCount = 0;
        }
        
        public void RemoveNullItems()
        {
            //safely loop through items, find ones to remove, then remove them
            List<Interactable> toRemove = new List<Interactable>();

            foreach (var interactable in nearbyInteractables)
            {
                if (interactable == null)
                    toRemove.Add(interactable);
            }

            foreach (var interactable in toRemove)
            {
                nearbyInteractables.Remove(interactable);
            }
        }

        public void ConsumeItem()
        {
            Destroy(heldObject.gameObject);
            Drop();
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

        public void SetCanInput(bool val)
        {
            canInput = val;
        }

        void DrawThrowTrajectory(Vector3 direction)
        {
            
            float chargePercent = throwForce / maxThrowForce;
            float scaledArc = Mathf.Lerp(minArc, maxArc, arcCurve.Evaluate(chargePercent));
            Vector3 curvedDirection = direction;
            curvedDirection.y += scaledArc * 1f;
            curvedDirection = curvedDirection.normalized;
            
            int resolution = 50;
            float timeStep = 0.08f;

            Vector3 startPosition = handPoint.position;
            Vector3 velocity = curvedDirection * throwForce;
            
            // Scale down the velocity for the actual throw
            storedThrowVelocity = velocity * throwPowerScaler; // Try 0.5f, adjust to taste

            lineRenderer.positionCount = resolution;

            for (int i = 0; i < resolution; i++)
            {
                float time = i * timeStep;
                Vector3 point = startPosition + velocity * time + 0.5f * Physics.gravity * 2f * time * time;
                lineRenderer.SetPosition(i, point);
            }

            throwDirection = curvedDirection;
            
            if (!targetAssetObject.activeSelf)
            {
                targetAssetObject.SetActive(true);
            }

            targetAssetObject.transform.position = FindThrowCollisionPoint();

        }

        private Vector3 FindThrowCollisionPoint()
        {
            LayerMask layerMask = LayerMask.GetMask("Ground", "Wall");
            Vector3 collisionPoint = new Vector3(0, 0, 0);
            Vector3 firstPoint = lineRenderer.GetPosition(0);

            for (int i = 0; i < lineRenderer.positionCount - 1; i++) 
            {
                Vector3 currentPoint = lineRenderer.GetPosition(i);
                if (Physics.Linecast(firstPoint, currentPoint, out RaycastHit hit))
                {
                    Physics.Raycast(hit.point, transform.position - hit.point, out RaycastHit inSightCheck);
                    if (inSightCheck.collider.gameObject.CompareTag("Player"))
                    {
                        collisionPoint = hit.point;
                    }
                }
                
                firstPoint = currentPoint;
            }

            return collisionPoint;
        }

        private void Flip(Vector3 faceDirection)
        {
            if (input.magnitude <= 0.1f) return;

            // radians is ccw, unity is cw
            // Raw angle from input -> radians -> degrees
            float rawAngle = -Mathf.Atan2(faceDirection.z, faceDirection.x) * Mathf.Rad2Deg;

            // Snap to 45 degrees
            newFaceAngle = Mathf.Round(rawAngle / 45f) * 45f;

            // Avoid pure front/back facings
            if (newFaceAngle == 90f)  newFaceAngle = 45f;
            if (newFaceAngle == -90f) newFaceAngle = -45f;

            if (Mathf.Abs(Mathf.DeltaAngle(newFaceAngle, faceAngle)) < flipPadding)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.identity;
            if(newFaceAngle == 0f)
            {
                targetRotation = Quaternion.Euler(0, 0, 0);
            }else if(newFaceAngle == 45f)
            {
                targetRotation = Quaternion.Euler(0, 45f, 0);
            }else if(newFaceAngle == 135f)
            {
                targetRotation = Quaternion.Euler(0, 135f, 0);  
            }else if(newFaceAngle == 180f || newFaceAngle == -180f)
            {
                targetRotation = Quaternion.Euler(0, 180f, 0);
            }else if(newFaceAngle == -135f)
            {
                targetRotation = Quaternion.Euler(0, -135f, 0);
            }else if(newFaceAngle == -45f)
            {
                targetRotation = Quaternion.Euler(0, -45f, 0);
            }
            playerSprite.transform.rotation = Quaternion.Slerp(playerSprite.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            faceAngle = Mathf.Round(playerSprite.transform.rotation.eulerAngles.y);
        }

        void OnDrawGizmos()
        {
            //what is the purpose of this wiresphere LOL
            // Gizmos.color = Color.red;
            // Gizmos.DrawWireSphere(transform.position, 2);
            
            // Draw ground check -- is this supposed to show the collider size? doesn't work with a capsule
            if (normalCollider != null)
            {
                Gizmos.color = isGrounded ? Color.green : Color.yellow;
                Vector3 origin = transform.position + Vector3.up * (normalCollider.height * 0.5f);
                Gizmos.DrawWireSphere(origin - Vector3.up * ((normalCollider.height * 0.5f) + groundCheckDistance), normalCollider.radius * 0.9f);
            }
        }

        void SetupTrinketGuideLine()
        {
            GameObject lineObj = new GameObject("TrinketGuideLine");
            lineObj.transform.SetParent(transform);
            trinketGuideLine = lineObj.AddComponent<LineRenderer>();
            
            trinketGuideLine.material = trinketGuideMaterial;
            trinketGuideLine.startWidth = trinketGuideWidth;
            trinketGuideLine.endWidth = trinketGuideWidth;
            if (trinketGuideLine.material != null)
            {
                trinketGuideLine.material.color = trinketGuideColor;
            }
            trinketGuideLine.positionCount = 2;
            trinketGuideLine.useWorldSpace = true;
            trinketGuideLine.enabled = false;
        }

        void ShowTrinketGuide()
        {
            FindNearestWindow();
            if (nearestWindow != null)
            {
                trinketGuideLine.enabled = true;
                UpdateTrinketGuideLine();
            }
        }

        void FindNearestWindow()
        {
            GameObject[] windows = GameObject.FindGameObjectsWithTag("Window");
            if (windows.Length == 0) return;
            
            float closestDistance = Mathf.Infinity;
            foreach (GameObject window in windows)
            {
                float distance = Vector3.Distance(transform.position, window.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    nearestWindow = window.transform;
                }
            }
        }

        void UpdateTrinketGuideLine()
        {
            FindNearestWindow();
            if (nearestWindow != null && trinketGuideLine.enabled)
            {
                Vector3 startPos = transform.position + Vector3.up * 0.5f;
                Vector3 endPos = nearestWindow.position + Vector3.up * 0.5f;
                
                trinketGuideLine.SetPosition(0, startPos);
                trinketGuideLine.SetPosition(1, endPos);
            }
        }
    }
}