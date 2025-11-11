#include "Stripes.hlsl"

inline float2 VoronoiRandomVector(float2 UV, float Offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);

    UV = mul(UV, m);
    UV = float2(sin(UV.x), sin(UV.y));
    UV = frac(UV);

    return float2(
        sin(UV.y*+Offset)*0.5+0.5, 
        cos(UV.x*Offset)*0.5+0.5
        );
}

// this and above function pulled from unity generated shaders
void Voronoi(
    float2 UV, float AngleOffset, float CellDensity, 
    out float Out, out float Cells
)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);

    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = VoronoiRandomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);

            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                Out = res.x;
                Cells = res.y;
            }
        }
    }
}

void Hatching_float(
    float2 UV, float VoronoiSize, float TileRotationPower, float StripeSize, float LineThickness, float4 Color,
    out float4 Output
)
{
    // generate a voronoi texture of filled cells
    float VoronoiT, Cells;
    Voronoi(UV, 5, VoronoiSize, VoronoiT, Cells);

    // convert the cell colour to a rotation value
    float rot = degrees(Cells * TileRotationPower);

    // generate the striped texture
    float stripeT = 0;
    Stripes_float(UV, StripeSize, 20, LineThickness, rot, stripeT);

    // rotate chunks of the stripes according to the voronoi
    Output = Color * stripeT;
}