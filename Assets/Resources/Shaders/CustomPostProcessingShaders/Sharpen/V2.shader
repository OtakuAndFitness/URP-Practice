Shader "Custom/PostProcessing/Sharpen/SharpenV2"
{
    Properties
    {
        _Sharpness("Sharpness", Float) = 1
        _MainTex("MainTex", 2D) = "white" {}
    }

    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
            float _Sharpness;
        CBUFFER_END

        half4 frag(Varyings i) : SV_Target
	{

		half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
		pixelSize *= 1.5f;

		half4 blur = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(pixelSize.x, -pixelSize.y));
		blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-pixelSize.x, -pixelSize.y));
		blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(pixelSize.x, pixelSize.y));
		blur += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv + half2(-pixelSize.x, pixelSize.y));
		blur *= 0.25;


		half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

		return sceneColor + (sceneColor - blur) * _Sharpness;
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