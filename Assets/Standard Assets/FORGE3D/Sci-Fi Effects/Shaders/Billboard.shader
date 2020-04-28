// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FORGE3D/Billboard Additive" {
Properties {
	_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	AlphaTest Greater .01
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
			fixed4 _Color;
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD1;
				#endif
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;

	            float4 ori = mul(UNITY_MATRIX_MV,float4(0,0,0,1));
	            float4 vt = v.vertex;	            
	            float2 r1 = float2(unity_ObjectToWorld[0][0],unity_ObjectToWorld[0][2]);
	            float2 r2 = float2(unity_ObjectToWorld[2][0],unity_ObjectToWorld[2][2]);
	            float2 vt0 = vt.x*r1;
	            vt0 += vt.z*r2;
	            vt.xy = vt0;
	            vt.z = 0;
	            vt.xyz += ori.xyz;
	            o.vertex = mul(UNITY_MATRIX_P,vt);	 
	            o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
	            o.color = v.color;

	            #ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif

	            return o;
			}

			sampler2D_float _CameraDepthTexture;
			float _InvFade;
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				return i.color * _Color * tex2D(_MainTex, i.texcoord);
			}
			ENDCG 
		}
	}	
}
}
/*
Shader "Tut/Project/Billboard_2" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        pass{
        Cull Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        sampler2D _MainTex;
        struct v2f {
            float4 pos:SV_POSITION;
            float2 texc:TEXCOORD0;
        };
        v2f vert(appdata_base v)
        {
            v2f o;
            float4 ori=mul(UNITY_MATRIX_MV,float4(0,0,0,1));
            float4 vt=v.vertex;
            //vt.y=vt.z;
            float2 r1=float2(_Object2World[0][0],_Object2World[0][2]);
            float2 r2=float2(_Object2World[2][0],_Object2World[2][2]);
            float2 vt0=vt.x*r1;
            vt0+=vt.z*r2;
            vt.xy=vt0;
            vt.z=0;
            vt.xyz+=ori.xyz;//result is vt.z==ori.z ,so the distance to camera keeped ,and screen size keeped
            o.pos=mul(UNITY_MATRIX_P,vt);
 
            o.texc=v.texcoord;
            return o;
        }
        float4 frag(v2f i):COLOR
        {
            return tex2D(_MainTex,i.texc);
        }
        ENDCG
        }//endpass
    }
}*/