// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_17th_Code/NumberMatrix"
{
	Properties
	{
		_TintColor ("Tint Color", Color) = (1,1,1,1)
		_RandomTex ("Random Tex", 2D) = "white" {}
		_FlowingTex ("Flowing Tex", 2D) = "white" {}
		_NumberTex ("Number Tex", 2D) = "white" {}
		_CellSize ("Cell Size (xyz)", Vector) = (0.03, 0.04, 0.03, 0)
		_TexelSizes ("Random Texel Size, Flowing Texel Size, Number Count", Vector) = (0.015625, 0.00390625, 10, 0)
		_Speed ("Flowing Speed, Number Changing Speed", Vector) = (1,5,0,0)
		_Intensity ("Global Intensity", Float) = 1
	}

	Subshader
	{
		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
			"IgnoreProjector"="True"
		}
		Pass
		{
			Fog { Mode Off }
			Lighting Off
			Blend One One
			Cull Off
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			float4 _TintColor;
			sampler2D _RandomTex;
			sampler2D _FlowingTex;
			sampler2D _NumberTex;
			float4 _CellSize;
			float4 _TexelSizes;
			float4 _Speed;
			float _Intensity;
			
			#define _RandomTexelSize (_TexelSizes.x)
			#define _FlowingTexelSize (_TexelSizes.y)
			#define _NumberCount (_TexelSizes.z)
			#define T (_Time.y)
			#define EPSILON (0.00876)

			struct appdata_v
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float3 texc : TEXCOORD0;
			};

			v2f vert (appdata_v v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos (v.vertex);
				o.texc = v.vertex.xyz;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				float3 cellc = i.texc.xyz / _CellSize + EPSILON;
				float speed = tex2D(_RandomTex, cellc.xz * _RandomTexelSize).g * 3 + 1;
				cellc.y += T*speed*_Speed.x;
				float intens = tex2D(_FlowingTex, cellc.xy * _FlowingTexelSize).r;
				
				float2 nc = cellc;
				nc.x += round(T*_Speed.y*speed);
				float number = round(tex2D(_RandomTex, nc * _RandomTexelSize).r * _NumberCount) / _NumberCount;
				
				float2 number_tex_base = float2(number, 0);
				float2 number_tex = number_tex_base + float2(frac(cellc.x/_NumberCount), frac(cellc.y));
				fixed4 ncolor = tex2Dlod(_NumberTex, float4(number_tex, 0, 0)).rgba;
				
				return ncolor * pow(intens,3) * _Intensity * _TintColor;
			}
			ENDCG
		}
	}
}
