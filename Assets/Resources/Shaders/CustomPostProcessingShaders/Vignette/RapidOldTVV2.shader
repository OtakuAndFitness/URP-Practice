Shader "Custom/PostProcessing/Vignette/RapidOldTVV2"
{
    Properties
    {
//        _VignetteColor("Color", Color) = (1, 1, 1, 1)
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float2 _RapidOldTVV2Parameters;
            // half4 _Params2;
            half4 _RapidOldTVV2Color;
        // CBUFFER_END

        #define _VignetteSize _RapidOldTVV2Parameters.x
		#define _SizeOffset _RapidOldTVV2Parameters.y

	    float4 frag(Varyings i): SV_Target
		{
			half2 uv = -i.uv * i.uv + i.uv;	     //MAD
			half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
			return VignetteIndensity * GetSource(i.uv);
		}
		
		float4 frag_ColorAdjust(Varyings i): SV_Target
		{
			half2 uv = -i.uv * i.uv + i.uv;    //MAD
			half VignetteIndensity = saturate(uv.x * uv.y * _VignetteSize + _SizeOffset);
			
			return lerp(_RapidOldTVV2Color, GetSource(i.uv), VignetteIndensity);
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
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag_ColorAdjust
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}