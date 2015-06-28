
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
	float power;
};
struct Material
{
	float4 ambient;
	float4 diffuse;
	float4 emissive;
	float4 specular;
	float shininess;
};
float4x4 IdentityMatrix =   
{  
    {1,0,0,0},  
    {0,1,0,0},  
    {0,0,1,0},  
    {0,0,0,1}  
}; 


// Global variables
// Can be accessed from outside the shader, using Effect->Parameters["key"] where key = variable name
float4x4 worldMatrix;
float4x4	worldViewProjectionMatrix;
float4x4	worldInverseTransposeMatrix; // For calculating normals

float4x4 voxelMatrix;

float3 cameraPos;
float4 globalAmbient;
float2 scaleBias;

Light light;
Light light2;
Material material;

struct VSInput
{
    float4 Position : POSITION;
};
struct VertexToPixel
{
    float4 Position			: POSITION;    
    float3 Color			: COLOR0;
    float3 Normal        : TEXCOORD1;
    float3 Position3D    : TEXCOORD2;
};

struct PixelToFrame
{
    float4 Color        : COLOR0;
};




float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
{
    float3 lightDir = normalize(pos3D - lightPos);
    return dot(-lightDir, normal);    
}
VertexToPixel PerPixelVertexShader( VSInput input, float4x4 transform : TEXCOORD0, float3 color : COLOR0, float3 normal : NORMAL0)
{
    VertexToPixel Output;
	
	Output.Position = mul(input.Position, transpose(transform));
	Output.Position = mul(Output.Position, worldViewProjectionMatrix);

	
    Output.Normal = normalize(mul(normal, (float3x3)worldMatrix));  
	Output.Position3D = mul(input.Position, transpose(transform));  
    Output.Position3D = mul(Output.Position3D, worldMatrix);
	
	Output.Color = color;
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

struct VertexShaderInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float3 Color : COLOR0;
};

VertexToPixel PerPixelNonInstancedVS( VertexShaderInput input)
{
    VertexToPixel Output;
    
    Output.Position = float4( input.Position, 1);
	Output.Position = mul(Output.Position, worldViewProjectionMatrix);
    Output.Normal = normalize(mul(input.Normal, (float3x3)worldMatrix));    
    Output.Position3D = mul(input.Position, worldMatrix);
	Output.Color = input.Color;
    return Output;
}
PixelToFrame PerPixelNonInstancedPS(VertexToPixel PSIn)
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
technique PerPixelNonInstanced
{
	pass P0
	{
		VertexShader = compile vs_3_0 PerPixelNonInstancedVS();
		PixelShader = compile ps_3_0 PerPixelNonInstancedPS();
	}
}


// our technique
technique PerPixelLight
{
	pass P0
	{
		VertexShader = compile vs_3_0 PerPixelVertexShader();
		PixelShader = compile ps_3_0 PerPixelPixelShader();
	}
}