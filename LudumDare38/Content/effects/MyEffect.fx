// Our texture sampler
texture Texture;
sampler TextureSampler = sampler_state
{
    Texture = <Texture>;
};
 
// This data comes from the sprite batch vertex shader
struct VertexShaderOutput
{
    float4 Position : TEXCOORD0;
    float4 Color : COLOR0;
    float2 TextureCordinate : TEXCOORD0;
};
 
// Our pixel shader
float Duration;
float4 FlashTexture(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TextureCordinate);
    if (Duration > 0.0f && color.a > 0) {
    	color.rgb = color.rgb + Duration;
    }
 
    return color;
}
 
// Compile our shader
technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 FlashTexture();
    }
}