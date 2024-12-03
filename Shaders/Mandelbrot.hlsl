sampler2D input : register(s0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float4 color = tex2D(input, uv);
    return float4(1.0, 0, 0, color.a);  // Keep alpha from input
}