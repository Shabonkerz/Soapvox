struct Light
{
	float3 dir;				// world space direction
	float3 pos;				// world space position
	float4 ambient;
	float4 diffuse;
	float4 specular;
	float spotInnerCone;	// spot light inner cone (theta) angle
	float spotOuterCone;	// spot light outer cone (phi) angle
	float radius;           // applies to point and spot lights only
};

struct Material
{
	float4 ambient;
	float4 diffuse;
	float4 emissive;
	float4 specular;
	float shininess;
};

float4x4 worldMatrix;
float4x4 worldInverseTransposeMatrix;
float4x4 worldViewProjectionMatrix;

float4x4 voxelMatrix;

float3 cameraPos;
float4 globalAmbient;
float2 scaleBias;

Light light;
Material material;

//-----------------------------------------------------------------------------
// Textures.
//-----------------------------------------------------------------------------

texture voxelTexture;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Color	: COLOR0;
    float3 Normal	: NORMAL0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 LightDir	: TEXCOORD0;
    float3 Normal	: TEXCOORD1;
    float4 Color	: COLOR0;   
};
struct VertexShaderOutputDir
{
	float4 position : POSITION;
    float4 color	: COLOR0;   
	float3 halfVector : TEXCOORD0;
	float3 lightDir : TEXCOORD1;
	float3 normal : TEXCOORD2;
	float4 diffuse : COLOR1;
	float4 specular : TEXCOORD3;
};

struct PixelShaderOutput
{
	float4 Color	: COLOR0;
};

sampler2D TextureMapSampler = sampler_state
{
	texture = <voxelTexture>;
	AddressU = CLAMP;
	AddressV = CLAMP;
	MinFilter = LINEAR;
	MipFIlter = LINEAR;
	MagFIlter = LINEAR;
};

float AreaElement( float x, float y )
{
    return atan2(x * y, sqrt(x * x + y * y + 1));
}
//=============================================
//------------ Technique: Default -------------
//=============================================

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, worldViewProjectionMatrix);
    output.Color = float4(input.Color,0.0f);
    
    // Will be moved into PixelShader for deferred rendering
    output.LightDir = normalize(light.dir);			// Light Direction (normalised)
	output.Normal = normalize(mul(input.Normal, worldMatrix));		// Surface Normal (normalised) [World Coordinates]
	
    return output;
}
VertexShaderOutputDir VertexShaderDirLighting(VertexShaderInput input)
{
	VertexShaderOutputDir OUT;

	float3 worldPos = mul(input.Position, worldMatrix).xyz;
	float3 viewDir = cameraPos - worldPos;
	
    OUT.color = float4(input.Color,0.0f);
    OUT.position = mul(input.Position, worldViewProjectionMatrix);
	OUT.lightDir = -light.dir;
	OUT.halfVector = normalize(normalize(OUT.lightDir) + normalize(viewDir));
	OUT.normal = mul(input.Normal, (float3x3)worldInverseTransposeMatrix);
	OUT.diffuse = material.diffuse * light.diffuse;
	OUT.specular = material.specular * light.specular;
        
	return OUT;
}
float4 PixelShaderDirLighting(VertexShaderOutputDir IN) : COLOR
{
    float3 n = normalize(IN.normal);
    float3 h = normalize(IN.halfVector);
    float3 l = normalize(IN.lightDir);
    
    float nDotL = saturate(dot(n, l));
    float nDotH = saturate(dot(n, h));
    float power = (nDotL == 0.0f) ? 0.0f : pow(nDotH, material.shininess);   

	float4 color = (material.ambient * (globalAmbient + light.ambient)) +
                   (IN.diffuse * nDotL) + (IN.specular * power);

	return color * IN.color;
}
PixelShaderOutput PS(VertexShaderOutput input)
{
	PixelShaderOutput output;
	
	// [Ambient Light] I = Ai * Ac
	float Ambient_Intensity = 0.3f;
	float4 Ambient_Colour = float4(1.0f, 1.0f, 1.0f, 1.0f);
	float4 Ambient_Light = Ambient_Intensity * Ambient_Colour;
	
	// [Diffuse Light] I = Di * Dc * N.L
	float Diffuse_Intensity = 1.0f;
	float4 Diffuse_Colour = float4(1.0f, 1.0f, 1.0f, 1.0f);
	float NdotL = dot(input.Normal, input.LightDir);
	float4 Diffuse_Light = Diffuse_Intensity * Diffuse_Colour * saturate(NdotL);
	
	output.Color = (Ambient_Light + Diffuse_Light) * input.Color;
	
    return output;
}
technique PerPixelDirectionalLighting
{
	pass
	{
		VertexShader = compile vs_2_0 VertexShaderDirLighting();
		PixelShader = compile ps_2_0 PixelShaderDirLighting();
	}
}
technique Default
{
	pass
	{
		VertexShader = compile vs_2_0 VS();
		PixelShader = compile ps_2_0 PS();
	}
}