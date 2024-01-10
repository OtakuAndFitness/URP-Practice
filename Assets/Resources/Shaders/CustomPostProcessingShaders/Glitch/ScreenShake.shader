Shader "Custom/PostProcessing/Glitch/ScreenShake"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _ScreenShake("_ScreenShake", Float) = 1
//        _Params2("_Params2", vector) = (1,1,1,1)
//        _Params3("_Params3", vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

		// #pragma shader_feature USING_FREQUENCY_INFINITE

        // CBUFFER_START(UnityPerMaterial)
			float _ScreenShakeParams;
			// half4 _Params2;
            // half3 _Params3;
        // CBUFFER_END

        float randomNoise(float x, float y)
		{
			return frac(sin(dot(float2(x, y), float2(127.1, 311.7))) * 43758.5453);
		}
		
		
		half4 Frag_Horizontal(Varyings i): SV_Target
		{
			float shake = (randomNoise(_Time.x, 2) - 0.5) * _ScreenShakeParams;
			
			half4 sceneColor = GetSource(frac(float2(i.uv.x + shake, i.uv.y)));
			
			return sceneColor;
		}
		
		half4 Frag_Vertical(Varyings i): SV_Target
		{
			
			float shake = (randomNoise(_Time.x, 2) - 0.5) * _ScreenShakeParams;
			
			half4 sceneColor = GetSource(frac(float2(i.uv.x, i.uv.y + shake)));
			
			return sceneColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "ScreenShake Horizontal"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Horizontal
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	 Pass
        {
			Name "ScreenShake Vertical"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Vertical
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}