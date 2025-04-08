using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineFadeController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float fadeStartDistance = 5f;  // Distance where fading starts
    public float fadeEndDistance = 10f;   // Distance where it fully fades

    void Update()
    {
        if (lineRenderer.positionCount < 2) return;

        // Get the total distance of the line
        float totalDistance = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1));

        // Create a new gradient
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];

        colorKeys[0] = new GradientColorKey(Color.white, 0f); // Start of line
        colorKeys[1] = new GradientColorKey(Color.white, 1f); // End of line

        // Calculate fading based on distance
        float fadeFactor = Mathf.InverseLerp(fadeStartDistance, fadeEndDistance, totalDistance);
        alphaKeys[0] = new GradientAlphaKey(1f, 0f); // Fully visible at start
        alphaKeys[1] = new GradientAlphaKey(1f - fadeFactor, 1f); // Fade out at the end

        // Apply gradient
        gradient.SetKeys(colorKeys, alphaKeys);
        lineRenderer.colorGradient = gradient;
    }
}
