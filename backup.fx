


struct VertexShaderInput
{
    float4 Position : POSITION;
    float3 Color	: COLOR0;
    float3 Normal	: NORMAL0;
};
// The OUT structure our Vertex Shader will use.
struct VSOUT
{
	float4 Pos: POSITION; // Store transformed position  here
	float3 L:	TEXCOORD0; // Store the normalized light direction here
	float3 N:	TEXCOORD1; // Store the transformed and normalized normal here
	float3 Color:	COLOR0;
};
// The OUT structure our Vertex Shader will use.
struct VSOutput
{
	float4 Position: POSITION; // Store transformed position  here
	float3 LightDirection:	TEXCOORD0; // Store the normalized light direction here
	float3 Normal:	TEXCOORD1; // Store the transformed and normalized normal here
	float3 Color:	COLOR0;
};
float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}
// Our vertex shader, takes the vertex position and vertex normal as input
VSOutput VS( VertexShaderInput input )
{
	VSOutput Out = (VSOUT) 0;
	Out.Position = mul(input.Position, worldViewProjectionMatrix);

	// normalize(a) returns a normalized version of a.
	// in this case, a = vLightDirection
	Out.LightDirection = normalize(light.dir);

	// transform our normal with matInverseWorld, and normalize it
	Out.Normal = normalize(mul(worldInverseTransposeMatrix, input.Normal));
	Out.Color = input.Color;
	return Out;
}

VSOutput InstancingVS(VertexShaderInput input, float4x4 transform : TEXCOORD1, float3 color : COLOR1, float3 normal : NORMAL1)
{
	VSOutput output;

	output.Position = mul(input.Position, transpose(transform));
	output.Position = mul(output.Position, worldViewProjectionMatrix);

	// normalize(a) returns a normalized version of a.
	// in this case, a = vLightDirection
	output.LightDirection = normalize(light.dir);

	// transform our normal with matInverseWorld, and normalize it
	output.Normal = normalize(mul(worldInverseTransposeMatrix, normal));

	// Keep the same color.
	 output.Color = color;

	return output;
}

// Our pixelshader. Needs the light direction and normal from the vertex shader.
float4 PS(VSOutput input) : COLOR
{
	// Ambient light
	float Ai = 0.6f;
	float4 Ac = float4(input.Color, 1.0);
	
	// Diffuse light
	float Di = 0.6f;
	float4 Dc = float4(input.Color, 1.0f);

	// return Ambient light * diffuse light. See tutorial if
	// you dont understand this formula
	return Ai * Ac + Di * Dc * saturate(dot(input.LightDirection, input.Normal));
}



VertexToPixel PerPixelVertexShader( VertexShaderInput input)
{
    VertexToPixel Output;
    
    Output.Position =mul(input.Position, worldViewProjectionMatrix);
    Output.Normal = normalize(mul(input.Normal, (float3x3)worldMatrix));    
    Output.Position3D = mul(input.Position, worldMatrix);
	Output.Color = input.Color;
    return Output;
}
PixelToFrame PerPixelPixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output;

    float diffuseLightingFactor = DotProduct(light.pos, PSIn.Position3D, PSIn.Normal);
    diffuseLightingFactor = saturate(diffuseLightingFactor);
    diffuseLightingFactor *= light.power;

    float4 baseColor = float4(PSIn.Color, 1.0f);
    Output.Color = baseColor*(diffuseLightingFactor + light.ambient);

    return Output;
}
// our technique
technique DiffuseLight
{
	pass P0
	{
		VertexShader = compile vs_3_0 VS();
		PixelShader = compile ps_3_0 PS();
	}
}