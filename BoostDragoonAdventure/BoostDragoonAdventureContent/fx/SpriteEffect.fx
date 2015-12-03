float depth;
float3 solidColor;

// TODO: add effect parameters here.

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

float4 PixelShaderFunction(PixelShaderInput input) : COLOR0
{
    float4 color = tex2D(ColorMapSampler, input.TexCoord);

	if(color.a < 0.1f)
	  {
		color.a = 0;
		clip(-1);
	  }

    
	return float4(lerp(0, 1, input.Color.r-0.15), 0, 0, 1);
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

technique Depth
{
    pass Depth
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

technique SolidColor
{
	pass SolidColor
	{
		PixelShader = compile ps_2_0 SolidColorPixelShaderFunction();
	}
}
