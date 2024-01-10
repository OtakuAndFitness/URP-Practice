Shader "Custom/PostProcessing/ColorAdjustment/Tint"
{
    Properties
    {
//        _Indensity("Indensity", Float) = 1
//    	_ColorTint("ColorTint", Color) = (1,1,1,1)
//        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
        // CBUFFER_START(UnityPerMaterial)
            float _TintIndensity;
			half4 _ColorTint;
        // CBUFFER_END

        half4 frag(Varyings i): SV_Target
		{	
			half4 sceneColor = GetSource(i.uv);
			
			half3 finalColor = lerp(sceneColor.rgb, sceneColor.rgb * _ColorTint.rgb, _TintIndensity);
			
			return half4(finalColor, 1.0);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Tint"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}