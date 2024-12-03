sampler2D input : register(s0);

cbuffer Parameters : register(b0)
{
    float2 Center;
    float Zoom;
    float MaxIterations;
    float2 Resolution;
    float ColorMode;  // Added for different color schemes
}

float3 RainbowColor(float t)
{
    float3 color;
    t *= 6.0;
    if (t < 1.0) color = float3(1, t, 0);
    else if (t < 2.0) color = float3(2.0 - t, 1, 0);
    else if (t < 3.0) color = float3(0, 1, t - 2.0);
    else if (t < 4.0) color = float3(0, 4.0 - t, 1);
    else if (t < 5.0) color = float3(t - 4.0, 0, 1);
    else color = float3(1, 0, 6.0 - t);
    return color;
}

float3 BlueYellowColor(float t)
{
    return lerp(float3(0, 0, 1), float3(1, 1, 0), t);
}

float3 GrayscaleColor(float t)
{
    return float3(t, t, t);
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    // Use higher precision calculations
    float2 z = (uv - 0.5) * Resolution / min(Resolution.x, Resolution.y) / (Zoom * 0.5) + Center;
    float2 c = z;
    float2 dz = float2(1, 0);  // For orbit trapping
    
    int i;
    float smooth_i = 0;
    
    for (i = 0; i < MaxIterations; i++)
    {
        if (dot(z, z) > 4.0)
        {
            // Smooth iteration count for better coloring
            smooth_i = i + 1 - log2(log2(dot(z, z))) / log(2.0);
            break;
        }
        
        // Calculate next iteration with higher precision
        float x = z.x * z.x - z.y * z.y + c.x;
        float y = 2 * z.x * z.y + c.y;
        z = float2(x, y);
        
        // Update derivative for orbit trapping
        float dx = 2 * (z.x * dz.x - z.y * dz.y) + 1;
        float dy = 2 * (z.x * dz.y + z.y * dz.x);
        dz = float2(dx, dy);
    }
    
    if (i >= MaxIterations)
        return float4(0, 0, 0, 1);
    
    float t = smooth_i / MaxIterations;
    
    // Add some orbit trap coloring
    float orbit = 0.5 + 0.5 * sin(log(length(dz)) * 0.5);
    t = lerp(t, orbit, 0.2);  // Blend with orbit trap
    
    float3 color;
    if (ColorMode == 0)
        color = RainbowColor(t);
    else if (ColorMode == 1)
        color = GrayscaleColor(t);
    else if (ColorMode == 2)
        color = BlueYellowColor(t);
    else
        color = RainbowColor(t);
        
    return float4(color, 1.0);
}