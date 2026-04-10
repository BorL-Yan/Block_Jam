Shader "Custom/Outline"
{
    Properties
    {
        // ===== BASE =====
        [MainTexture] _MainTex ("Base Map", 2D) = "white" {}
        [MainColor]   _BaseColor ("Base Color", Color) = (1,1,1,1)

        // ===== PBR =====
        _Metallic   ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5

        [HDR] _EmissionColor ("Emission Color", Color) = (0,0,0,0)

        // ===== OUTLINE =====
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize  ("Outline Size", Float) = 0.05
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType"="Lit"
        }
        
        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _EmissionColor;
            float4 _OutlineColor;
            float  _OutlineSize;
            float  _Metallic;
            float  _Smoothness;
        CBUFFER_END
        ENDHLSL

        // =================================================
        // PASS 1 — OUTLINE (UNLIT)
        // =================================================
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="SRPDefaultUnlit" }

            Cull Front
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vertOut
            #pragma fragment fragOut
            #pragma multi_compile_instancing
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vertOut(Attributes v)
            {
                Varyings o;
                float3 pos = v.positionOS.xyz + normalize(v.normalOS) * _OutlineSize;
                o.positionHCS = TransformObjectToHClip(pos);
                return o;
            }

            half4 fragOut(Varyings i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // =================================================
        // PASS 2 — LIT (ОСНОВНОЙ)
        // =================================================
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD0;
                float3 normalWS    : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS  = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS    = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // ===== Surface Data =====
                SurfaceData surfaceData;
                ZERO_INITIALIZE(SurfaceData, surfaceData);

                half4 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                albedo *= _BaseColor;

                surfaceData.albedo     = albedo.rgb;
                surfaceData.alpha      = albedo.a;
                surfaceData.metallic   = _Metallic;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS   = half3(0,0,1);
                surfaceData.occlusion  = 1.0;
                surfaceData.emission   = _EmissionColor.rgb;

                // ===== Input Data =====
                InputData inputData;
                ZERO_INITIALIZE(InputData, inputData);

                inputData.positionWS      = i.positionWS;
                inputData.normalWS        = normalize(i.normalWS);
                inputData.viewDirectionWS = normalize(GetWorldSpaceViewDir(i.positionWS));
                inputData.shadowCoord     = TransformWorldToShadowCoord(i.positionWS);
                inputData.bakedGI         = SampleSH(inputData.normalWS);

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }   
}