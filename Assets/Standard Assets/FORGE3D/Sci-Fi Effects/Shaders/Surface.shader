// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FORGE3D/Surface" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "black" {}
        _Normal ("Normal", 2D) = "white" {}      
        _Cubemap ("Cubemap", Cube) = "_Skybox" {}
        _Glow ("Glowmap", 2D) = "black" {}
        _DiffusePower ("Diffuse Power", Float) = 1
        _SpecularAdd ("Specular Add", Float) = 0
        _SpecularMult ("Specular Mult", Float) = 0
        _SpecularA ("Specular A", Float) = 0
        _SpecularB ("Specular B", Float) = 1
        _SpecularRatio ("Specular Ratio", Float) = 5.276861
        _ReflectionMult ("Reflection Mult", Float) = 0
        _ReflectionDesaturation ("Reflection Desaturation", Float) = 0
        _FresnelPower ("Fresnel Power", Float) = 0
        _FresnelMult ("Fresnel Mult", Float) = 0     
        _GlowMult ("Glow", Float) = 0       
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        Pass
        {
            Name "ForwardBase"
            Tags
            {
                "LightMode"="ForwardBase"
            }            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0

            uniform float4 _LightColor0;

            uniform sampler2D _Normal; uniform float4 _Normal_ST;           
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Glow; uniform float4 _Glow_ST;

            uniform samplerCUBE _Cubemap;

            uniform float _DiffusePower;
            uniform float _SpecularRatio;
            uniform float _SpecularA;
            uniform float _SpecularB;            
            uniform float _SpecularMult;
            uniform float _SpecularAdd;
           
            uniform float _ReflectionMult;
            uniform float _ReflectionDesaturation;
            uniform float _FresnelPower;
            uniform float _FresnelMult;          
           
            uniform float _GlowMult;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };

            // Vertex
            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = TRANSFORM_TEX( v.texcoord0, _Diffuse );
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }

            // Spherical Gaussian Power Function
            float powx(float x, float n)
			{
				n = n * 1.4427f + 1.4427f;
				return exp2(x * n - n);
			}

			// Desaturate
			float Desaturate(float3 color)
			{
				return color.r * 0.299 + color.g * 0.587 + color.b * 0.114;
			}

			// Fragment
            float4 frag(VertexOutput i) : COLOR
            {
            	// normal and transforms
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                
                // normal calculation
                float4 normalMap = tex2D(_Normal, i.uv0);
                float2 n = ((float2(normalMap.g,normalMap.a).rg*2.0)-1.0).rg;
                float3 normalLocal = mul( tangentTransform, normalize((normalize(i.normalDir)+(normalize(i.tangentDir)*n.g)+(normalize(i.binormalDir)*n.r))) ).xyz.rgb;
                float3 normalDir = normalize(mul( normalLocal, tangentTransform ));

                // light
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float attenuation = LIGHT_ATTENUATION(i);
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

                // view, half, reflect
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            	float3 reflectDir = reflect( -viewDir, normalDir );                
                float3 halfDir = normalize(viewDir + lightDir);

                float4 diffuseMap = tex2D(_Diffuse, i.uv0);               

                float ndl = pow(saturate(dot(normalDir, lightDir)), _DiffusePower);

                float specDot = max(0.0, dot(normalDir, halfDir));
                float spec = max(0, _SpecularRatio) * powx(specDot, max(0, _SpecularA)) + powx(specDot, max(0, _SpecularB)) * diffuseMap.a;
                spec = saturate(spec) * ndl * attenuation;

                float3 diffuse = (diffuseMap.rgb * attenuation) *
                				((ndl + _SpecularMult * spec) * _LightColor0.rgb) +
            					_SpecularAdd * spec * _LightColor0.rgb;

				float3 fresnel = saturate(powx(saturate(1 - abs(dot(viewDir, normalDir))), _FresnelPower) * _FresnelMult) * diffuseMap.a * _LightColor0.rgb;

				float4 cubeMap = texCUBE(_Cubemap, reflectDir);
				float dCubeMap = Desaturate(cubeMap.rgb);
				float3 reflection = max(0.0, _ReflectionMult) * lerp(dCubeMap.xxx, cubeMap.rgb, saturate(_ReflectionDesaturation)) * diffuseMap.a;

				float4 glowMap = tex2D(_Glow, i.uv0);

				float3 glowColor = glowMap.rgb * _GlowMult;				

				float3 ambientColor = (diffuseMap.rgb * ambient);

                float3 finalColor = ambientColor + diffuse + reflection + fresnel + glowColor;

                return float4(finalColor,1);
            }
            ENDCG
        }

        Pass
        {
            Name "ForwardAdd"
            Tags
            {
                "LightMode"="ForwardAdd"
            }

            Blend One One  
            

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0

         	uniform float4 _LightColor0;

            uniform sampler2D _Normal; uniform float4 _Normal_ST;           
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform sampler2D _Glow; uniform float4 _Glow_ST;

            uniform samplerCUBE _Cubemap;

            uniform float _DiffusePower;
            uniform float _SpecularRatio;
            uniform float _SpecularA;
            uniform float _SpecularB;            
            uniform float _SpecularMult;
            uniform float _SpecularAdd;
           
            uniform float _ReflectionMult;
            uniform float _ReflectionDesaturation;
            uniform float _FresnelPower;
            uniform float _FresnelMult;
          
            uniform float4 _GlowColor;
            uniform float _HullGlow;
            uniform float _EngGlow;

            struct VertexInput
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };

            VertexOutput vert (VertexInput v)
            {
                VertexOutput o;
                o.uv0 = TRANSFORM_TEX( v.texcoord0, _Diffuse );
                o.normalDir = mul(float4(v.normal,0), unity_WorldToObject).xyz;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }

              // Spherical Gaussian Power Function
            float powx(float x, float n)
			{
				n = n * 1.4427f + 1.4427f;
				return exp2(x * n - n);
			}

			// Desaturate
			float Desaturate(float3 color)
			{
				return color.r * 0.299 + color.g * 0.587 + color.b * 0.114;
			}


            float4 frag(VertexOutput i) : COLOR
            {	
            	// normal and transforms
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                
                // normal calculation
                float4 normalMap = tex2D(_Normal, i.uv0);
                float2 n = ((float2(normalMap.g,normalMap.a).rg*2.0)-1.0).rg;
                float3 normalLocal = mul( tangentTransform, normalize((normalize(i.normalDir)+(normalize(i.tangentDir)*n.g)+(normalize(i.binormalDir)*n.r))) ).xyz.rgb;
                float3 normalDir = normalize(mul( normalLocal, tangentTransform ));

                // light
                float3 lightDir = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float attenuation = LIGHT_ATTENUATION(i);
                float3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;

                // view, half, reflect
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
            	float3 reflectDir = reflect( -viewDir, normalDir );                
                float3 halfDir = normalize(viewDir + lightDir);

         		 float4 diffuseMap = tex2D(_Diffuse, i.uv0);               

                float ndl = max(0.0, pow(dot(normalDir, lightDir), _DiffusePower));

                float specDot = max(0.0, dot(normalDir, halfDir));
                float spec = max(0, _SpecularRatio) * powx(specDot, max(0, _SpecularA)) + powx(specDot, max(0, _SpecularB)) * diffuseMap.a;
                spec = saturate(spec) * ndl;

                float3 diffuse = (diffuseMap.rgb * attenuation) *
                				((ndl + _SpecularMult * spec) * _LightColor0.rgb) +
            					_SpecularAdd * spec * _LightColor0.rgb;

				float3 fresnel = saturate(powx(saturate(1 - abs(dot(viewDir, normalDir))), _FresnelPower) * _FresnelMult) * diffuseMap.a * _LightColor0.rgb;

				float4 cubeMap = texCUBE(_Cubemap, reflectDir);
				float dCubeMap = Desaturate(cubeMap.rgb);
				float3 reflection = max(0.0, _ReflectionMult) * lerp(dCubeMap.xxx, cubeMap.rgb, saturate(_ReflectionDesaturation)) * diffuseMap.a;

				

				float3 ambientColor = (diffuseMap.rgb * ambient);

                float3 finalColor = saturate((diffuse + fresnel) * attenuation * _LightColor0.rgb);

                
                return float4(finalColor, 1);

            }
            ENDCG
        }
    }

    FallBack "Diffuse"
    
}
