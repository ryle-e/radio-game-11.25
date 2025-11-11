#define ADDITIONAL_LIGHT_INCLUDED

void SplitMainLight_half(
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

void SplitMainLight_float(float3 WorldPos, 
out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
    SplitMainLight_half(WorldPos, Direction, Color, DistanceAtten, ShadowAtten);
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

void CalculateDiffuse_float(
    float3 Direction, float3 WorldNormal, float3 Color, float Distance, float Shadow,
    out float3 Diffuse
)
{
    float NdotL = dot(WorldNormal, Direction);
    float intensity = saturate(NdotL);

    Diffuse = intensity * Color * (Distance * Shadow);
}

void CalculateDiffuse_half(
    float3 Direction, float3 WorldNormal, float3 Color, float Distance, float Shadow,
    out float3 Diffuse
)
{
    CalculateDiffuse_float(Direction, WorldNormal, Color, Distance, Shadow, Diffuse);
}

void CalculateSpecular_float(
    float3 Direction, float3 WorldNormal, float3 ViewDir, float3 Color, float3 Distance, float Metallic, float Smoothness,
    out float3 Specular
)
{
    Specular = 0;

    float3 H = normalize(Direction + ViewDir);

    float NdotH = dot(WorldNormal, H);

    float intensity = pow(saturate(NdotH), Smoothness);

    Specular = intensity * Color * (1 / Distance);
}

void CalculateSpecular_half(
    float3 Direction, float3 WorldNormal, float3 ViewDir, float3 Color, float3 Distance, float Metallic, float Smoothness,
    out float3 Specular
)
{
    CalculateSpecular_float(Direction, WorldNormal, ViewDir, Color, Distance, Metallic, Smoothness, Specular);
}

void StepGradient_float(
    float3 InColor, float Steps, float RoundingMode,
    out float3 OutColor
)
{
    // these methods come from https://discussions.unity.com/t/shader-for-stepping-banding-gradient/748394
    if (RoundingMode == 0)
    {
        OutColor = floor(InColor * Steps) / Steps;
    }
    else if (RoundingMode == 1)
    {
        OutColor = round(InColor * Steps) / Steps;
    }
    else
    {
        OutColor = ceil(InColor * Steps) / Steps;
    }
}

void StepGradient_half(
    float3 InColor, float Steps, float RoundingMode,
    out float3 OutColor
)
{
    StepGradient_float(InColor, Steps, RoundingMode, OutColor);
}

void MainLight_float(
    float3 WorldPos, float3 WorldNormal, float3 ViewDir, float Metallic, float Smoothness,
    out float3 Diffuse, out float3 Specular
)
{
    float3 Direction, Color;
    float Distance, Shadow;

    SplitMainLight_float(WorldPos, Direction, Color, Distance, Shadow);

    CalculateDiffuse_float(Direction, WorldNormal, Color, Distance, Shadow, Diffuse);
    CalculateSpecular_float(Direction, WorldNormal, ViewDir, Color, Distance, Metallic, Smoothness, Specular); 
}

void MainLight_half(
    float3 WorldPos, float3 WorldNormal, float3 ViewDir, float Metallic, float Smoothness,
    out float3 Diffuse, out float3 Specular
)
{
    MainLight_float(WorldPos, WorldNormal, ViewDir, Metallic, Smoothness, Diffuse, Specular);
}



void AdditionalLights_float(
    float3 WorldPos, float3 WorldNormal, float3 ViewDir, float Metallic, float Smoothness,
    out float3 Diffuse, out float3 Specular
)
{
    Diffuse = 0;
    Specular = 0;

#if SHADERGRAPH_PREVIEW
    float distance = 0.05;
    float shadow = 1;//AdditionalLightRealtimeShadow(i, WorldPos);

    float3 diffuseColor, specularColor = 0;

    CalculateDiffuse_float(float3(-0.7,-0.7,-1), WorldNormal, float3(1,1,1), distance, shadow, diffuseColor);
    CalculateSpecular_float(float3(-0.7,-0.7,-1), WorldNormal, ViewDir, float3(1,1,1), distance, Metallic, Smoothness, specularColor);

    Diffuse = diffuseColor * 1;
    Specular = specularColor * 1;

#else
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

        //float3 lightingNormalized = LightingLambert(light.color, light.direction, WorldNormal);
        //float distanceFactor = smoothstep(CutoffThresholds.x, CutoffThresholds.y, light.color) * light.distanceAttenuation;
        //float3 diffuseColor = lightingNormalized * distanceFactor;

        //float3 specularNormalized = LightingSpecular(light.color, light.direction, WorldNormal, ViewDir, Metallic, Smoothness);
        //float3 specularColor = specularNormalized * distanceFactor;

        float distance = light.distanceAttenuation;
        float shadow = AdditionalLightRealtimeShadow(i, WorldPos);

        float3 diffuseColor, specularColor = 0;

        CalculateDiffuse_float(light.direction, WorldNormal, light.color, distance, shadow, diffuseColor);
        CalculateSpecular_float(light.direction, WorldNormal, ViewDir, light.color, distance, Metallic, Smoothness, specularColor);

        Diffuse += diffuseColor;
        Specular += specularColor;
    }
#endif
}

void CalculateDistanceMultiplier_float(
    float Input, float RoundingPower,
    out precise float Output
)
{
    // the nearest odd number to the RoundingPower parameter, this ensures we keep a cubic-shaped curve
    float N = floor(RoundingPower * 0.5) * 2 + 1;

    // the height of the curve
    float a = 0.5;

    // tightness of the curve itself, however because this curve uses a fraction exponent it also acts as an x-displacement
    float b = 2;

    // the x-displacement of the curve
    float c = 1;

    // the y-displacement of the curve
    float d = 0.5;

    // a(bx - d)^(1/N) + c
    // can be simplified in this case to ((2x - 1)^(1/N) + 1) / 2
    // https://www.desmos.com/calculator/wr9ioojwsw

    //float Term = (b * Input) - c;
    //float InvPower = (1.0 / N);

    //Output = (a * pow(Term, InvPower)) + d;

    // clamp output between 0 and 1
    //Output = saturate(Output);

    Output = ((Input + 0.5) * 2) + 0.5;

    Output = saturate(Input);

}

void CalculateDistanceMultiplier_half(
    float Input, float RoundingPower,
    out precise float Output
)
{
    CalculateDistanceMultiplier_float(Input, RoundingPower, Output);
}