
void RotateDegrees(
    float2 UV, float2 Center, float Rotation, 
    out float2 Output
)
{
    // copied from Unity's internal shader code

    Rotation = Rotation * (3.1415926f/180.0f);
    UV -= Center;
    float s = sin(Rotation);
    float c = cos(Rotation);

    //center rotation matrix
    float2x2 rMatrix = float2x2(c, -s, s, c);
    rMatrix *= 0.5;
    rMatrix += 0.5;
    rMatrix = rMatrix*2 - 1;

    //multiply the UVs by the rotation matrix
    UV.xy = mul(UV.xy, rMatrix);
    UV += Center;

    Output = UV;
}

// also pulled from unity shader code iirc, or at least from the stripes texture in the procedural textures package
void Rectangle(
    float2 UV, float Width, float Height,
    out float Output
)
{
    float2 d = abs(UV * 2 - 1) - float2(Width, Height);
#if defined(SHADER_STAGE_RAY_TRACING)
    d = saturate((1 - saturate(d * 1e7)));
#else
    d = saturate(1 - d / fwidth(d));
#endif
    Output = min(d.x, d.y);
}

void Stripes_float(
    float2 UV, float Frequency, float Offset, float Thickness, float Rotation,
    out float Output
)
{
    // rotate the uv by the provided value
    RotateDegrees(UV, float2(0.5,0.5), Rotation, UV);

    // scale and offset the uv
    UV *= Frequency;
    UV += Offset;

    // generate a single stripe that gets propagated across the uv
    Rectangle(frac(UV), Thickness, 1, Output);

    // lock the stripes to either 0 or 1
    round(Output);
}