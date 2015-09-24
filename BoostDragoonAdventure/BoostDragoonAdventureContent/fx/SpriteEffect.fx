float depth;

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

    
	//return float4(lerp(0.37, 0.97, input.TexCoord.y/1440), 0, 0, 1);
	//return float4(lerp(0.37, 0.97, input.TexCoord.y/1440), 0, 0, 1);
	//return float4(lerp(0.432, 0.734, input.Color.r), 0, 0, 1);
	return float4(lerp(0, 1, input.Color.a-0.15), 0, 0, 1);
}

technique Depth
{
    pass Depth
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
