// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FORGE3D/Billboard Nebula"
{
	Properties
	{
		_CoronaNoise("Texture", 2D) = "black" {}  
		_CoronaColor("Color", Color) = (0.5,0.5,0.5,0.5)
		_EdgeMaskFalloff("Edge Mask Falloff", float) = 0
		_EdgeMaskPower("Edge Mask Power", float) = 0
		_MinFadeStart("Min Fade Start", float) = 0
		_MinFadeEnd("Min Fade End", float) = 0
		_MaxFadeStart("Max Fade Start", float) = 0
		_MaxFadeEnd("Max Fade End", float) = 0
	}

	Category
	{
	
		
		
			
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend One One
		AlphaTest Greater .01
		ColorMask RGB
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		// Tags { "RenderType"="Opaque" }
		SubShader {	

		// Disable Dynamic Batching
		Pass{}

		Pass {

			CGPROGRAM
		 	#pragma vertex vert
            #pragma fragment frag
            #pragma only_renderers opengl d3d9 d3d11
            #pragma glsl
            #pragma target 3.0
            #pragma multi_compile_particles
            #include "UnityCG.cginc"
			
			

			float4 _CoronaColor;

			float _EdgeMaskFalloff, _EdgeMaskPower;
			float _MinFadeStart, _MinFadeEnd;
			float _MaxFadeStart, _MaxFadeEnd;

			sampler2D _CoronaNoise;

		

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;   
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput
            {	            	
                float4 pos : POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;              
                float2 uv : TEXCOORD2;   
			};

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
               	o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;               
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord0;
        
			
                return o;
            }

         

          


            float4 frag(VertexOutput i) : COLOR
            {
            	
            	float3 finalColor = tex2D(_CoronaNoise, i.uv.xy);

            	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);  
            	float viewDot = abs(dot(viewDirection, normalize(i.normalDir)));            
            	viewDot = pow(viewDot, _EdgeMaskFalloff) * _EdgeMaskPower;
            	viewDot = clamp(viewDot, 0 ,1);

            	float dist = distance(_WorldSpaceCameraPos.xyz, i.posWorld.xyz);
            	float MinFade = saturate((_MinFadeEnd - dist) / (_MinFadeStart -_MinFadeEnd)); 
            	float MaxFade = saturate((_MaxFadeEnd - dist) / (_MaxFadeEnd - _MaxFadeStart)); 

            	
                return float4(saturate(pow(finalColor * viewDot *_CoronaColor, 1.3) * 1.5 * MaxFade * MinFade), 1);

             
            }

			ENDCG	
		}
	} 	
       
}
}
