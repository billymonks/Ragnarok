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
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
	MipFilter = Linear;
};

texture2D NormalMap;
sampler NormalMapSampler = sampler_state
{
	Texture = <NormalMap>;
	MinFilter = Anisotropic;
	MagFilter = Anisotropic;
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

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 NormCoord : TEXCOORD1;
	float3 View : TEXCOORD2;
	float3x3 WorldToTangentSpace : TEXCOORD3;
};

struct PerPixelVertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float2 NormCoord : TEXCOORD1;
	float4 Light : TEXCOORD2;
	float3 View : TEXCOORD3;
	float3x3 WorldToTangentSpace : TEXCOORD4;
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

	return output;
}

PerPixelVertexShaderOutput PerPixelVertexShaderFunctionSpecial(VertexShaderInput input)
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

	//output.Light.x = 1-output.Light.x;
	//output.Light.y = 1-output.Light.y;

	output.Light.xyz = normalize(mul(output.WorldToTangentSpace, Light));
	output.Light.w = saturate(1 - dot(Light, Light));

	output.View = mul(output.WorldToTangentSpace, EyePosition - worldPosition);

	return output;
}

float4 PerPixelPixelShaderFunction(PerPixelVertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);
	float3 normalMap = 2.0 * (tex2D(NormalMapSampler, input.NormCoord)) - 1.0;

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

float4 PPPixelShaderFunctionColorOnly(PerPixelVertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);
	float3 normalMap = float3(0,0,1);

	float3 lDirection = normalize(input.Light.xyz);
	float3 vDirection = normalize(input.View);

	if(color.a < 0.8f)
	  {
		clip(-1);
	  }

	float diffuse = saturate(dot(normalMap, lDirection));
	float shadow = saturate(4.0 * lDirection.z);
	float3 reflect = normalize(2 * diffuse * normalMap - lDirection);
	float specular = min(pow(saturate(dot(reflect, vDirection)), 3), color.w);

	return (shadow * (color * DiffuseIntensity * (diffuse * DiffuseColor + specular * SpecularColor) * input.Light.w));
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

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.TexCoord = input.TexCoord;
	output.NormCoord = input.NormCoord;

	output.WorldToTangentSpace[0] = mul(normalize(input.Tangent), World);
	output.WorldToTangentSpace[1] = mul(normalize(input.Binormal), World);
	output.WorldToTangentSpace[2] = mul(normalize(input.Normal), World);

    output.View = normalize(float4(EyePosition, 1.0) - worldPosition);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.8f)
	  {
		clip(-1);
	  }

	float3 normalMap = 2.0 * (tex2D(NormalMapSampler, input.NormCoord)) - 1.0;
	normalMap = normalize(mul(normalMap, input.WorldToTangentSpace));
	float4 normal = float4(normalMap,1.0);

    float4 diffuse = saturate(dot(-LightDirection,normal));
	float4 reflect = normalize(2*diffuse*normal-float4(LightDirection,1.0));
	float4 specular = pow(saturate(dot(reflect,input.View)),8);

	/*return color * AmbientColor * AmbientIntensity +
		   color * DiffuseIntensity * DiffuseColor * diffuse +
		   color * SpecularColor * specular;*/
	return float4(0,0,0,1) +
		   color * AmbientColor * AmbientIntensity +
		   color * DiffuseIntensity * DiffuseColor * diffuse +
		   color * SpecularColor * specular * DiffuseIntensity;
	//return normal;
}

float4 FBPixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.8f)
	  {
		clip(-1);
	  }

	
	return baseColor + 
	color * AmbientColor * AmbientIntensity +
	color * DiffuseIntensity * DiffuseColor;
}

float4 PixelShaderColorOnlyNoShadow(PerPixelVertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ColorMapSampler, input.TexCoord);
	float3 normalMap = float3(0,0,1);

	float3 lDirection = normalize(input.Light.xyz);
	float3 vDirection = normalize(input.View);

	if(color.a < 0.8f)
	  {
		clip(-1);
	  }

	float diffuse = saturate(dot(normalMap, lDirection));
	float shadow = saturate(4.0 * lDirection.z);

	return (shadow * (color * DiffuseIntensity * (diffuse * DiffuseColor)));
}

technique PerPixelNormalMappingTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PerPixelPixelShaderFunction();
	}
}

technique PerPixelNoNormalTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PPPixelShaderFunctionColorOnly();
	}
}

technique NormalMappingTechnique
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique FullBrightTechnique
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 VertexShaderFunction();
		PixelShader = compile ps_2_0 FBPixelShaderFunction();
	}
}
technique CharacterNoShadow
{
	pass Pass1
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderColorOnlyNoShadow();
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
technique MultiPassLightColorOnly
{
	pass Ambient
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PixelShaderFunctionAmbient();
	}
	pass Point
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunction();
		PixelShader = compile ps_2_0 PPPixelShaderFunctionColorOnly();
	}
}

technique MultiPassLightColorOnlyVariable
{
	pass Ambient
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunctionSpecial();
		PixelShader = compile ps_2_0 PixelShaderFunctionAmbient();
	}
	pass Point
	{
		VertexShader = compile vs_2_0 PerPixelVertexShaderFunctionSpecial();
		PixelShader = compile ps_2_0 PixelShaderColorOnlyNoShadow();
	}
}
