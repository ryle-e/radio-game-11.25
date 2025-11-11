#define ADDITIONAL_LIGHT_INCLUDED

void MainLight_half(
    float3 WorldPos, 
    out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten
)
{
#if SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else

    Light mainLight = GetMainLight();

    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;

    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    half shadowStrength = GetMainLightShadowStrength();
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();

    ShadowAtten = SampleShadowmap(
        shadowCoord, 
        TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
        shadowSamplingData,
        shadowStrength,
        false);

#endif
}

void MainLight_float(float3 WorldPos, 
out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
    MainLight_half(WorldPos, Direction, Color, DistanceAtten, ShadowAtten);
}

void AdditionalLights_float(
    float3 SurfaceColor, float3 WorldPos, float3 WorldNormal, float2 CutoffThresholds, float3 ViewDir, float Metallic, float Smoothness,
    out float3 DiffuseLight, out float3 SpecularLight
)
{
    DiffuseLight = 0;
    SpecularLight = 0;

#if !SHADERGRAPH_PREVIEW
    int lightCount = GetAdditionalLightsCount();

    for (int i = 0; i < lightCount; i++)
    {
        Light light = GetAdditionalLight(i, WorldPos);

        //float D = dot(ViewDir, WorldNormal);

        // calculate diffuse effect of this light

        //diffuseColor = smoothstep(CutoffThresholds.x, CutoffThresholds.y, diffuseColor);
        //diffuseColor *= light.color;
        //diffuseColor *= light.distanceAttenuation;
        // --

        // calculate specular effect of this light
        //float3 R = reflect(light.direction, WorldNormal);
        //float specTerm = pow(max(0, dot(R, ViewDir)), Smoothness);
        //specularColor = light.color * specTerm;
        // --

        float3 lightingNormalized = LightingLambert(light.color, light.direction, WorldNormal);
        float distanceFactor = smoothstep(CutoffThresholds.x, CutoffThresholds.y, light.color) * light.distanceAttenuation;
        float3 diffuseColor = lightingNormalized * distanceFactor;

        //float3 specularNormalized = LightingSpecular(light.color, light.direction, WorldNormal, ViewDir, Metallic, Smoothness);
        //float3 specularColor = specularNormalized * distanceFactor;

        DiffuseLight += diffuseColor;
        //SpecularLight += specularColor;
    }
#endif
}

void EvaluateHeight_float(
    float2 UV, UnityTexture2D HeightMap, UnitySamplerState SS, float Strength,
    out float Height
)
{
    float4 color = SAMPLE_TEXTURE2D(HeightMap, SS, UV);
    Height = color.x * Strength;
}

void EvaluateHeight_half(
    float2 UV, UnityTexture2D HeightMap, UnitySamplerState SS, float Strength,
    out float Height
)
{
    EvaluateHeight_float(UV, HeightMap, SS, Strength, Height);
}