// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FORGE3D/Warp Tunnel" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_Speed("UV Speed", Float) = 1.0
	_WiggleX("_WiggleX", Float) = 1.0
	_WiggleY("_WiggleY", Float) = 1.0
	_WiggleDist("Wiggle distance", Float) = 1.0
	_MaxFadeStart("Max Fade Start", float) = 0
	_MaxFadeEnd("Max Fade End", float) = 0
	
}

Category {
	Tags { "Queue"="Transparent+2" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend One One
	
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				 float4 posWorld : TEXCOORD1;
			
			};
			
			float4 _MainTex_ST;
			float _Speed;
			float _MaxFadeStart, _MaxFadeEnd;
			float _WiggleX, _WiggleY, _WiggleDist;

			v2f vert (appdata_t v)
			{
				v2f o;
				 o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);	
				o.vertex.x += sin(_Time.y * _WiggleX) * _WiggleDist;	
				o.vertex.y -= sin(_Time.y * _WiggleY) * _WiggleDist;		
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

		
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				i.texcoord.y += _Time.x * _Speed;
				float dist = distance(_WorldSpaceCameraPos.xyz, i.posWorld.xyz);
				float MaxFade = saturate((_MaxFadeEnd - dist) / (_MaxFadeEnd - _MaxFadeStart)); 
				return (_TintColor.a * 20) * _TintColor * tex2D(_MainTex, i.texcoord) * MaxFade;
			}
			ENDCG 
		}
	}	
}
}
