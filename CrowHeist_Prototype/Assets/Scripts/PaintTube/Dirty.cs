using UnityEngine;

public class DirtyCharacterController : MonoBehaviour
{
    public Renderer targetRenderer;
    public float dirtAmount = 0f;
    public float changeSpeed = 0.3f;
    public Texture2D dirtMask;
    public Material baseMaterial;

    Material runtimeMaterial;

    void Start()
    {
        if (targetRenderer != null)
        {
            runtimeMaterial = Instantiate(baseMaterial != null ? baseMaterial : targetRenderer.material);
            targetRenderer.material = runtimeMaterial;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            dirtAmount += changeSpeed * Time.deltaTime;
            if (dirtAmount > 1f) dirtAmount = 1f;
            UpdateDirt();
        }

        if (Input.GetKey(KeyCode.C))
        {
            dirtAmount -= changeSpeed * Time.deltaTime;
            if (dirtAmount < 0f) dirtAmount = 0f;
            UpdateDirt();
        }
    }

    void UpdateDirt()
    {
        if (runtimeMaterial == null) return;

        // to be finished
        runtimeMaterial.SetFloat("_DirtStrength", dirtAmount);

        if (dirtMask != null)
            runtimeMaterial.SetTexture("_DirtMask", dirtMask);
    }
}
