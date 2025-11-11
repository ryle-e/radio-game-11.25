#include "Hatching.hlsl"

// pulled from unity generated shaders iirc
void BlendLighten(
    float4 Base, float4 Blend, float Opacity,
    out float4 Output
)
{
    Output = max(Blend, Base);
    Output = lerp(Base, Output, Opacity);
}


void LayeredHatching_float(
    float2 UV, float VoronoiSize, float TileRotationPower, float StripeSize, float LineThickness, float MaxLayers, float CurrentLayers, float OffsetWidth,
    out float4 Output
)
{
    float2 offset = float2(0,0);
    Output = float4(0,0,0,0);

    // create layers on layers of hatches
    for (int i = 0; i < MaxLayers; i += 1)
    {
        float4 hatchLayer;
        Hatching_float(UV + offset, VoronoiSize, TileRotationPower, StripeSize, LineThickness, float4(1,1,1,1), hatchLayer);

        offset += float2(OffsetWidth, 0);

        BlendLighten(Output, hatchLayer, clamp(1 * (CurrentLayers - i), 0, 1), Output);
    }
}

void LayeredHatching_half(
    float2 UV, float VoronoiSize, float TileRotationPower, float StripeSize, float LineThickness, float MaxLayers, float CurrentLayers, float OffsetWidth,
    out float4 Output
)
{
    LayeredHatching_float(UV, VoronoiSize, TileRotationPower, StripeSize, LineThickness, MaxLayers, CurrentLayers, OffsetWidth, Output);
}


// create a hatching texture, but make the layers grayscale so that they can be sampled using a single number factor
// in other words, make the texture a faded grayscale so that we can fade hatching layers in and out
void LayeredHatchingGrayscale_float(
    float2 UV, float VoronoiSize, float TileRotationPower, float StripeSize, float LineThickness, float MaxLayers, float CurrentLayers, float OffsetWidth,
    out float4 Output
)
{
    float2 offset = float2(0,0);
    Output = float4(0,0,0,0);

    for (int i = 0; i < MaxLayers; i += 1)
    {
        float4 hatchLayer;
        Hatching_float(UV + offset, VoronoiSize, TileRotationPower, StripeSize, LineThickness, float4(1,1,1,1) * (i / MaxLayers), hatchLayer);

        offset += float2(OffsetWidth, 0);

        BlendLighten(Output, hatchLayer, clamp(1 * (CurrentLayers - i), 0, 1), Output);
    }
}

void LayeredHatchingGrayscale_half(
    float2 UV, float VoronoiSize, float TileRotationPower, float StripeSize, float LineThickness, float MaxLayers, float CurrentLayers, float OffsetWidth,
    out float4 Output
)
{
    LayeredHatchingGrayscale_float(UV, VoronoiSize, TileRotationPower, StripeSize, LineThickness, MaxLayers, CurrentLayers, OffsetWidth, Output);
}