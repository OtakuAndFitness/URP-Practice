Shader "Custom/Distortion"
{
    Properties
    {
        _NoiseTex("NoiseTex", 2D) = "black" {}
        [HideInInspector]_MainTex("MainTex", 2D) = "white" {}
        _DistortStrength("Distort Strength", Float) = 0
        _DistortTimeFactor("Distort Time Factor", Float) = 1
        _Radius ("_Radius",float ) =1
//        _FadeTex("FadeTex", 2D) = "white" {}


    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);
            // TEXTURE2D(_FadeTex);
            // SAMPLER(sampler_FadeTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            
            CBUFFER_START(UnityPerMaterial)
                float _DistortStrength;
                float _DistortTimeFactor;
                float _Radius;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 fade = pow(1-length(IN.uv-0.5),_Radius);
                // half4 fade = SAMPLE_TEXTURE2D(_FadeTex, sampler_FadeTex, IN.uv);

                half4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uv + _Time.xy * _DistortTimeFactor);
                float2 offset = noise.xy * _DistortStrength;

                half4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, IN.uv);
                mask *= fade;
                float2 UV = IN.uv + offset * mask.r;

                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, UV);
                return col;
            }
            ENDHLSL
        }
    }
}