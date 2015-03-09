float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
float AmbientIntensity;

float3 LightDirection;
float4 DiffuseColor;
float DiffuseIntensity;

float4 SpecularColor;
float SpecularIntensity;
float3 EyePosition;

float3 PointLightPosition;
float PointLightRange;

float4 baseColor;

texture2D ColorMap;
sampler ColorMapSampler = sampler_state
{
	Texture = <ColorMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 NormCoord : TEXCOORD1;
	float3 Normal : NORMAL0;
	float3 Binormal : BINORMAL0;
	float3 Tangent : TANGENT0;
};

struct PerPixelVertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 NormCoord : TEXCOORD1;
	float4 Light : TEXCOORD2;
	float3 View : TEXCOORD3;
	float Depth : TEXCOORD4;
	float3x3 WorldToTangentSpace : TEXCOORD5;
	
};

PerPixelVertexShaderOutput PerPixelVertexShaderFunction(VertexShaderInput input)
{
	PerPixelVertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);

    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.NormCoord = input.NormCoord;

	output.WorldToTangentSpace[0] = normalize(mul(input.Binormal, World));
	output.WorldToTangentSpace[1] = normalize(mul(input.Tangent, World));
	output.WorldToTangentSpace[2] = normalize(mul(input.Normal, World));

	float3 Light = PointLightPosition - worldPosition;

	output.Light.xyz = normalize(mul(output.WorldToTangentSpace, Light));
	output.Light.w = saturate(1 - dot(Light / PointLightRange, Light / PointLightRange));

	output.View = mul(output.WorldToTangentSpace, EyePosition - worldPosition);

	output.Depth.r = 1 - (output.Position.z * output.Position.w); 

	return output;
}



float4 PerPixelPixelShaderFunction(PerPixelVertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);
	float3 normalMap = 2.0 * (tex2D(ColorMapSampler, input.NormCoord)) - 1.0;

	if(color.a < 0.8f)
	  {
		clip(-1);
	  }

	float3 lDirection = normalize(input.Light.xyz);
	float3 vDirection = normalize(input.View);

	float diffuse = saturate(dot(lDirection, normalMap));
	float shadow = saturate(4.0 * lDirection.z);
	float3 reflect = normalize(2 * diffuse * normalMap - lDirection);
	float specular = min(pow(saturate(dot(reflect, vDirection)), 16), color.w);

	return (shadow * (color * (DiffuseIntensity * (diffuse * DiffuseColor)
				 + DiffuseIntensity * SpecularIntensity * (specular * SpecularColor)) * input.Light.w));
}

float4 PixelShaderFunctionAmbient(PerPixelVertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.8f)
	{
		clip(-1);
	}

	return baseColor +
			color * AmbientColor * AmbientIntensity;
}

float4 PixelShaderFunctionDepth(PerPixelVertexShaderOutput input) : COLOR0
{

	return input.Depth.r;
}

technique DisplayDepth
{
	pass Depth
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunctionDepth();
	}
}

technique MultiPassLight
{
	pass Ambient
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunctionAmbient();
	}
	pass Point
	{
		SrcBlend = One;
		DestBlend = One;
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PerPixelPixelShaderFunction();
	}
}
