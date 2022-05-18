Shader "Custom/ScreenBroken"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScreenBrokenNormal("Normal Tex", 2D) = "white" {}
        _BrokenScale("Broken Scale", Range(0,1)) = 1

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
                float2 uvNormal : TEXCOORD1;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float2 uvNormal : TEXCOORD1;
                float4 positionHCS : SV_POSITION;
            };

            TEXTURE2D(_ScreenBrokenNormal);
            SAMPLER(sampler_ScreenBrokenNormal);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _ScreenBrokenNormal_ST;
                half _BrokenScale;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                OUT.uvNormal = TRANSFORM_TEX(IN.uvNormal, _ScreenBrokenNormal);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
		        half3 normalTangent = UnpackNormal(SAMPLE_TEXTURE2D(_ScreenBrokenNormal, sampler_ScreenBrokenNormal, IN.uvNormal));
                normalTangent.xy *= _BrokenScale;
                IN.uv += normalTangent.xy;
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                // half luminance = (col.r + col.g + col.b) / 3;
				// half3 finalCol = lerp(half3(luminance,luminance,luminance),col,0.25);
                return col;
            }
            ENDHLSL
        }
    }
}