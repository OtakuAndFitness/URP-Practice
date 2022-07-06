Shader "Custom/PostProcessing/Blur/KawaseBlur"
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
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "../CustomPPHeader.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_TexelSize;
                float _Offset;
                // float4 _BaseMap_ST;
                // half4 _BaseColor;
            CBUFFER_END

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