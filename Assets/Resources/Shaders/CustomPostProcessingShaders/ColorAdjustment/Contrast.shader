Shader "Custom/PostProcessing/ColorAdjustment/Cotrast"
{
    Properties
    {
        _Contrast("Contrast", Float) = 1
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
			float _Contrast;
        CBUFFER_END

        half3 ColorAdjustment_Contrast(half3 In, half Contrast)
		{
			half midpoint = 0.21763h;//pow(0.5, 2.2);
			half3 Out = (In - midpoint) * Contrast + midpoint;
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			half4 finalColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

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
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}