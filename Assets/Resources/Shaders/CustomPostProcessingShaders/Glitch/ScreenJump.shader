Shader "Custom/PostProcessing/Glitch/ScreenJump"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _Params("_Params", vector) = (1,1,1,1)
//        _Params2("_Params2", vector) = (1,1,1,1)
//        _Params3("_Params3", vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

		// #pragma shader_feature USING_FREQUENCY_INFINITE

        // CBUFFER_START(UnityPerMaterial)
            float2 _ScanLineJitterParams;
		    // half4 _Params2;
            // half3 _Params3;
        // CBUFFER_END

        #define _JumpIndensity _ScanLineJitterParams.x
		#define _JumpTime _ScanLineJitterParams.y


        half4 Frag_Horizontal(Varyings i): SV_Target
		{		
			float jump = lerp(i.uv.x, frac(i.uv.x + _JumpTime), _JumpIndensity);	
			half4 sceneColor = GetSource(frac(float2(jump, i.uv.y)));		
			return sceneColor;
		}
		
		half4 Frag_Vertical(Varyings i): SV_Target
		{		
			float jump = lerp(i.uv.y, frac(i.uv.y + _JumpTime), _JumpIndensity);		
			half4 sceneColor = GetSource(frac(float2(i.uv.x, jump)));	
			return sceneColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "ScreenJump Horizontal"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Horizontal
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	 Pass
        {
        	Name "ScreenJump Vertical"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Vertical
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}