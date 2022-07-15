Shader "Custom/PostProcessing/Vignette/RapidOldTVV2"
{
    Properties
    {
        _VignetteColor("Color", Color) = (1, 1, 1, 1)
        _MainTex("MainTex", 2D) = "white" {}
        _Params("Params", Vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float2 _Params;
            // half4 _Params2;
            half4 _VignetteColor;
        CBUFFER_END

        #define _VignetteSize _Params.x
		#define _SizeOffset _Params.y

	    float4 frag(Varyings i): SV_Target
		{
			half2 uv = -i.uv * i.uv + i.uv;	     //MAD
			half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
			return VignetteIndensity * SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
		}
		
		float4 frag_ColorAdjust(Varyings i): SV_Target
		{
			half2 uv = -i.uv * i.uv + i.uv;    //MAD
			half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
			
			return lerp(_VignetteColor, SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv), VignetteIndensity);
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
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment frag_ColorAdjust
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}