sampler2D input : register(s0);

cbuffer Parameters : register(b0)
{
    float2 Center;
    float Zoom;
    float MaxIterations;
    float2 Resolution;
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    // Modify the scaling factor by dividing by 2 to zoom out
    float2 z = (uv - 0.5) * Resolution / min(Resolution.x, Resolution.y) / (Zoom * 0.5) + Center;
    float2 c = z;
    
    int i;
    for (i = 0; i < MaxIterations; i++)
    {
        if (dot(z, z) > 4.0) break;
        float2 z_squared = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y);
        z = z_squared + c;
    }
    
    if (i >= MaxIterations)
        return float4(0, 0, 0, 1);
        
    // Smooth coloring
    float smoothed = i - log2(log2(dot(z, z))) / 2;
    float hue = smoothed / MaxIterations;
    
    // HSV to RGB conversion for better coloring
    float h = hue * 6.0;
    float s = 0.8;
    float v = 1.0;
    
    float colorMax = v * s;
    float x = colorMax * (1 - abs(fmod(h, 2) - 1));
    float m = v - colorMax;
    
    float3 color;
    if (h < 1) color = float3(colorMax, x, 0);
    else if (h < 2) color = float3(x, colorMax, 0);
    else if (h < 3) color = float3(0, colorMax, x);
    else if (h < 4) color = float3(0, x, colorMax);
    else if (h < 5) color = float3(x, 0, colorMax);
    else color = float3(colorMax, 0, x);
    
    return float4(color + m, 1.0);
}