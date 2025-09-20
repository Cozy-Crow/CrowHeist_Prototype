using System.Collections;
using UnityEngine;
using FMODUnity;
using KinematicCharacterController.Examples;

public class PixieDust : MonoBehaviour, IPickupable
{
    [Header("Levitation Settings")]
    [SerializeField] private float levitationDuration = 5f;
    [SerializeField] private float levitationForce = 50f;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem useExplosionParticles;
    [SerializeField] private ParticleSystem levitatingTrailParticles;
    [SerializeField] private ParticleSystem floorDustPrefab;
    [SerializeField] private Transform particleSpawnPoint;

    [Header("Audio")]
    [SerializeField] private EventReference shimmerSFX;
    [SerializeField] private EventReference levitationSFX;
    [SerializeField] private EventReference dustEndSFX;

    [Header("Visual Effects")]
    [SerializeField] private Material pixieDustMaterial;
    [SerializeField] private float shaderEffectDuration = 1f;
    [SerializeField] private AnimationClip levitationAnimation;

    [Header("Floor Dust Management")]
    [SerializeField] private int maxFloorDustParticles = 50;
    [SerializeField] private float floorDustLifetime = 60f;

    [Header("Usage Settings")]
    [SerializeField] private float pickupCooldown = 0.5f; // Delay before item can be used after pickup

    private GameObject item;
    private bool isUsed = false;
    private GameObject player;
    private Controller2Point5D playerController;
    //private KinematicCharacterController.Examples.Controller2Point5D playerController;
    private Renderer playerRenderer;
    private Material[] originalMaterials;
    private Animator playerAnimator;

    // Track when the item was picked up to prevent immediate use
    private float pickupTime = -1f;
    private bool wasPickedUp = false;

    // Static list to track floor dust particles across all pixie dust instances
    private static System.Collections.Generic.List<GameObject> floorDustInstances =
        new System.Collections.Generic.List<GameObject>();

    // Audio instances
    private FMOD.Studio.EventInstance shimmerInstance;
    private FMOD.Studio.EventInstance levitationInstance;
    private FMOD.Studio.EventInstance dustEndInstance;

    public GameObject Item => item;

    void Awake()
    {
        item = gameObject;
        gameObject.tag = "PixieDust";

        // Create particle spawn point if not assigned
        if (particleSpawnPoint == null)
        {
            GameObject spawnPoint = new GameObject("ParticleSpawnPoint");
            spawnPoint.transform.SetParent(transform);
            spawnPoint.transform.localPosition = Vector3.zero;
            particleSpawnPoint = spawnPoint.transform;
        }
        
        
        player = GameObject.FindWithTag("Player");
        playerController = player.GetComponent<Controller2Point5D>();

        // Initialize audio instances
        InitializeAudio();
    }

    void Update()
    {
        if (transform.parent != null && !isUsed)
        {
            if (playerController == null)
            {
                playerController = FindObjectOfType<KinematicCharacterController.Examples.Controller2Point5D>();
                if (playerController != null)
                {
                    // Get player components for effects
                    playerRenderer = playerController.GetComponentInChildren<Renderer>();
                    playerAnimator = playerController.GetComponentInChildren<Animator>();

                    if (playerRenderer != null)
                    {
                        originalMaterials = playerRenderer.materials;
                    }
                }
            }

            // Check if enough time has passed since pickup before allowing use
            bool canUse = !wasPickedUp || (Time.time - pickupTime >= pickupCooldown);

            if (Input.GetKeyDown(KeyCode.E) && playerController != null && canUse)
            {
                Debug.Log("Using Enhanced Pixie Dust!");
                Use();
            }
        }
    }

    private void InitializeAudio()
    {
        if (!shimmerSFX.IsNull)
            shimmerInstance = RuntimeManager.CreateInstance(shimmerSFX);
        if (!levitationSFX.IsNull)
            levitationInstance = RuntimeManager.CreateInstance(levitationSFX);
        if (!dustEndSFX.IsNull)
            dustEndInstance = RuntimeManager.CreateInstance(dustEndSFX);
    }

    public void PickUP(Transform parent)
    {
        if (isUsed) return;

        Debug.Log("Picking up Enhanced Pixie Dust");
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Track pickup time to prevent immediate use
        pickupTime = Time.time;
        wasPickedUp = true;
    }

    public void Drop(Vector3 position)
    {
        if (isUsed) return;

        Debug.Log("Dropping Enhanced Pixie Dust");
        transform.SetParent(null);
        transform.position = position;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        // Reset pickup tracking when dropped
        wasPickedUp = false;
        pickupTime = -1f;
    }

    public void Use()
    {
        if (isUsed || playerController == null) return;

        isUsed = true;
        Debug.Log("Starting enhanced pixie dust effect");

        // Play use explosion particles
        PlayUseExplosionEffect();

        // Play shimmer sound
        PlayShimmerSound();

        // Start levitation with all effects
        StartCoroutine(EnhancedLevitation());

        // Create floor dust
        CreateFloorDust();
        
        //Update Crowley
        //playerController.Drop();

        // Destroy the pixie dust item
        //Destroy(gameObject, 0.1f);
    }

    private void PlayUseExplosionEffect()
    {
        if (useExplosionParticles != null)
        {
            ParticleSystem explosion = Instantiate(useExplosionParticles, particleSpawnPoint.position, particleSpawnPoint.rotation);
            explosion.Play();

            // Auto-destroy after playing
            Destroy(explosion.gameObject, explosion.main.duration + explosion.main.startLifetime.constantMax);
        }
    }

    private void PlayShimmerSound()
    {
        if (shimmerInstance.isValid())
        {
            shimmerInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            shimmerInstance.start();
        }
    }

    private void CreateFloorDust()
    {
        if (floorDustPrefab == null) return;

        // Clean up old floor dust if we exceed the limit
        CleanupExcessFloorDust();

        // Create new floor dust at player position
        Vector3 floorPosition = playerController.transform.position;
        floorPosition.y = GetGroundHeight(floorPosition);

        GameObject floorDust = Instantiate(floorDustPrefab.gameObject, floorPosition, Quaternion.identity);
        ParticleSystem floorParticles = floorDust.GetComponent<ParticleSystem>();

        if (floorParticles != null)
        {
            floorParticles.Play();
        }

        // Add to tracking list
        floorDustInstances.Add(floorDust);

        // Auto cleanup after lifetime
        StartCoroutine(CleanupFloorDustAfterDelay(floorDust, floorDustLifetime));
    }

    private void CleanupExcessFloorDust()
    {
        // Remove null references
        floorDustInstances.RemoveAll(dust => dust == null);

        // If we have too many, destroy the oldest ones
        while (floorDustInstances.Count >= maxFloorDustParticles)
        {
            if (floorDustInstances[0] != null)
            {
                Destroy(floorDustInstances[0]);
            }
            floorDustInstances.RemoveAt(0);
        }
    }

    private IEnumerator CleanupFloorDustAfterDelay(GameObject floorDust, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (floorDust != null)
        {
            floorDustInstances.Remove(floorDust);
            Destroy(floorDust);
        }
    }

    private float GetGroundHeight(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 2f, Vector3.down, out hit, 5f))
        {
            return hit.point.y;
        }
        return position.y - 1f; // Fallback
    }

    private IEnumerator EnhancedLevitation()
    {
        float timer = levitationDuration;
        Rigidbody playerRb = playerController.GetComponent<Rigidbody>();

        // Start levitation effects
        StartLevitationEffects();

        Debug.Log($"Starting enhanced levitation - Duration: {levitationDuration}, Force: {levitationForce}");

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            float timeRemaining = timer;

            // Apply levitation force
            if (playerRb != null)
            {
                Vector3 upwardForce = Vector3.up * levitationForce;
                Vector3 antiGravity = -Physics.gravity * playerRb.mass;
                Vector3 totalForce = upwardForce + antiGravity;

                playerRb.AddForce(totalForce, ForceMode.Force);
            }

            // Check if we're running low on time for end warning
            if (timeRemaining <= 1f && timeRemaining > 0.9f)
            {
                PlayDustEndSound();
            }

            yield return null;
        }

        // End all levitation effects
        EndLevitationEffects();

        Debug.Log("Enhanced levitation effect complete");
    }

    private void StartLevitationEffects()
    {
        // Start levitation trail particles
        if (levitatingTrailParticles != null)
        {
            levitatingTrailParticles.transform.SetParent(playerController.transform);
            levitatingTrailParticles.transform.localPosition = Vector3.zero;
            levitatingTrailParticles.Play();
        }

        // Apply pixie dust shader effect
        ApplyPixieDustShader();

        // Set levitation animation
        SetLevitationAnimation();

        // Play levitation sound
        PlayLevitationSound();
    }

    private void EndLevitationEffects()
    {
        // Stop trail particles
        if (levitatingTrailParticles != null)
        {
            levitatingTrailParticles.Stop();
            levitatingTrailParticles.transform.SetParent(null);
        }

        // Remove pixie dust shader effect
        RemovePixieDustShader();

        // Reset animation
        ResetPlayerAnimation();

        // Stop levitation sound
        StopLevitationSound();
    }

    private void ApplyPixieDustShader()
    {
        if (playerRenderer != null && pixieDustMaterial != null)
        {
            StartCoroutine(ApplyShaderEffect());
        }
    }

    private IEnumerator ApplyShaderEffect()
    {
        if (playerRenderer == null || pixieDustMaterial == null) yield break;

        // Create a copy of the pixie dust material for this instance
        Material dustMaterial = new Material(pixieDustMaterial);

        // Apply the shader effect (blend with original materials)
        Material[] newMaterials = new Material[originalMaterials.Length + 1];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = originalMaterials[i];
        }
        newMaterials[originalMaterials.Length] = dustMaterial;

        playerRenderer.materials = newMaterials;

        yield return new WaitForSeconds(levitationDuration);

        // The effect will be removed by RemovePixieDustShader()
    }

    private void RemovePixieDustShader()
    {
        if (playerRenderer != null && originalMaterials != null)
        {
            playerRenderer.materials = originalMaterials;
        }
    }

    private void SetLevitationAnimation()
    {
        if (playerAnimator != null && levitationAnimation != null)
        {
            // Override the current animation with the levitation pose
            playerAnimator.Play("LevitationPose", 0, 0f);
        }
    }

    private void ResetPlayerAnimation()
    {
        if (playerAnimator != null)
        {
            // Let the player controller handle normal animations again
            // This will naturally transition back to appropriate animations
        }
    }

    private void PlayLevitationSound()
    {
        if (levitationInstance.isValid())
        {
            levitationInstance.set3DAttributes(RuntimeUtils.To3DAttributes(playerController.transform.position));
            levitationInstance.start();
        }
    }

    private void StopLevitationSound()
    {
        if (levitationInstance.isValid())
        {
            levitationInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void PlayDustEndSound()
    {
        if (dustEndInstance.isValid())
        {
            dustEndInstance.set3DAttributes(RuntimeUtils.To3DAttributes(playerController.transform.position));
            dustEndInstance.start();
        }
    }

    private void OnDestroy()
    {
        // Clean up audio instances
        if (shimmerInstance.isValid())
            shimmerInstance.release();
        if (levitationInstance.isValid())
            levitationInstance.release();
        if (dustEndInstance.isValid())
            dustEndInstance.release();
    }

    // Static method to clear all floor dust (useful for level transitions)
    public static void ClearAllFloorDust()
    {
        foreach (GameObject dust in floorDustInstances)
        {
            if (dust != null)
            {
                Destroy(dust);
            }
        }
        floorDustInstances.Clear();
    }
}