using System.Collections.Generic;
using UnityEngine;

public class PixieDustFloorManager : MonoBehaviour
{
    [Header("Floor Dust Settings")]
    [SerializeField] private int maxFloorDustInstances = 10;
    [SerializeField] private float dustLifetime = 60f; // How long dust stays on floor
    [SerializeField] private float minDistanceBetweenDust = 2f;

    private List<GameObject> floorDustInstances = new List<GameObject>();
    private List<float> dustCreationTimes = new List<float>();

    void Update()
    {
        CleanupOldDust();
    }

    public void AddPixieDustToFloor(Vector3 position, GameObject dustParticlePrefab)
    {
        // Check if there's already dust too close to this position
        if (IsTooCloseToExistingDust(position))
        {
            return;
        }

        // Remove oldest dust if we're at the limit
        if (floorDustInstances.Count >= maxFloorDustInstances)
        {
            RemoveOldestDust();
        }

        // Create new dust on the floor
        GameObject floorDust = Instantiate(dustParticlePrefab, position, Quaternion.identity);

        // Make sure it doesn't move
        Rigidbody dustRb = floorDust.GetComponent<Rigidbody>();
        if (dustRb != null)
        {
            dustRb.isKinematic = true;
        }

        // Adjust particle system for floor effect
        ParticleSystem dustPS = floorDust.GetComponent<ParticleSystem>();
        if (dustPS != null)
        {
            var main = dustPS.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.startLifetime = dustLifetime;

            var emission = dustPS.emission;
            emission.rateOverTime = 0; // No continuous emission
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 50) // One burst at the start
            });

            var velocityOverLifetime = dustPS.velocityOverLifetime;
            velocityOverLifetime.enabled = false; // Stop movement after landing

            var shape = dustPS.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1f;
        }

        floorDustInstances.Add(floorDust);
        dustCreationTimes.Add(Time.time);
    }

    private bool IsTooCloseToExistingDust(Vector3 position)
    {
        foreach (GameObject dust in floorDustInstances)
        {
            if (dust != null && Vector3.Distance(dust.transform.position, position) < minDistanceBetweenDust)
            {
                return true;
            }
        }
        return false;
    }

    private void RemoveOldestDust()
    {
        if (floorDustInstances.Count > 0)
        {
            GameObject oldestDust = floorDustInstances[0];
            if (oldestDust != null)
            {
                Destroy(oldestDust);
            }
            floorDustInstances.RemoveAt(0);
            dustCreationTimes.RemoveAt(0);
        }
    }

    private void CleanupOldDust()
    {
        for (int i = floorDustInstances.Count - 1; i >= 0; i--)
        {
            if (floorDustInstances[i] == null ||
                Time.time - dustCreationTimes[i] > dustLifetime)
            {
                if (floorDustInstances[i] != null)
                {
                    Destroy(floorDustInstances[i]);
                }
                floorDustInstances.RemoveAt(i);
                dustCreationTimes.RemoveAt(i);
            }
        }
    }

    // Public method to clear all dust 
    public void ClearAllDust()
    {
        foreach (GameObject dust in floorDustInstances)
        {
            if (dust != null)
            {
                Destroy(dust);
            }
        }
        floorDustInstances.Clear();
        dustCreationTimes.Clear();
    }
}
