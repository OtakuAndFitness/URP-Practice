Shader "Custom/PostProcessing/ColorAdjustment/LensFilter"
{
    Properties
    {
//        _Indensity("Indensity", Float) = 1
//    	_LensColor("LensColor", Color) = (1,1,1,1)
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
			float _LensFilterIndensity;
            half4 _LensColor;
        // CBUFFER_END

        half luminance(half3 color)
		{
			return dot(color, half3(0.222, 0.707, 0.071));
		}

		half4 frag(Varyings i): SV_Target
		{
			half4 sceneColor = GetSource(i.uv);

			half lum = luminance(sceneColor.rgb);

			// Interpolate with half4(0.0, 0.0, 0.0, 0.0) based on luminance
			half4 filterColor = lerp(half4(0.0, 0.0, 0.0, 0.0), _LensColor, saturate(lum * 2.0));

			// Interpolate withhalf4(1.0, 1.0, 1.0, 1.0) based on luminance
			filterColor = lerp(filterColor, half4(1.0, 1.0, 1.0, 1.0), saturate(lum - 0.5) * 2.0);

			filterColor = lerp(sceneColor, filterColor, saturate(lum * _LensFilterIndensity));

			return half4(filterColor.rgb, sceneColor.a);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Lens Filter"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}