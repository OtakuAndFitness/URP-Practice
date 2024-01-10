Shader "Custom/PostProcessing/ColorAdjustment/Saturation"
{
    Properties
    {
//        _Saturation("Saturation", Float) = 1
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
            float _Saturation;
        // CBUFFER_END

        half3 Saturation(half3 In, half Saturation)
		{
			half luma = dot(In, half3(0.2126729, 0.7151522, 0.0721750));
			half3 Out = luma.xxx + Saturation.xxx * (In - luma.xxx);
			return Out;
		}

		half4 frag(Varyings i) : SV_Target
		{

			// half3 col = 0.5 + 0.5 * cos(_Time.y + i.uv.xyx + half3(0, 2, 4));

			half4 sceneColor = GetSource(i.uv);

			return half4(Saturation(sceneColor.rgb, _Saturation), 1.0);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Saturation"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}