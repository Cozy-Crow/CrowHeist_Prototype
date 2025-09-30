using UnityEngine;

public class Shining : MonoBehaviour
{
    [Header("Particle Appearance")]
    [SerializeField] private Color shineColor = new Color(1f, 0.92f, 0.016f, 1f); // Bright yellow
    [SerializeField] private float particleSize = 0.1f;
    [SerializeField] private float emissionRate = 20f;
    [SerializeField] private float particleLifetime = 1f;
    [SerializeField] private float particleSpeed = 0.5f;

    [Header("Orbit Settings")]
    [SerializeField] private float orbitRadius = 0.5f;
    [SerializeField] private float orbitSpeed = 1f;

    private ParticleSystem shineParticles;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.ShapeModule shapeModule;
    private ParticleSystem.ColorOverLifetimeModule colorModule;
    private bool isPlayerInRange = false;
    private Interactable interactable;

    private void Awake()
    {
        // Get reference to Interactable component
        interactable = GetComponentInParent<Interactable>();
        if (interactable == null)
        {
            Debug.LogError($"Shining script on {gameObject.name} requires an Interactable component on parent!");
            enabled = false;
            return;
        }

        // Create and configure particle system
        CreateShineParticles();
        ConfigureParticles();
        shineParticles.Stop(); // Start with particles off
    }

    private void Update()
    {
        // Check if player is in range using Interactable component
        if (interactable != null)
        {
            bool wasInRange = isPlayerInRange;
            isPlayerInRange = interactable.Interact;

            // Handle state changes
            if (isPlayerInRange && !wasInRange)
            {
                if (!shineParticles.isPlaying)
                {
                    shineParticles.Play();
                }
            }
            else if (!isPlayerInRange && wasInRange)
            {
                if (shineParticles.isPlaying)
                {
                    shineParticles.Stop();
                }
            }
        }
    }

    private void CreateShineParticles()
    {
        // Create a new GameObject for the particle system
        GameObject particleObj = new GameObject("ShineParticles");
        particleObj.transform.SetParent(transform);
        particleObj.transform.localPosition = Vector3.zero;
        particleObj.transform.localRotation = Quaternion.identity;

        // Add and configure particle system
        shineParticles = particleObj.AddComponent<ParticleSystem>();

        // Get the renderer for material settings
        ParticleSystemRenderer renderer = particleObj.GetComponent<ParticleSystemRenderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
        }
    }

    private void ConfigureParticles()
    {
        if (shineParticles == null) return;

        // Main module
        mainModule = shineParticles.main;
        mainModule.startLifetime = particleLifetime;
        mainModule.startSpeed = particleSpeed;
        mainModule.startSize = particleSize;
        mainModule.startColor = shineColor;
        mainModule.loop = true;
        mainModule.playOnAwake = false;
        mainModule.maxParticles = 100;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        // Emission module
        emissionModule = shineParticles.emission;
        emissionModule.rateOverTime = emissionRate;

        // Shape module - create a sphere around the object
        shapeModule = shineParticles.shape;
        shapeModule.enabled = true;
        shapeModule.shapeType = ParticleSystemShapeType.Sphere;
        shapeModule.radius = orbitRadius;
        shapeModule.radiusThickness = 1f; // Emit from surface of sphere

        // Color over lifetime for fade effect
        colorModule = shineParticles.colorOverLifetime;
        colorModule.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(shineColor, 0f),
                new GradientColorKey(shineColor, 0.5f),
                new GradientColorKey(shineColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),      // Fade in
                new GradientAlphaKey(1f, 0.2f),    // Full opacity
                new GradientAlphaKey(1f, 0.8f),    // Stay visible
                new GradientAlphaKey(0f, 1f)       // Fade out
            }
        );
        colorModule.color = gradient;

        // Size over lifetime - particles grow slightly
        var sizeModule = shineParticles.sizeOverLifetime;
        sizeModule.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 0.5f);
        sizeCurve.AddKey(0.5f, 1f);
        sizeCurve.AddKey(1f, 0.5f);
        sizeModule.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Velocity over lifetime for orbital motion
        var velocityModule = shineParticles.velocityOverLifetime;
        velocityModule.enabled = true;
        velocityModule.space = ParticleSystemSimulationSpace.World;

        // Create orbital motion by using velocity curves
        AnimationCurve orbitCurve = AnimationCurve.Linear(0f, orbitSpeed, 1f, orbitSpeed);
        velocityModule.orbitalX = new ParticleSystem.MinMaxCurve(orbitSpeed, orbitCurve);
        velocityModule.orbitalY = new ParticleSystem.MinMaxCurve(orbitSpeed * 0.5f, orbitCurve);
        velocityModule.orbitalZ = new ParticleSystem.MinMaxCurve(orbitSpeed * 0.3f, orbitCurve);
    }

    private void OnDisable()
    {
        // Make sure particles stop when object is disabled
        if (shineParticles != null && shineParticles.isPlaying)
        {
            shineParticles.Stop();
        }
    }

    private void OnDestroy()
    {
        // Clean up created particles
        if (shineParticles != null)
        {
            Destroy(shineParticles.gameObject);
        }
    }
}
