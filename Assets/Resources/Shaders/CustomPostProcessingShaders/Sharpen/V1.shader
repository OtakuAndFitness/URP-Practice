Shader "Custom/PostProcessing/Sharpen/SharpenV1"
{
    Properties
    {
//        _Params("Params", Vector) = (1, 1, 1, 1)
//        _MainTex("MainTex", 2D) = "white" {}
    }

    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
            float2 _SharpenV1;
        // CBUFFER_END

        #define _Strength _SharpenV1.x
        #define _Threshold _SharpenV1.y

        half4 frag(Varyings i): SV_Target
		{

			half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
			half2 halfPixelSize = pixelSize * 0.5;

			half4 blur = GetSource(i.uv + half2(halfPixelSize.x, -pixelSize.y));
			blur += GetSource(i.uv + half2(-pixelSize.x, -halfPixelSize.y));
			blur += GetSource(i.uv + half2(pixelSize.x, halfPixelSize.y));
			blur += GetSource(i.uv + half2(-halfPixelSize.x, pixelSize.y));
			blur *= 0.25;

			half4 sceneColor = GetSource(i.uv);
			half4 lumaStrength = half4(0.222, 0.707, 0.071, 0.0) * _Strength;
			half4 sharp = sceneColor - blur;

			sceneColor += clamp(dot(sharp, lumaStrength), -_Threshold, _Threshold);
			
			return sceneColor;
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
    }
}