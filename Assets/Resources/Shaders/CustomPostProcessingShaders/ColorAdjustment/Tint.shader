Shader "Custom/PostProcessing/ColorAdjustment/Tint"
{
    Properties
    {
        _Indensity("Indensity", Float) = 1
    	_ColorTint("ColorTint", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
            float _Indensity;
			half4 _ColorTint;
        CBUFFER_END

        half4 frag(Varyings i): SV_Target
		{	
			half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
			
			half3 finalColor = lerp(sceneColor.rgb, sceneColor.rgb * _ColorTint.rgb, _Indensity);
			
			return half4(finalColor, 1.0);
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