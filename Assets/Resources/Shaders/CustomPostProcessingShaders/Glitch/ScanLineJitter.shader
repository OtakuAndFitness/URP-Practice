Shader "Custom/PostProcessing/Glitch/ScanLineJitter"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
        _Params("_Params", vector) = (1,1,1,1)
//        _Params2("_Params2", vector) = (1,1,1,1)
//        _Params3("_Params3", vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"

		#pragma shader_feature USING_FREQUENCY_INFINITE

        CBUFFER_START(UnityPerMaterial)
            half3 _Params;
		    // half4 _Params2;
            // half3 _Params3;
        CBUFFER_END

        #define _Amount _Params.x
		#define _Threshold _Params.y
		#define _Frequency _Params.z


        float randomNoise(float x, float y)
		{
			return frac(sin(dot(float2(x, y), float2(12.9898, 78.233))) * 43758.5453);
		}
		
		
		half4 Frag_Horizontal(Varyings i): SV_Target
		{
			half strength = 0;
			#if USING_FREQUENCY_INFINITE
				strength = 1;
			#else
				strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
			#endif
			
			
			float jitter = randomNoise(i.uv.y, _Time.x) * 2 - 1;
			jitter *= step(_Threshold, abs(jitter)) * _Amount * strength;
			
			half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(i.uv + float2(jitter, 0)));
			
			return sceneColor;
		}
		
		half4 Frag_Vertical(Varyings i): SV_Target
		{
			half strength = 0;
			#if USING_FREQUENCY_INFINITE
				strength = 1;
			#else
				strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
			#endif
			
			float jitter = randomNoise(i.uv.x, _Time.x) * 2 - 1;
			jitter *= step(_Threshold, abs(jitter)) * _Amount * strength;
			
			half4 sceneColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, frac(i.uv + float2(0, jitter)));
			
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
            #pragma fragment Frag_Horizontal
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	 Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment Frag_Vertical
            #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}