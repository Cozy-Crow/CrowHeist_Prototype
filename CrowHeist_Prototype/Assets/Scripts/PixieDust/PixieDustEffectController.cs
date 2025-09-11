using System.Collections;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PixieDustEffectController : MonoBehaviour
{
    private KinematicCharacterController.Examples.Controller2Point5D playerController;
    private Rigidbody playerRigidbody;
    private Renderer playerRenderer;
    private Material originalMaterial;
    private Material pixieDustMaterial;

    private float levitationDuration;
    private float levitationForce;
    private float timer;
    private bool isLevitating;

    private GameObject particleEffect;
    private GameObject levitationParticles;
    private GameObject floorParticles;

    private EventInstance shimmerSoundInstance;
    private EventInstance levitationSoundInstance;
    private EventInstance endSoundInstance;

    private EventReference shimmerSound;
    private EventReference levitationSound;
    private EventReference endSound;

    private PixieDustFloorManager floorManager;
    private bool effectStarted = false;

    public void Initialize(float duration, float force, GameObject particlePrefab, GameObject levitationParticlePrefab,
                          Material shaderMaterial, EventReference shimmer, EventReference levitation, EventReference end,
                          PixieDustFloorManager manager)
    {
        levitationDuration = duration;
        levitationForce = force;
        pixieDustMaterial = shaderMaterial;
        shimmerSound = shimmer;
        levitationSound = levitation;
        endSound = end;
        floorManager = manager;

        playerController = GetComponentInParent<KinematicCharacterController.Examples.Controller2Point5D>();
        playerRigidbody = playerController.GetComponent<Rigidbody>();

        // Get the player's renderer (assuming it's in a child object)
        playerRenderer = GetComponentInChildren<Renderer>();
        if (playerRenderer == null)
        {
            playerRenderer = playerController.GetComponentInChildren<Renderer>();
        }

        if (playerRenderer != null)
        {
            originalMaterial = playerRenderer.material;
        }

        // Create initial particle explosion
        if (particlePrefab != null)
        {
            particleEffect = Instantiate(particlePrefab, transform.position, Quaternion.identity);

            // Create floor particles that fall down
            CreateFloorParticles(particlePrefab);
        }

        // Create levitation particles
        if (levitationParticlePrefab != null)
        {
            levitationParticles = Instantiate(levitationParticlePrefab, transform);
            levitationParticles.transform.localPosition = Vector3.zero;
        }

        StartCoroutine(PixieDustEffect());
    }

    private void CreateFloorParticles(GameObject particlePrefab)
    {
        if (floorManager != null)
        {
            Vector3 floorPosition = transform.position;
            floorPosition.y = 0; // Assume ground is at y=0, adjust as needed

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f))
            {
                floorPosition = hit.point;
            }

            floorManager.AddPixieDustToFloor(floorPosition, particlePrefab);
        }
    }

    private IEnumerator PixieDustEffect()
    {
        effectStarted = true;
        timer = levitationDuration;
        isLevitating = true;

        // Play shimmer sound
        if (AudioManager.Instance != null)
        {
            shimmerSoundInstance = AudioManager.Instance.CreateInstance(shimmerSound);
            shimmerSoundInstance.start();
        }

        // Apply pixie dust shader
        if (playerRenderer != null && pixieDustMaterial != null)
        {
            playerRenderer.material = pixieDustMaterial;
        }

        // Start levitation sound
        if (AudioManager.Instance != null)
        {
            levitationSoundInstance = AudioManager.Instance.CreateInstance(levitationSound);
            levitationSoundInstance.start();
        }

        // Main levitation loop
        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // Apply levitation force
            if (playerRigidbody != null)
            {
                // Counter gravity and add levitation force
                Vector3 antiGravity = -Physics.gravity * playerRigidbody.mass;
                Vector3 levitationForceVector = Vector3.up * levitationForce;
                playerRigidbody.AddForce(antiGravity + levitationForceVector, ForceMode.Force);
            }

            // Update levitation particles
            if (levitationParticles != null)
            {
                // Make sure particles follow player
                levitationParticles.transform.position = transform.position;
            }

            yield return null;
        }

        // Effect ending
        isLevitating = false;
        Debug.Log("Pixie Dust effect ending");

        // Play end sound (skip if AudioManager is null to avoid errors)
        if (AudioManager.Instance != null && !endSound.IsNull)
        {
            try
            {
                endSoundInstance = AudioManager.Instance.CreateInstance(endSound);
                endSoundInstance.start();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to play end sound: " + e.Message);
            }
        }

        // Stop levitation sound
        if (levitationSoundInstance.isValid())
        {
            levitationSoundInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        // Restore original material
        if (playerRenderer != null && originalMaterial != null)
        {
            playerRenderer.material = originalMaterial;
            Debug.Log("Restored original material");
        }

        // Clean up particles
        if (levitationParticles != null)
        {
            Destroy(levitationParticles, 2f); // Give time for particles to fade out
        }

        if (particleEffect != null)
        {
            Destroy(particleEffect, 5f); // Clean up after a delay
        }

        // Clean up this controller
        Destroy(gameObject, 3f);
    }

    void OnDestroy()
    {
        // Clean up audio instances
        if (shimmerSoundInstance.isValid())
        {
            shimmerSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            shimmerSoundInstance.release();
        }

        if (levitationSoundInstance.isValid())
        {
            levitationSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            levitationSoundInstance.release();
        }

        if (endSoundInstance.isValid())
        {
            endSoundInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            endSoundInstance.release();
        }
    }

    // Public getter for animation system if needed
    public bool IsLevitating => isLevitating;
}