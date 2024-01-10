Shader "Custom/PostProcessing/Glitch/AnalogNoise"
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

//    	#pragma shader_feature JITTER_DIRECTION_HORIZONTAL
//		#pragma shader_feature USING_FREQUENCY_INFINITE

        // CBUFFER_START(UnityPerMaterial)
            half4 _AnalogNoiseParams;
		    // half4 _Params2;
            // half3 _Params3;
        // CBUFFER_END

        #define _Speed _AnalogNoiseParams.x
		#define _Fading _AnalogNoiseParams.y
		#define _LuminanceJitterThreshold _AnalogNoiseParams.z
		#define _TimeX _AnalogNoiseParams.w


        float randomNoise(float2 c)
		{
			return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
		}

		half4 Frag(Varyings i): SV_Target
		{

			half4 sceneColor = GetSource(i.uv);
			half4 noiseColor = sceneColor;

			half luminance = dot(noiseColor.rgb, half3(0.22, 0.707, 0.071));
			if (randomNoise(float2(_TimeX * _Speed, _TimeX * _Speed)) > _LuminanceJitterThreshold)
			{
				noiseColor = float4(luminance, luminance, luminance, luminance);
			}

			float noiseX = randomNoise(_TimeX * _Speed + i.uv / float2(-213, 5.53));
			float noiseY = randomNoise(_TimeX * _Speed - i.uv / float2(213, -5.53));
			float noiseZ = randomNoise(_TimeX * _Speed + i.uv / float2(213, 5.53));

			noiseColor.rgb += 0.25 * float3(noiseX,noiseY,noiseZ) - 0.125;

			noiseColor = lerp(sceneColor, noiseColor, _Fading);
			
			return noiseColor;
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Analog Noise"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }

    }
}