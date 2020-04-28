// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FORGE3D/Warp Tunnel Distortion" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	
	_Speed("UV Speed", Float) = 1.0
	_WiggleX("_WiggleX", Float) = 1.0
	_WiggleY("_WiggleY", Float) = 1.0
	_WiggleDist("Wiggle distance", Float) = 1.0

	_Offset("Vertex offset", float) = 0

	_TintColor("Tint", Color) = (1.0, 1.0, 1.0, 1.0)
		
	_MainTex("Distortion map", 2D) = "" {}
	_Dist("Distortion ammount", Float) = 10.0	
	
}

Category {
			Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
	Cull Back Lighting Off ZWrite Off 
	Fog { Color (0,0,0,0) }
	ZTest LEqual
	
	SubShader {


GrabPass {
			Name "BASE"
			Tags { "LightMode" = "Always" }
		}

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
				float4 normal : NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float4 uvgrab : TEXCOORD2;	
				float4 projPos : TEXCOORD3;
			
			};
			
			float4 _MainTex_ST;
			float _Speed;
		
			float _WiggleX, _WiggleY, _WiggleDist;
			float _Offset;
			float _Dist;

			sampler2D _CameraDepthTexture;
		float _InvFade;

		sampler2D _GrabTexture;
		float4 _GrabTexture_TexelSize;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				v.vertex.xyz += normalize(v.normal.xyz) * _Offset;
				o.vertex = UnityObjectToClipPos(v.vertex);	
				o.vertex.x += sin(_Time.y * _WiggleX) * _WiggleDist;	
				o.vertex.y -= sin(_Time.y * _WiggleY) * _WiggleDist;		
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);


				#if UNITY_UV_STARTS_AT_TOP
					float scale = -1.0;
				#else
					float scale = 1.0;
				#endif

				o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
				o.uvgrab.zw = o.vertex.zw;

				return o;
			}

		
			
			fixed4 frag (v2f i) : SV_Target
			{
				
				i.texcoord.y += _Time.x * _Speed;
			
				float4 packedTex = tex2D(_MainTex, i.texcoord);

				float local1 = packedTex.z * 2.4;
				float2 local2 = packedTex.rg * 2.25;

				packedTex.rg = local1 * local2;

				half2 bump = UnpackNormal(packedTex).rg; 
				float2 offset = bump * _Dist * _GrabTexture_TexelSize.xy;
				i.uvgrab.xy = offset * i.uvgrab.z + i.uvgrab.xy;
		
				half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));	

				return col;
			}
			ENDCG 
		}
	}	
}
}
