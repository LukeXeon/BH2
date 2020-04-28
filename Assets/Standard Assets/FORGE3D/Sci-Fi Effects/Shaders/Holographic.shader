// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FORGE3D/Holographic" {
Properties {
	_MainTex ("Interlace Mask", 2D) = "white" {}
	_bLayerColorA ("Tint Color A", Color) = (0.5,0.5,0.5,0.5)
	_bLayerColorB ("Tint Color B", Color) = (0.5,0.5,0.5,0.5)
	_bLayerColorC ("Tint Color C", Color) = (0.5,0.5,0.5,0.5)

	_Inter("Interlace scale: Back, X, Y | UV Speed", Vector) = (0.1,200,100,50)

	_FresPow("Surface Factor", float) = 0
	_FresMult("Surface Mult", float) = 0

	_FresPowOut("Edge Factor", float) = 0
	_FresMultOut("Edge Mult", float) = 0

	_InvFade ("Soft Fade Factor", Range(0.01,3.0)) = 1.0
	_Fade ("Fade Factor", Range(0.0,1.0)) = 1.0
}

Category {

	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend OneMinusDstColor One
	AlphaTest Greater .01
	ColorMask RGB
	Cull Back Lighting Off ZWrite Off

	SubShader {

	
		Pass {
		

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 normal: NORMAL;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD1;
				#endif
				float3 normalDir : TEXCOORD2;
				float3 posWorld : TEXCOORD3;
				float4 screenPos : TEXCOORD4;				
			};
			
			sampler2D _MainTex;

			fixed4 _bLayerColorA, _bLayerColorB, _bLayerColorC;

			float4 _MainTex_ST;
			float _FresPow, _FresMult;
			float _FresPowOut, _FresMultOut;

			float4 _Inter;

			sampler2D_float _CameraDepthTexture;
			float _InvFade;

			float _Fade;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif

				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				o.normalDir = normalize(mul(unity_ObjectToWorld, float4(v.normal.xyz,0)).xyz);
				o.posWorld = mul(unity_ObjectToWorld, v.vertex);				
				o.screenPos = ComputeScreenPos(o.vertex);

				return o;
			}			
			
			float4 frag (v2f i) : COLOR
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
		    	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);           	 
             	
		    	float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) );

             	float dist = distance(_WorldSpaceCameraPos.xyz, objectOrigin.xyz);

             	float2 wcoord = i.screenPos.xy / i.screenPos.w;
             	
             	wcoord *= dist * _Inter.x;

             	float3 nMask = tex2D(_MainTex, wcoord + float2(0, _Time.x * _Inter.w));

             	wcoord.x *= _Inter.y;
             	wcoord.y *= _Inter.z;
             	
		    	float3 hMask = tex2D(_MainTex, wcoord + float2(0, _Time.x * _Inter.w));

		    	float fresnel = pow(abs(dot(viewDirection, i.normalDir)), _FresPow) * _FresMult;
		    	float3 bLayer = lerp(_bLayerColorA, _bLayerColorB, fresnel);

		    	float fresnelOut = pow(1 - abs(dot(viewDirection, i.normalDir)), _FresPowOut) * _FresMultOut;
		    	float3 bLayerC = _bLayerColorC * fresnelOut;

		    	float3 final = saturate(bLayer + bLayerC + hMask * (bLayer + bLayerC) + nMask * (bLayer + bLayerC)) * i.color.a;
			
		    	return float4(final * _Fade, 1);
		    
		    
		    		
			}
			ENDCG 
		}
	}	
}

}
