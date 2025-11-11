void LerpGrayscaleTexture_float(
    float2 UV, UnityTexture2D Texture, UnitySamplerState SS, float MaxLayers, float CurrentLayers,
    out float4 Output
)
{ 
    //Output = SAMPLE_TEXTURE2D(Texture, SS, UV);
    //Output = float4(1,1,1,1) * (CurrentLayers / MaxLayers);
    //Output = float4(1,1,1,1) * clamp(0, 1, SAMPLE_TEXTURE2D(Texture, SS, UV) - clamp(0, 1, 1 - CurrentLayers / MaxLayers));

    float layerT = CurrentLayers / MaxLayers;
    float invLayerT = 1 - layerT;

    float hatchValue = SAMPLE_TEXTURE2D(Texture, SS, UV).x; // clamped to [0, 1]

    //Output = float4(1,1,1,1) * clamp(0,1, SAMPLE_TEXTURE2D(Texture, SS, UV).x - (1 - (CurrentLayers / MaxLayers)));

    if (invLayerT < hatchValue)
    {
        Output = float4(1,1,1,1);
    }
    else
    {
        Output = float4(0,0,0,0);
    }

    /*
    if ((1 - (CurrentLayers / MaxLayers)) < SAMPLE_TEXTURE2D(Texture, SS, UV).x)
    {
        Output = float4(1,1,1,1);
    }
    else
    {
        Output = float4(0,0,0,0);
    }
    */

    //Output = float4(1,1,1,1) * clamp(0, 1, 1 - (SAMPLE_TEXTURE2D(Texture, SS, UV)) - (CurrentLayers / MaxLayers));
}

void LerpGrayscaleTexture_half(
    float2 UV, UnityTexture2D Texture, UnitySamplerState SS, float MaxLayers, float CurrentLayers,
    out float4 Output
)
{
    LerpGrayscaleTexture_float(UV, Texture, SS, MaxLayers, CurrentLayers, Output);
}