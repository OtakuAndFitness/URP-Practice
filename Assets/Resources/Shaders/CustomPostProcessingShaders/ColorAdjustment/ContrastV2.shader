Shader "Custom/PostProcessing/ColorAdjustment/ContrastV2"
{
    Properties
    {
        _Contrast("Contrast", Float) = 1
    	_ContrastFactorRGB("ContrastFactorRGB", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
			float _Contrast;
			half3 _ContrastFactorRGB;
        CBUFFER_END

        half3 ColorAdjustment_Contrast_V2(float3 In, half3 ContrastFactor, float Contrast)
		{
			half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			half4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

			finalColor.rgb = ColorAdjustment_Contrast_V2(finalColor.rgb , _ContrastFactorRGB,_Contrast);

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