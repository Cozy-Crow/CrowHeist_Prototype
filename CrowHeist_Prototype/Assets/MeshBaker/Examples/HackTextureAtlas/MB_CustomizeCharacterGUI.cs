using System.IO;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;
using UnityEngine.Assertions;
using DigitalOpus.MB;

namespace MeshBaker_Examples_2017.HackTextureAtlas
{
    /// <summary>
    /// The MB_CustomizeCharacterGUI class is a simple example implementation that shows how
    /// the MB_TextureBakerQuickHack can be used to customize a character's texture without
    /// using the TextureBaker, which is more robust but slower.
    ///
    /// The reasoning for this is because the regular TextureBaker is too slow to use at runtime
    /// without lag spikes.
    /// </summary>
    public class MB_CustomizeCharacterGUI : MonoBehaviour
    {
        public Material[] sourceMaterials;
        public GameObject[] objectsToBeCombined;

        [Header("Mesh Baker Config")]
        public MB3_MeshBaker targetMeshBaker;

        private MB_TextureBakerQuickHack textureBakerQuickHack;

        private string colorTintPropertyName;
        private string albedoTexturePropertyName;
        private string shaderName;

        // Start is called before the first frame update
        void Start()
        {
            // Detect the current pipeline and set the shader names accordingly
            switch (MBVersion.DetectPipeline())
            {
                case MBVersion.PipelineType.Default:
                {
                    colorTintPropertyName = "_Color"; 
                    albedoTexturePropertyName = "_MainTex"; 
                    shaderName = "Standard";
                    break;
                }
                case MBVersion.PipelineType.URP:
                {
                    colorTintPropertyName = "_BaseColor"; 
                    albedoTexturePropertyName = "_BaseMap"; 
                    shaderName = "Universal Render Pipeline/Lit";
                    break;
                }
                case MBVersion.PipelineType.HDRP:
                {
                    colorTintPropertyName = "_BaseColor"; 
                    albedoTexturePropertyName = "_BaseColorMap"; 
                    shaderName = "HDRP/Lit";
                    break;
                }
                default:
                {
                    Debug.LogError("Unknown pipeline type");
                    break;
                }
            }

            textureBakerQuickHack = GetComponent<MB_TextureBakerQuickHack>();
            Debug.Log("Creating atlas using TextureBakerQuickHack method");
            {
                textureBakerQuickHack.colorTintPropertyName = colorTintPropertyName;
                textureBakerQuickHack.albedoTexturePropertyName = albedoTexturePropertyName;
                textureBakerQuickHack.shaderName = shaderName;
            }
            textureBakerQuickHack.CreateAtlas(sourceMaterials); // Generate a custom texture atlas without using the TextureBaker
            Debug.Log("Baking MeshBaker using TextureBakerQuickHack output");
            BakeMeshBaker(); // This is the function that bakes the combined mesh
        }

        // OnGUI is called for rendering and handling GUI events
        void OnGUI()
        {
            string textureBakerQuickHackInfo =
                "This example demonstrates how to create\r\n" +
                "solid-color-rectangle texture atlases at\r\n" +
                "runtime for character customization. This\r\n" +
                "is MUCH faster and more flexible than using\r\n" +
                "the full TextureBaker. These atlases can be\r\n" +
                "used at runtime with a Mesh Baker.\r\n";
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(textureBakerQuickHackInfo.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Hoof Color");
            if (GUILayout.Button("Red"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[0], Color.red);
            }
            if (GUILayout.Button("Green"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[0], Color.green);
            }
            if (GUILayout.Button("Blue"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[0], Color.blue);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Body Color");
            if (GUILayout.Button("Red"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[1], Color.red);
            }
            if (GUILayout.Button("Green"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[1], Color.green);
            }
            if (GUILayout.Button("Blue"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[1], Color.blue);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Horns Color");
            if (GUILayout.Button("Red"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[2], Color.red);
            }
            if (GUILayout.Button("Green"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[2], Color.green);
            }
            if (GUILayout.Button("Blue"))
            {
                SetColorInMaterialBakeResultAndBakeMeshBaker(sourceMaterials[2], Color.blue);
            }
            GUILayout.EndHorizontal();
        }

        private void SetColorInMaterialBakeResultAndBakeMeshBaker(Material bodyPartMaterial, Color color)
        {
            Debug.Log("Changing color of material " + bodyPartMaterial + " used in atlas generation");
            bodyPartMaterial.SetColor(colorTintPropertyName, color);
            Debug.Log("Creating atlas using TextureBakerQuickHack method");
            textureBakerQuickHack.CreateAtlas(sourceMaterials);
            Debug.Log("Baking MeshBaker using TextureBakerQuickHack output");
            BakeMeshBaker();
        }

        [ContextMenu("Bake Mesh Baker")]
        private void BakeMeshBaker()
        {
            Assert.IsNotNull(targetMeshBaker, "targetMeshBaker is null. Make sure one is assigned in the inspector.");
            targetMeshBaker.textureBakeResults = textureBakerQuickHack.materialBakeResult;
            targetMeshBaker.ClearMesh();
            if (targetMeshBaker.AddDeleteGameObjects(objectsToBeCombined, null, true))
            {
                targetMeshBaker.Apply();
            }
        }
    }
}
