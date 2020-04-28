// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FORGE3D/Burnout" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "black" {}
        _Normal ("Normal", 2D) = "white" {}      
    
        _DiffusePower ("Diffuse Power", Float) = 1
        _SpecularAdd ("Specular Add", Float) = 0
        _SpecularMult ("Specular Mult", Float) = 0
        _SpecularA ("Specular A", Float) = 0
        _SpecularB ("Specular B", Float) = 1
        _SpecularRatio ("Specular Ratio", Float) = 5.276861
       
        _FresnelPower ("Fresnel Power", Float) = 0
        _FresnelMult ("Fresnel Mult", Float) = 0     
        _FresnelColor("Fresnel Color", Color) = (4.0, 0.7, 0.004, 1.0)

        //
        _Cut ("AlphaTest Cutoff", Float) = 1.0  
        _LoopTex("Loop Texture", 2D) = "white" {}
        _MaskTex("Corrupt Mask", 2D) = "white" {}
        _BurnoutMask("Burnout mask", 2D) = ""{}
        _WipeEm("Buronout Emission mask", 2D) = ""{}
        _WipeOp("Burnout Opacity mask", 2D) = ""{}

        _BurnColor("Burn Color", Color) = (4.0, 0.7, 0.004, 1.0)

        _FresnelExp("Burnout fresnel Power", Float) = 0.0   

        _BurnOut("Burnout", Float) = 0.0
        _BurnUVOffset("Burnout offset", Float) = 0.0
        
      
    }
    SubShader
    {
        //AlphaTest Greater [_Cut]
        Blend One OneMinusSrcAlpha      
        Tags { "RenderType"="Transparent" }
        Cull Off

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
    
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"         
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0

            uniform float4 _LightColor0;

            uniform sampler2D _Normal; uniform float4 _Normal_ST;           
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
          //  uniform sampler2D _Glow; uniform float4 _Glow_ST;

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
            uniform float4 _FresnelColor;       
           
            //

            float _BurnOut;
            float _FresnelExp;
            float _BurnUVOffset;

            sampler2D _BurnoutMask;
            sampler2D _LoopTex;
            sampler2D _MaskTex;
            sampler2D _WipeEm;
            sampler2D _WipeOp;

            float4 _BurnColor;

            float _Cut;

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
              //  TRANSFER_VERTEX_TO_FRAGMENT(o)
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
                float spec = max(0, _SpecularRatio) * powx(specDot, max(0, _SpecularA)) + powx(specDot, max(0, _SpecularB)) * diffuseMap.g;
                spec = saturate(spec) * ndl * attenuation;

                float3 diffuse = (diffuseMap.rgb * attenuation) *
                                ((ndl + _SpecularMult * spec) * _LightColor0.rgb) +
                                _SpecularAdd * spec * _LightColor0.rgb;

                float3 fresnel = saturate(powx(saturate(1 - abs(dot(viewDir, normalDir))), _FresnelPower) * _FresnelMult) * diffuseMap.a * _FresnelColor;

                float3 ambientColor = (diffuseMap.rgb * ambient);
                float3 finalColor = ambientColor + diffuse + fresnel;

                //

                float2 uvPan = float2(i.uv0.x + _Time.x * 1, i.uv0.y);
                float4 loop_ = tex2D (_LoopTex, uvPan * 2) * 200;
                float4 crptMask = tex2D (_MaskTex, i.uv0 * 1);

                float localFresnel = saturate(dot(normalize(viewDir), normalize(normalDir)));   
                
                float2 c = float2(0.0, 1 ) * _BurnOut + pow((1 - localFresnel), _FresnelExp);
                c = float2(0.0, 1.2) - c;
                
                float3 wipe = tex2D(_WipeEm, c);
                float3 opacityWipe = tex2D(_WipeOp, (1 - (c + _BurnUVOffset)));

                float wipeMask = crptMask.r * opacityWipe.r;

                float4 crptMask_g = crptMask.g * float4(15.0, 1.0, 0.0, 0.0);
                float4 crptMask_b = crptMask.b * float4(15.0, 1.0, 0.0, 0.0);
                
                float4 loop1 = (loop_ * crptMask_g) + crptMask_b;   

                float3 burnoutMask =  tex2D(_BurnoutMask, i.uv0);
                float3 burnwipe = dot( burnoutMask, float3(0.22, 0.707, 0.071) ) * wipe * _BurnColor.rgb * _BurnColor.a * 300;

                clip(wipeMask - _Cut);
            
                return float4(loop1.rgb + burnwipe + finalColor, 1);               
            
            }
            ENDCG
        }

       
       
     
    }

    FallBack "Transparent"
    
}
