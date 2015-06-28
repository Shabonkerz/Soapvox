float4x4 World;
float4x4 View;
float4x4 Projection;

Texture Texture;
sampler BlockTextureSampler = sampler_state
{
	texture = <Texture>;
	magfilter = POINT;
	minfilter = POINT;
	mipfilter = POINT;
	AddressU = WRAP;
	AddressV = WRAP;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
	float3 normal : NORMAL;
    float4 tangent : TANGENT;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoords : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);

    
    output.TexCoords = input.TexCoords;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    return tex2D(BlockTextureSampler, input.TexCoords);
}

technique Main
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
