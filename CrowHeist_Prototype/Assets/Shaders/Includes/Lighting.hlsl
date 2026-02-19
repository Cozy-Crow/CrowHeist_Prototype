#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

struct CustomLightData
{
    float ambientOcclusion;
    float3 albedo;
    float3 bakedGI;
    float3 normal;
};

#if !defined(SHADERGRAPH_PREVIEW)
float3 CustomGlobalIllumination(CustomLightData d)
{
    float3 indirectDiffuse = d.albedo * d.bakedGI * d.ambientOcclusion;
    
    return indirectDiffuse;
}


void CalculateMainLight_float(float3 WorldPos, float AmbientOcclusion,float2 LightmapUV, float3 Normal,  out float3 Direction, out float3 Color, out half DistanceAtten, out half ShadowAtten){


    CustomLightData d;
    d.ambientOcclusion = AmbientOcclusion;
#if defined(SHADERGRAPH_PREVIEW)
    Direction = float3(0.5, 0.5, 0);
    Normal = float3(0,0,0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
    d.bakedGI = 0;
    
#else 
    #if defined(SHADOWS_SCREEN)
        half4 clipPos = TransformWorldToHClip(WorldPos);
        half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    #endif
    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
    
    float3 lightmapUV;
    OUTPUT_LIGHTMAP_UV(LightmapUV, unity_LightmapST, lightmapUV);
    float3 vertexSH;
    OUTPUT_SH(Normal, vertexSH);
    d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);
#endif
    
   
}

void AddAdditionalLights_float(float Smoothness, float3 WorldPosition, float3 WorldNormal, float3 WorldView, float MainDiffuse, float MainSpecular, float3 MainColor, out float Diffuse, out float Specular, out float3 Color)
{
    Diffuse = MainDiffuse;
    Specular = MainSpecular;
    Color = MainColor * (MainDiffuse + MainSpecular);
    
#if !defined(SHADERGRAPH_PREVIEW)
    int pixelLightCount = GetAdditionalLightsCount();
    
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half atten = light.distanceAttenuation * light.shadowAttenuation;
        half thisDiffuse = atten * NdotL;
        half thisSpecular = LightingSpecular(thisDiffuse, light.direction, WorldNormal, WorldView, 1, Smoothness);
        
        Diffuse += thisDiffuse;
        Specular += thisSpecular;
        Color += light.color * (thisDiffuse + thisSpecular);
    }
#endif
    half total = Diffuse + Specular;
    Color = total <= 0 ? MainColor : Color / total;
}
#endif