Shader "Custom/PostProcessing/ColorAdjustment/ContrastV3"
{
    Properties
    {
        _Contrast("Contrast", Vector) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
			float4 _Contrast;
        CBUFFER_END

        half3 ColorAdjustment_Contrast_V3(float3 In, half3 ContrastFactor, float Contrast)
		{
			half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			half4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

			finalColor.rgb = ColorAdjustment_Contrast_V3(finalColor.rgb , half3(_Contrast.x, _Contrast.y, _Contrast.z),1- (_Contrast.w ));
			return finalColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}