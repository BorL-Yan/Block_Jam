Shader "Custom/Liquid"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (1, 1, 1, 1)
        _ColorBottom ("Liquid Color", Color) = (0, 0.5, 1, 1)
        _WaveSpeed ("Wave Speed", Float) = 2.0
        _WaveAmplitude ("Wave Amplitude", Float) = 0.02
        _FillLevel ("Fill Level", Float) = 0.0 // Высота уровня в мировых координатах
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        Cull Off

        Pass
        {
            Name "ForwardLit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0; // Используем мировые координаты
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorTop;
                float4 _ColorBottom;
                float _WaveSpeed;
                float _WaveAmplitude;
                float _FillLevel;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                // Преобразуем позицию из локальных координат в мировые
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                return output;
            }

            float4 frag(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_Target
            {
                // Берем мировую высоту Y. 
                // Теперь, когда вы крутите объект, эта координата все равно привязана к горизонту.
                float yPos = input.positionWS.y - _FillLevel;

                // Анимация волны
                float wave = sin(_Time.y * _WaveSpeed) * _WaveAmplitude;
                float combined = yPos + wave;

                // Маски (Step 0.0 дает четкую границу на уровне _FillLevel)
                float mask1 = step(combined, 0.05); 
                float mask2 = step(combined, 0.0);  
                float lineMask = mask1 - mask2;
                

                float4 liquidEffect = (lineMask * _ColorTop) + (mask1 * _ColorBottom);
                clip(mask1 - 0.1);

                // Если видим внутреннюю часть меша — это "поверхность" жидкости
                return isFrontFace ? liquidEffect : _ColorTop;
            }
            ENDHLSL
        }
    }
}