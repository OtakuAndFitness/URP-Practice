Shader "Custom/PostProcessing/Glitch/TileJitter"
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

    	#pragma shader_feature JITTER_DIRECTION_HORIZONTAL
		#pragma shader_feature USING_FREQUENCY_INFINITE

        // CBUFFER_START(UnityPerMaterial)
            half4 _TileJitterParams;
		    // half4 _Params2;
            // half3 _Params3;
        // CBUFFER_END

        #define _SplittingNumber _TileJitterParams.x
		#define _JitterAmount _TileJitterParams.y
		#define _JitterSpeed _TileJitterParams.z
		#define _Frequency _TileJitterParams.w


        float randomNoise(float2 c)
		{
			return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
		}

		float4 Frag_Vertical(Varyings i): SV_Target
		{
			float2 uv = i.uv.xy;
			half strength = 1.0;
			half pixelSizeX = 1.0 / _ScreenParams.x;
			
			// --------------------------------Prepare Jitter UV--------------------------------
			#if USING_FREQUENCY_INFINITE
				strength = 1;
			#else
				strength = 0.5 + 0.5 *cos(_Time.y * _Frequency);
			#endif

			if (fmod(uv.x * _SplittingNumber, 2) < 1.0)
			{
				#if JITTER_DIRECTION_HORIZONTAL
					uv.x += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
				#else
					uv.y += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
				#endif
			}

			// -------------------------------Final Sample------------------------------
			half4 sceneColor = GetSource(uv);
			return sceneColor;
		}
		
		float4 Frag_Horizontal(Varyings i): SV_Target
		{
			float2 uv = i.uv.xy;
			half strength = 1.0;
			half pixelSizeX = 1.0 / _ScreenParams.x;

			// --------------------------------Prepare Jitter UV--------------------------------
			#if USING_FREQUENCY_INFINITE
				strength = 1;
			#else
				strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
			#endif
			if(fmod(uv.y * _SplittingNumber, 2) < 1.0)
			{
				#if JITTER_DIRECTION_HORIZONTAL
					uv.x += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
				#else
					uv.y += pixelSizeX * cos(_Time.y * _JitterSpeed) * _JitterAmount * strength;
				#endif
			}

			// -------------------------------Final Sample------------------------------
			half4 sceneColor = GetSource(uv);
			return sceneColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "TileJitter Horizontal"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Horizontal
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	 Pass
        {
        	Name "TileJitter Vertical"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Vertical
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}