Shader "Custom/PostProcessing/ColorAdjustment/BleachBypass"
{
    Properties
    {
//        _Indensity("Indensity", Float) = 1
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
            float _BleachBypassIndensity;
        // CBUFFER_END

        half luminance(half3 color)
		{
			return dot(color, half3(0.222, 0.707, 0.071));
		}
		
		//reference : https://developer.download.nvidia.com/shaderlibrary/webpages/shader_library.html
		half4 frag(Varyings i): SV_Target
		{	
			half4 color = GetSource(i.uv);
			half lum = luminance(color.rgb);
			half3 blend = half3(lum, lum, lum);
			half L = min(1.0, max(0.0, 10.0 * (lum - 0.45)));
			half3 result1 = 2.0 * color.rgb * blend;
			half3 result2 = 1.0 - 2.0 * (1.0 - blend) * (1.0 - color.rgb);
			half3 newColor = lerp(result1, result2, L);
			
			return lerp(color, half4(newColor, color.a), _BleachBypassIndensity);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "BleachBypass"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}