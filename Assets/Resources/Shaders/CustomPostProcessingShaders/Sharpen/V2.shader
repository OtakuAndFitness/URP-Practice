Shader "Custom/PostProcessing/Sharpen/SharpenV2"
{
    Properties
    {
//        _Sharpness("Sharpness", Float) = 1
//        _MainTex("MainTex", 2D) = "white" {}
    }

    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
            float _SharpenV2;
        // CBUFFER_END

        half4 frag(Varyings i) : SV_Target
	{

		half2 pixelSize = float2(1 / _ScreenParams.x, 1 / _ScreenParams.y);
		pixelSize *= 1.5f;

		half4 blur = GetSource(i.uv + half2(pixelSize.x, -pixelSize.y));
		blur += GetSource(i.uv + half2(-pixelSize.x, -pixelSize.y));
		blur += GetSource(i.uv + half2(pixelSize.x, pixelSize.y));
		blur += GetSource(i.uv + half2(-pixelSize.x, pixelSize.y));
		blur *= 0.25;


		half4 sceneColor = GetSource(i.uv);

		return sceneColor + (sceneColor - blur) * _SharpenV2;
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