Shader "Custom/PostProcessing/KawaseBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
        _Offset("Offset", Float) = 1
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZTest Always
        Cull Off
        Zwrite Off

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Offset;
                // float4 _BaseMap_ST;
                // half4 _BaseColor;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                half4 o = 0;
		        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(_Offset + 0.5, _Offset + 0.5) * _MainTex_TexelSize.xy); 
		        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-_Offset - 0.5, _Offset + 0.5) * _MainTex_TexelSize.xy); 
		        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(-_Offset - 0.5, -_Offset - 0.5) * _MainTex_TexelSize.xy); 
		        o += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + float2(_Offset + 0.5, -_Offset - 0.5) * _MainTex_TexelSize.xy); 
		        return o * 0.25;
            }
            ENDHLSL
        }
    }
}