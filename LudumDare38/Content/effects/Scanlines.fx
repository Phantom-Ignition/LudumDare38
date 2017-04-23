texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};

struct VertexShaderOutput
{
    float4 Position : TEXCOORD0;
    float4 Color : COLOR0;
    float2 TextureCordinate : TEXCOORD0;
};

float Attenuation;
float LinesFactor;

float4 FlashTexture(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TextureCordinate);
    float scanline = sin( input.Position.y * LinesFactor ) * Attenuation;
    color.rgb -= scanline;
    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 FlashTexture();
    }
}