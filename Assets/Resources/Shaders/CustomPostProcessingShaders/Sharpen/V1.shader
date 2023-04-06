Shader "Custom/PostProcessing/Sharpen/SharpenV1"
{
    Properties
    {
        _Params("Params", Vector) = (1, 1, 1, 1)
        _MainTex("MainTex", 2D) = "white" {}
    }

    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
            float2 _Params;
        CBUFFER_END

        #define _Strength _Params.x
        #define _Threshold _Params.y

        half4 frag(Varyings i): SV_Target
		{

			half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
			half2 halfPixelSize = pixelSize * 0.5;

			half4 blur = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(halfPixelSize.x, -pixelSize.y));
			blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-pixelSize.x, -halfPixelSize.y));
			blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(pixelSize.x, halfPixelSize.y));
			blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-halfPixelSize.x, pixelSize.y));
			blur *= 0.25;

			half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
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
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}