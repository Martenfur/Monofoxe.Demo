sampler s0;
uniform const int radius;
float width;
float height;

float4x4 World;
float4x4 View;
float4x4 Projection;


struct VertexShaderInput
{
  float4 Position : POSITION0;
  float2 TexCoords : TEXCOORD0;
  float4 Color : COLOR0;
};

struct VertexShaderOutput
{
  float4 Position : POSITION0;
  float2 TexCoords : TEXCOORD0;
  float4 Color : COLOR0;
};


VertexShaderOutput PassThroughVertexFunction(VertexShaderInput input)
{
  VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
  float4 viewPosition = mul(worldPosition, View);
  output.Position = mul(viewPosition, Projection);
  output.TexCoords = input.TexCoords;
  output.Color = input.Color;

  return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
  float4 color = float4(0, 0, 0, 0);
  
	float2 pixel = float2(1.0 / width, 1.0 / height);

	int limit = radius * 2 + 1;
  [unroll(9)] for(int x = 0; x < limit; x += 1)
  {
    [unroll(9)] for(int y = 0; y < limit; y += 1)
    {
      color += tex2D(s0, float2(input.TexCoords.x + (x - radius) * pixel.x, input.TexCoords.y + (y - radius) * pixel.y));
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
    
    VertexShader = compile vs_3_0 PassThroughVertexFunction();
    PixelShader = compile ps_3_0 PixelShaderFunction();
  }
}