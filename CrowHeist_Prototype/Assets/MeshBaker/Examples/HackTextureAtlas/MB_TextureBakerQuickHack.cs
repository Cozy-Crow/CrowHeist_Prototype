using System.Collections.Generic;
using System.Text;
using DigitalOpus.MB.Core;
using UnityEngine;

namespace MeshBaker_Examples_2017.HackTextureAtlas
{
    /// <summary>
    /// Sometimes we want to create a custom texture atlas without using the TextureBaker. Examples:
    /// 
    /// - We have some solid colors that we want in an atlas.
    /// - The texture baker has a lot of features which makes it slow at runtime.
    /// 
    /// This script creates an atlas, combinedMaterial, Material Bake Result without using the TextureBaker.
    /// </summary>
    public class MB_TextureBakerQuickHack : MonoBehaviour
    {
        [Header("Hack Atlas Generation")]
        public string colorTintPropertyName;
        public string albedoTexturePropertyName;
        public string shaderName;
        public Material[] sourceMaterials;


        [Space(20)]
        [Header("Generated Output")]
        // materialBakeResult can't be modified by users, but we want to see it in the inspector.
        public MB2_TextureBakeResults materialBakeResult;
        public Material atlasMaterial;
        public Texture2D atlasTexture;
    
    
        [ContextMenu("Generate Material Bake Result")]
        public void CreateAtlas(Material[] passedInSourceMaterials)
        {
            // Validation of source materials
            Debug.Log("Validating source materials");
            {
                bool doProceed = true;
                {
                    // passedInSourceMaterials must be unique
                    HashSet<Material> uniqueMaterialsChecker = new HashSet<Material>(passedInSourceMaterials);
                    if (uniqueMaterialsChecker.Count != passedInSourceMaterials.Length)
                    {
                        Debug.LogError("Source materials are not unique");
                        doProceed = false;
                    }
                
                    // there must be at least one source material
                    if (passedInSourceMaterials.Length == 0)
                    {
                        Debug.LogError("No source materials were passed in");
                        doProceed = false;
                    }
                
                    // colorTintProperty must be a valid string
                    if (string.IsNullOrEmpty(colorTintPropertyName))
                    {
                        Debug.LogError("ColorTintProperty is not set");
                        doProceed = false;
                    }
                }
            
                // copy passed in source materials to sourceMaterials
                sourceMaterials = new Material[passedInSourceMaterials.Length];
                for (int i = 0; i < passedInSourceMaterials.Length; i++)
                {
                    // validate each passed in source material
                    {
                        // all passedInSourceMaterials must exist
                        if (passedInSourceMaterials[i] == null)
                        {
                            Debug.LogError("Source material " + i + " is null");
                            doProceed = false;
                        }
                
                        // all passedInSourceMaterials must have colorTintProperty
                        if (!passedInSourceMaterials[i].HasProperty(colorTintPropertyName))
                        {
                            Debug.LogError("Source material " + i + " does not have the colorTint property");
                            doProceed = false;
                        }
                    }
                
                    sourceMaterials[i] = passedInSourceMaterials[i];
                    sourceMaterials[i].shader = Shader.Find(shaderName);
                }

                if (!doProceed)
                {
                    Debug.LogError("Some validation of the source materials failed and the atlas was not generated.");
                    return;
                }
            }

            int padding = 2;
            int colorBlockSize = 8;
            bool isProjectLinear = MBVersion.GetProjectColorSpace() == ColorSpace.Linear;

            // Visit each source material and generate a solid color texture matching the color tint.
            // Building a string for debug output at the end of this function
            Texture2D[] solidColorTextures = new Texture2D[sourceMaterials.Length];
            {
                StringBuilder colorTintsFromSourceMaterials = new StringBuilder("Collecting color tints from source materials: \n");
                Color[] colorsToSet = new Color[colorBlockSize * colorBlockSize];
                for (int matIdx = 0; matIdx < sourceMaterials.Length; matIdx++)
                {
                    // Get the color tint
                    Material m = sourceMaterials[matIdx];
                    Debug.Assert(m.HasProperty(colorTintPropertyName), "Material was missing the colorTint property");
                    Color colorTint = m.GetColor(colorTintPropertyName);
                    colorTintsFromSourceMaterials.Append("Material: " + m.name + " - colorTint: " + colorTint + "\n");

                    // Generate a small solid color block texture
                    Texture2D tex = solidColorTextures[matIdx] = new Texture2D(colorBlockSize, colorBlockSize, TextureFormat.ARGB32, false, isProjectLinear);
                    for (int cIdx = 0; cIdx < colorsToSet.Length; cIdx++)
                    {
                        colorsToSet[cIdx] = colorTint;
                    }

                    tex.SetPixels(colorsToSet);
                    tex.Apply();
                }
                Debug.Log(colorTintsFromSourceMaterials);
            }

            // Calculate the atlas dimensions
            Debug.Log("Calculating the atlas dimensions");
            int atlasSize_pixels;
            {
                float numTexPerRow = Mathf.Ceil(Mathf.Sqrt(sourceMaterials.Length));
                atlasSize_pixels = (int) numTexPerRow * colorBlockSize;
            }
        
            // creating atlas for sourceMaterials.Length textures.
            Debug.Log("Creating atlas for " + sourceMaterials.Length + " textures");
            Rect[] atlasRects;
            {
                // Create the atlas
                atlasTexture = new Texture2D(atlasSize_pixels, atlasSize_pixels, TextureFormat.ARGB32, false, isProjectLinear);
            
                {
                    // Pass in padding 0. We will subtract some area from the edges of the rectangles to create padding
                    // We do it this way so that the padding is filled with block color not black.
                    atlasRects = atlasTexture.PackTextures(solidColorTextures, 0, atlasSize_pixels);
                }

                Debug.Log("Atlas size: w:" + atlasTexture.width + "  h:" + atlasTexture.height + "  numTex: " + solidColorTextures.Length + " (" + colorBlockSize + "x" + colorBlockSize + " each)");

                atlasTexture.filterMode = FilterMode.Point;
            }

            // Generate a combined material
            atlasMaterial = new Material(Shader.Find(shaderName)); // has to be different based on default, URP, HDRP
            atlasMaterial.SetTexture(albedoTexturePropertyName, atlasTexture); // has to be different based on default, URP, HDRP
            atlasMaterial.SetColor(colorTintPropertyName, Color.white);
        
            {
                StringBuilder atlasRectanglesInformation = new StringBuilder("Creating MB2_TextureBakeResult for storing atlas rectangle information: \n");
                for (int i = 0; i < solidColorTextures.Length; i++)
                {
                    atlasRectanglesInformation.Append("Material: " + sourceMaterials[i].name + " will use rectangle: " + atlasRects[i] + "\n");
                }
        
                Debug.Log(atlasRectanglesInformation);
            }

            // Create and setup the material bake result
            Debug.Log("Creating and setting up MB2_TextureBakeResults");
            {
                materialBakeResult = ScriptableObject.CreateInstance<MB2_TextureBakeResults>();
                materialBakeResult.resultType = MB2_TextureBakeResults.ResultType.atlas;
                materialBakeResult.materialsAndUVRects = new MB_MaterialAndUVRect[solidColorTextures.Length];
                float paddingWidth = ((float) padding) / atlasTexture.width;
                float paddingHeight = ((float)padding) / atlasTexture.height;
                for (int i = 0; i < solidColorTextures.Length; i++)
                {
                    Rect rectangleInAtlas = atlasRects[i];
                    {
                        // Pad the rectangle in the atlas.
                        // We packed the atlas with padding 0. Now we shrink the rectangles to create
                        // padding around the edges of each rectangle that is filled with each rectangles color.
                        rectangleInAtlas.x += paddingWidth;
                        rectangleInAtlas.y += paddingHeight;
                        rectangleInAtlas.width -= 2f * paddingWidth;
                        rectangleInAtlas.height -= 2f * paddingHeight;
                    }

                    // See the MB_MaterialAndUVRect for explanations of these parameters.
                    bool allPropsUseSameTiling = true;
                    materialBakeResult.materialsAndUVRects[i] = new MB_MaterialAndUVRect(
                        sourceMaterials[i],
                        rectangleInAtlas,
                        allPropsUseSameTiling,
                        new Rect(0, 0, 1, 1),
                        new Rect(0, 0, 1, 1),
                        new Rect(0, 0, 0, 0),
                        MB_TextureTilingTreatment.none,
                        sourceMaterials[i].name
                    );
                }

                materialBakeResult.resultMaterials = new MB_MultiMaterial[1];
                materialBakeResult.resultMaterials[0] = new MB_MultiMaterial();
                materialBakeResult.resultMaterials[0].combinedMaterial = atlasMaterial;
                materialBakeResult.resultMaterials[0].considerMeshUVs = false;

                List<Material> smats = new List<Material>();
                smats.AddRange(sourceMaterials);
                materialBakeResult.resultMaterials[0].sourceMaterials = smats;
            }
        }
    }
}
