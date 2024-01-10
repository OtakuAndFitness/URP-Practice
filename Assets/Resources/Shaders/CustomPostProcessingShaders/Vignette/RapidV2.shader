Shader "Custom/PostProcessing/Vignette/RapidV2"
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
            float4 _RapidV2Parameters;
            // half4 _Params2;
            half4 _RapidV2Color;
        // CBUFFER_END

        #define _VignetteIndensity _RapidV2Parameters.x
		#define _VignetteSharpness _RapidV2Parameters.y
		#define _VignetteCenter _RapidV2Parameters.zw

		float4 frag(Varyings i): SV_Target
		{
			
			float4 sceneColor = GetSource(i.uv);
			
			half indensity = distance(i.uv, _VignetteCenter.xy);
			indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
			return sceneColor * indensity;
		}
		
		
		float4 frag_ColorAdjust(Varyings i): SV_Target
		{
			
			float4 sceneColor = GetSource(i.uv);
			
			half indensity = distance(i.uv, _VignetteCenter.xy);
			indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
			
			half3 finalColor = lerp(_RapidV2Color.rgb, sceneColor.rgb, indensity);
			
			return float4(finalColor.rgb, _RapidV2Color.a);

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