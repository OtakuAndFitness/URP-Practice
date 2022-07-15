Shader "Custom/PostProcessing/ColorAdjustment/Brightness"
{
    Properties
    {
        _Brightness("Brightness", Float) = 1
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
            float _Brightness;
        CBUFFER_END

        half4 frag(Varyings i) : SV_Target
		{

			half3 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv).rgb;
			return half4(sceneColor * _Brightness, 1.0);
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