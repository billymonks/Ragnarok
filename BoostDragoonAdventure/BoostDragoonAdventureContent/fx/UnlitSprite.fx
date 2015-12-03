
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

	//if(color.a < 0.8f)
	  //{
		//color.a = 0;
		//clip(-1);
	  //}

    
	return float4(color.r, color.g, color.b, color.a * input.Color.a);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
