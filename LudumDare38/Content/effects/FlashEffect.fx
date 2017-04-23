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

float Progress;
float4 FlashTexture(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TextureCordinate);
    if (Progress > 0.0f && color.a > 0) {
    	color.rgb = color.rgb + Progress;
    }
 
    return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 FlashTexture();
    }
}