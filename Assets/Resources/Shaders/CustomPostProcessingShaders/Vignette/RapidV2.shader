Shader "Custom/PostProcessing/Vignette/RapidOldTV"
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
            float4 _Params;
            // half4 _Params2;
            half4 _VignetteColor;
        CBUFFER_END

        #define _VignetteIndensity _Params.x
		#define _VignetteSharpness _Params.y
		#define _VignetteCenter _Params.zw

		float4 frag(Varyings i): SV_Target
		{
			
			float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
			
			half indensity = distance(i.uv.xy, _VignetteCenter.xy);
			indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
			return sceneColor * indensity;
		}
		
		
		float4 frag_ColorAdjust(Varyings i): SV_Target
		{
			
			float4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv.xy);
			
			half indensity = distance(i.uv.xy, _VignetteCenter.xy);
			indensity = smoothstep(0.8, _VignetteSharpness * 0.799, indensity * (_VignetteIndensity + _VignetteSharpness));
			
			half3 finalColor = lerp(_VignetteColor.rgb, sceneColor.rgb, indensity);
			
			return float4(finalColor.rgb, _VignetteColor.a);

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