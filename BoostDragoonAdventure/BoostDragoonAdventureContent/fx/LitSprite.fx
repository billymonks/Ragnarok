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

float4 solidColor;

texture2D ColorMap;
sampler2D ColorMapSampler = sampler_state
{
    Texture = <ColorMap>;
};

struct PixelShaderInput
{
	float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0;
};

float4 AmbientPixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.8f)
	  {
		//color.a = 0;
		clip(-1);
	  }

    
	return baseColor + color * AmbientColor * AmbientIntensity;
}

float4 PointPixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);

	return color;
}

float4 UnlitPixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.1f)
	  {
		//color.a = 0;
		clip(-1);
	  }

	//return solidColor;
	return float4(color.r + solidColor.r, color.g + solidColor.g, color.b + solidColor.g, color.a);
}

float4 SolidColorPixelShaderFunction(PixelShaderInput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.1f)
	  {
		color.a = 0;
		clip(-1);
	  }

    
	return float4(solidColor.r, solidColor.g, solidColor.b, 1);
}

technique MultiPassLight
{
	//pass Ambient
	//{
		//PixelShader = compile ps_2_0 AmbientPixelShaderFunction();
	//}
	//pass Point
	//{
		//SrcBlend = One;
		//DestBlend = One;
		//PixelShader = compile ps_2_0 PointPixelShaderFunction();
	//}
	pass Unlit
	{
		PixelShader = compile ps_2_0 UnlitPixelShaderFunction();
	}
}

technique SolidColor
{
	pass SolidColor
	{
		PixelShader = compile ps_2_0 SolidColorPixelShaderFunction();
	}
}
