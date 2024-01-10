Shader "Custom/PostProcessing/ColorAdjustment/Contrast"
{
    Properties
    {
//        _Contrast("Contrast", Float) = 1
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
			float _Contrast;
        // CBUFFER_END

        half3 ColorAdjustment_Contrast(half3 In, half Contrast)
		{
			half midpoint = 0.21763h;//pow(0.5, 2.2);
			half3 Out = (In - midpoint) * Contrast + midpoint;
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			half4 finalColor = GetSource(i.uv);

			finalColor.rgb = ColorAdjustment_Contrast(finalColor.rgb , _Contrast);

			return finalColor;

		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Contrast"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}