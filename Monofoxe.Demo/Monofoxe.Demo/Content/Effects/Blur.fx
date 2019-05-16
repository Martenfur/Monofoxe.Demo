sampler s0;
uniform const int radius;
float width;
float height;


float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
  float4 color = float4(0, 0, 0, 0);
  
	float2 pixel = float2(1.0 / width, 1.0 / height);

	int limit = radius * 2 + 1;
  [unroll(9)] for(int x = 0; x < limit; x += 1)
  {
    [unroll(9)] for(int y = 0; y < limit; y += 1)
    {
      color += tex2D(s0, float2(coords.x + (x - radius) * pixel.x, coords.y + (y - radius) * pixel.y));
    }
  }

  return color / (limit * limit);
}



technique Technique1
{
  pass Pass1
  {
   // AlphaBlendEnable = true;
   // DestBlend = DESTALPHA;
   // SrcBlend = SRCALPHA;
    
    PixelShader = compile ps_3_0 PixelShaderFunction();
  }
}