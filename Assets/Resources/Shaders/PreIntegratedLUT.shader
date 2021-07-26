Shader "Otaku/PreIntegratedLUT"
{
    Properties
    {
        _Max1R("Max 1/Radiance", float) = 1
        [KeywordEnum(NormDiff, G1, G2)] _Radiance ("Radiance Profile type", Float) = 0
        _D ("D", Vector) = (1,1,1,1)
        _DiffColor1 ("NormDiff Color 1", Color) = (1,1,1,1)
        _DiffColor2 ("NormDiff Color 2", Color) = (0,0,0,0)
        _DiffColor3 ("NormDiff Color 2", Color) = (0,0,0,0)
        _DiffColor4 ("NormDiff Color 2", Color) = (0,0,0,0)
        [KeywordEnum(None, Uncharted, ACES)] _Tone ("Tone type", Float) = 0
        _AdaptedLum ("AdaptedLum", float) = 1
        [Toggle] _Gamma ("Gamma", Float) = 0
 
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline"="UniversalPipeline"}

        Pass
        {

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_cbuffer

            #pragma shader_feature _GAMMA_ON
            #pragma multi_compile _RADIANCE_NORMDIFF _RADIANCE_G1 _RADIANCE_G2
            #pragma multi_compile _TONE_NONE _TONE_UNCHARTED _TONE_ACES

            #include "LUTCore.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };


            Varyings vert (Attributes input)
            {
                Varyings output;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);                
                output.positionCS = vertexInput.positionCS;

                output.uv = input.uv;
                
                return output;
            }

            half4 frag (Varyings i) : SV_Target
            {
                float radian = 1.0 / (i.uv.y * _Max1R + 0.0001);

                float3 color = IntegratedLUT(lerp(-1,1,i.uv.x),radian);
                return float4(color,1);
            }
            ENDHLSL
        }

        
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"

}
