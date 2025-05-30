Shader "Custom/PostProcessing/Glitch/WaveJitter"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _Params("_Params", vector) = (1,1,1,1)
//        _Resolution("_Resolution", vector) = (1,1,1,1)
//        _Params3("_Params3", vector) = (1,1,1,1)
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"
		#include "../NoiseLibrary.hlsl"

    	#pragma shader_feature JITTER_DIRECTION_HORIZONTAL
		#pragma shader_feature USING_FREQUENCY_INFINITE

        // CBUFFER_START(UnityPerMaterial)
            float4 _WaveJitterParams;
		    float2 _WaveJitterResolution;
            // half3 _Params3;
        // CBUFFER_END

        #define _Frequency _WaveJitterParams.x
		#define _RGBSplit _WaveJitterParams.y
		#define _Speed _WaveJitterParams.z
		#define _Amount _WaveJitterParams.w


        float4 Frag_Horizontal(Varyings i): SV_Target
		{
			half strength = 0.0;
			#if USING_FREQUENCY_INFINITE
				strength = 1;
			#else
				strength = 0.5 + 0.5 *cos(_Time.y * _Frequency);
			#endif
			
			// Prepare UV
			float uv_y = i.uv.y * _WaveJitterResolution.y;
			float noise_wave_1 = snoise(float2(uv_y * 0.01, _Time.y * _Speed * 20)) * (strength * _Amount * 32.0);
			float noise_wave_2 = snoise(float2(uv_y * 0.02, _Time.y * _Speed * 10)) * (strength * _Amount * 4.0);
			float noise_wave_x = noise_wave_1 * noise_wave_2 / _WaveJitterResolution.x;
			float uv_x = i.uv.x + noise_wave_x;

			float rgbSplit_uv_x = (_RGBSplit * 50 + (20.0 * strength + 1.0)) * noise_wave_x / _WaveJitterParams.x;

			// Sample RGB Color-
			half4 colorG = GetSource(float2(uv_x, i.uv.y));
			half4 colorRB = GetSource(float2(uv_x + rgbSplit_uv_x, i.uv.y));
			
			return  half4(colorRB.r, colorG.g, colorRB.b, colorRB.a + colorG.a);
		}

		float4 Frag_Vertical(Varyings i) : SV_Target
		{
			half strength = 0.0;
			#if USING_FREQUENCY_INFINITE
				strength = 1;
			#else
				strength = 0.5 + 0.5 * cos(_Time.y * _Frequency);
			#endif

			// Prepare UV
			float uv_x = i.uv.x * _WaveJitterParams.x;
			float noise_wave_1 = snoise(float2(uv_x * 0.01, _Time.y * _Speed * 20)) * (strength * _Amount * 32.0);
			float noise_wave_2 = snoise(float2(uv_x * 0.02, _Time.y * _Speed * 10)) * (strength * _Amount * 4.0);
			float noise_wave_y = noise_wave_1 * noise_wave_2 / _WaveJitterParams.x;
			float uv_y = i.uv.y + noise_wave_y;

			float rgbSplit_uv_y = (_RGBSplit * 50 + (20.0 * strength + 1.0)) * noise_wave_y / _WaveJitterParams.y;

			// Sample RGB Color
			half4 colorG = GetSource(float2(i.uv.x, uv_y));
			half4 colorRB = GetSource(float2(i.uv.x, uv_y + rgbSplit_uv_y));

			return half4(colorRB.r, colorG.g, colorRB.b, colorRB.a + colorG.a);
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
            #pragma fragment Frag_Horizontal
            // #pragma multi_compile_instancing
            
            ENDHLSL
        }
    	
    	 Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Vertical
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}