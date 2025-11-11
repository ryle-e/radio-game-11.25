void GetFogType_float(
    out float Type
)
{
    Type = -1;

    #ifdef FOG_LINEAR
    Type = 0;

    #elif FOG_EXP
    Type = 1;

    #elif FOG_EXP2
    Type = 2

    #endif
}

void ApplyFog_float(
    float3 WorldPosition, float4 Color,
    out float4 OutColor
)
{
    OutColor = float4(1,1,1,1);
}