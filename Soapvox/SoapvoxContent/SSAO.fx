//Matrix used by this shader pass
float4x4 World;
float4x4 View;
float4x4 Projection;
 
//The ViewProjection matrix used to render the inital scene
float4x4 SceneViewProj;
 
//The render target half pixel
float2 halfPixel;
 
//The camera forward vector
float3 viewForward;
 
//Samples points distributed in the unit sphere using energy minimisation
float3 samples[32] =
   {
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.970699, -0.184458, 0.154010),
        float3(-0.831648, -0.217331, -0.511008),
        float3(-0.034957, 0.797863, -0.601824),
        float3(-0.305365, 0.806397, 0.506435),
        float3(-0.305365, 0.806397, 0.506435)
   };
 
//A random texture used for dithering
texture RandomTexture;
sampler randomSampler = sampler_state
{
    Texture = (RandomTexture);
    MAGFILTER = POINT;
    MINFILTER = POINT;
    MIPFILTER = POINT;
    AddressU = Mirror;
    AddressV = Mirror;
};
 
//Geometric information (World position)
texture RT1;
sampler textureSampler = sampler_state
{
    Texture = (RT1);
    MAGFILTER = Linear;
    MINFILTER = Linear;
    MIPFILTER = Linear;
    AddressU = Border;
    AddressV = Border;
};
 
//Geometric information (World normal)
texture RT3;
sampler normalSampler = sampler_state
{
    Texture = (RT3);
    MAGFILTER = Linear;
    MINFILTER = Linear;
    MIPFILTER = Linear;
    AddressU = Border;
    AddressV = Border;
};
 
//The number of samples used by the SSAO
const int nbrSamples = 16;
 
//The world scale 
float minScale = 3;
float maxScale = 30;
 
//Exponent used to enhance the final contrast
float exponent = 1;
 
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Color	: COLOR0;
    float3 Normal	: NORMAL0;
};
 
struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Color	: COLOR0;
    float3 Normal	: NORMAL0;
};
 
// Simple vertex shader
VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;
 
    float4x4 worldViewProjection = mul( mul(World, View), Projection);
    output.Position = mul(input.Position, worldViewProjection);
	output.Color = input.Color;
	output.Normal = input.Normal;

    return output;
}
 
// The SSAO pixel shader
float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{    
    //Obtain the normalized forward vector
    float3 forward = normalize(viewForward);
 
    //We sample the different buffers
    float4 fragmentPos = tex2D(textureSampler, input.TexCoord);
    float3 fragmentNormal = tex2D(normalSampler, input.TexCoord);
    fragmentNormal = normalize((fragmentNormal - 0.5f) * 2.0f);    
 
    //Pick a random vector for this pixel
    float3 random = normalize(tex2D(randomSampler, input.TexCoord * 250));
 
    float totalWeight = 0;
    float totalOcclusion = 0;
    for(int i = 0; fragmentPos.a > 0.5 && i<nbrSamples && i < 32; i++)
    {
        //Distribute samples scale between minScale and maxScale 
        float k = (float)i / (float)nbrSamples;
        float sampleScale = minScale * k  + maxScale * (1-k);
 
        //Compute a sample vector. We use a random reflection plane for dithering
        float3 sampleOffset = sampleScale * reflect(samples[i%32].rgb , random);
 
        //We reflect the sample vector if needed to avoid self-occlusion
        if( dot( sampleOffset, fragmentNormal) < 0)
            sampleOffset = reflect(sampleOffset , fragmentNormal);    
 
        //Compute the sample position
        float3 samplePos = fragmentPos.rgb + sampleOffset.rgb;
        //The corresponding depth
        float sampleDepth = dot(samplePos, forward);
 
        //The corresponding offset in pixel, in the screen space
        float4 screenOffset = mul(float4(sampleOffset,1), SceneViewProj);
        float2 uvOffset = float2( screenOffset.x, -screenOffset.y) / screenOffset.w;
 
        //Read the corresponding pixel position in the scene
        float4 scenePos = tex2D(textureSampler, input.TexCoord + uvOffset);
        //The corresponding depth
        float sceneDepth = dot(scenePos, forward);
 
        //Put depth to infinity if no geometry is drawn (scenePos.a=0), to avoid occlusion from background
        if(scenePos.a < 0.5)
            sceneDepth = -100000;    
 
        //Computing the relative depth
        float relativeDepth = (sampleDepth - sceneDepth) / (maxScale);
 
        //The sample contribution depends on its angle with surface normal
        float weight = dot( normalize(samplePos - fragmentPos), normalize(fragmentNormal));
        totalWeight += weight;
 
        //The final occlusion        
        float occlusionFactor =  weight * 1 / (1 + relativeDepth * relativeDepth);
 
        totalOcclusion += step(relativeDepth, -1/(sampleScale * 1000)) * occlusionFactor;
    }        
 
    //Renormalize the total occlusion
    totalOcclusion = totalOcclusion / totalWeight;
 
    //Enhance the final contrast
    float finalOcclusion = pow(totalOcclusion, exponent);
 
    return 1 - finalOcclusion;
}