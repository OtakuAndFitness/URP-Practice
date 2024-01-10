Shader "Custom/PostProcessing/ColorAdjustment/ContrastV2"
{
    Properties
    {
//        _Contrast("Contrast", Float) = 1
//    	_ContrastFactorRGB("ContrastFactorRGB", Color) = (1,1,1,1)
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
			float _ContrastV2;
			half3 _ContrastV2FactorRGB;
        // CBUFFER_END

        half3 ColorAdjustment_Contrast_V2(float3 In, half3 ContrastFactor, float Contrast)
		{
			half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			half4 finalColor = GetSource(i.uv);

			finalColor.rgb = ColorAdjustment_Contrast_V2(finalColor.rgb , _ContrastV2FactorRGB, _ContrastV2);

			return finalColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "ContrastV2"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}