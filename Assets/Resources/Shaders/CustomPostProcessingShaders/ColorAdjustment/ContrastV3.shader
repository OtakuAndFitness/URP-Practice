Shader "Custom/PostProcessing/ColorAdjustment/ContrastV3"
{
    Properties
    {
//        _Contrast("Contrast", Vector) = (1,1,1,1)
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
			float4 ContrastV3;
        // CBUFFER_END

        half3 ColorAdjustment_Contrast_V3(float3 In, half3 ContrastFactor, float Contrast)
		{
			half3 Out = (In - ContrastFactor) * Contrast + ContrastFactor;
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			half4 finalColor = GetSource(i.uv);

			finalColor.rgb = ColorAdjustment_Contrast_V3(finalColor.rgb , half3(ContrastV3.x, ContrastV3.y, ContrastV3.z),1- (ContrastV3.w ));
			return finalColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "ContrastV3"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}