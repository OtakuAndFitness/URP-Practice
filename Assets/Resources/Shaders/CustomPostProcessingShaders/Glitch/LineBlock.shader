Shader "Custom/PostProcessing/Glitch/LineBlock"
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

		#pragma shader_feature USING_FREQUENCY_INFINITE

        // CBUFFER_START(UnityPerMaterial)
            float4 _LineBlockParams;
		    float4 _LineBlockParams2;
            // half3 _Params3;
        // CBUFFER_END

        #define _Frequency _LineBlockParams.x
	    #define _TimeX _LineBlockParams.y
	    #define _Amount _LineBlockParams.z
	    #define _Offset _LineBlockParams2.x
	    #define _LinesWidth _LineBlockParams2.y
	    #define _Alpha _LineBlockParams2.z

        float randomNoise(float2 c)
		{
			return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
		}
		
		float trunc(float x, float num_levels)
		{
			return floor(x * num_levels) / num_levels;
		}
		
		float2 trunc(float2 x, float2 num_levels)
		{
			return floor(x * num_levels) / num_levels;
		}
		
		float3 rgb2yuv(float3 rgb)
		{
			float3 yuv;
			yuv.x = dot(rgb, float3(0.299, 0.587, 0.114));
			yuv.y = dot(rgb, float3(-0.14713, -0.28886, 0.436));
			yuv.z = dot(rgb, float3(0.615, -0.51499, -0.10001));
			return yuv;
		}
		
		float3 yuv2rgb(float3 yuv)
		{
			float3 rgb;
			rgb.r = yuv.x + yuv.z * 1.13983;
			rgb.g = yuv.x + dot(float2(-0.39465, -0.58060), yuv.yz);
			rgb.b = yuv.x + yuv.y * 2.03211;
			return rgb;
		}
		
		
		float4 Frag_Horizontal(Varyings i): SV_Target
		{
			float2 uv = i.uv;
			
			half strength = 0;
			#if USING_FREQUENCY_INFINITE
				strength = 10;
			#else
				strength = 0.5 + 0.5 * cos(_TimeX * _Frequency);
			#endif
			
			_TimeX *= strength;
			
			//	[1] 生成随机强度梯度线条
			float truncTime = trunc(_TimeX, 4.0);
			float uv_trunc = randomNoise(trunc(uv.yy, float2(8, 8)) + 100.0 * truncTime);
			float uv_randomTrunc = 6.0 * trunc(_TimeX, 24.0 * uv_trunc);
			
			// [2] 生成随机非均匀宽度线条
			float blockLine_random = 0.5 * randomNoise(trunc(uv.yy + uv_randomTrunc, float2(8 * _LinesWidth, 8 * _LinesWidth)));
			blockLine_random += 0.5 * randomNoise(trunc(uv.yy + uv_randomTrunc, float2(7, 7)));
			blockLine_random = blockLine_random * 2.0 - 1.0;
			blockLine_random = sign(blockLine_random) * saturate((abs(blockLine_random) - _Amount) / (0.4));
			blockLine_random = lerp(0, blockLine_random, _Offset);
			
			
			// [3] 生成源色调的blockLine Glitch
			float2 uv_blockLine = uv;
			uv_blockLine = saturate(uv_blockLine + float2(0.1 * blockLine_random, 0));
			float4 blockLineColor = GetSource(abs(uv_blockLine));
			
			// [4] 将RGB转到YUV空间，并做色调偏移
			// RGB -> YUV
			float3 blockLineColor_yuv = rgb2yuv(blockLineColor.rgb);
			// adjust Chrominance | 色度
			blockLineColor_yuv.y /= 1.0 - 3.0 * abs(blockLine_random) * saturate(0.5 - blockLine_random);
			// adjust Chroma | 浓度
			blockLineColor_yuv.z += 0.125 * blockLine_random * saturate(blockLine_random - 0.5);
			float3 blockLineColor_rgb = yuv2rgb(blockLineColor_yuv);
			
			
			// [5] 与源场景图进行混合
			float4 sceneColor = GetSource(i.uv);
			return lerp(sceneColor, float4(blockLineColor_rgb, blockLineColor.a), _Alpha);
		}
		
		float4 Frag_Vertical(Varyings i): SV_Target
		{
			float2 uv = i.uv;
			
			half strength = 0;
			#if USING_FREQUENCY_INFINITE
				strength = 10;
			#else
				strength = 0.5 + 0.5 * cos(_TimeX * _Frequency);
			#endif
			
			_TimeX *= strength;
			
			// [1] 生成随机均匀宽度线条
			float truncTime = trunc(_TimeX, 4.0);
			float uv_trunc = randomNoise(trunc(uv.xx, float2(8, 8)) + 100.0 * truncTime);
			float uv_randomTrunc = 6.0 * trunc(_TimeX, 24.0 * uv_trunc);
			
			// [2] 生成随机非均匀宽度线条 | Generate Random inhomogeneous Block Line
			float blockLine_random = 0.5 * randomNoise(trunc(uv.xx + uv_randomTrunc, float2(8 * _LinesWidth, 8 * _LinesWidth)));
			blockLine_random += 0.5 * randomNoise(trunc(uv.xx + uv_randomTrunc, float2(7, 7)));
			blockLine_random = blockLine_random * 2.0 - 1.0;
			blockLine_random = sign(blockLine_random) * saturate((abs(blockLine_random) - _Amount) / (0.4));
			blockLine_random = lerp(0, blockLine_random, _Offset);
			
			// [3] 生成源色调的blockLine Glitch
			float2 uv_blockLine = uv;
			uv_blockLine = saturate(uv_blockLine + float2(0, 0.1 * blockLine_random));
			float4 blockLineColor = GetSource(abs(uv_blockLine));
			
			// [4] 将RGB转到YUV空间，并做色调偏移
			// RGB -> YUV
			float3 blockLineColor_yuv = rgb2yuv(blockLineColor.rgb);
			// adjust Chrominance | 色度
			blockLineColor_yuv.y /= 1.0 - 3.0 * abs(blockLine_random) * saturate(0.5 - blockLine_random);
			// adjust Chroma | 浓度
			blockLineColor_yuv.z += 0.125 * blockLine_random * saturate(blockLine_random - 0.5);
			float3 blockLineColor_rgb = yuv2rgb(blockLineColor_yuv);
			
			// [5] 与源场景图进行混合
			float4 sceneColor = GetSource(i.uv);
			return lerp(sceneColor, float4(blockLineColor_rgb, blockLineColor.a), _Alpha);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "LineBlock Horizontal"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Horizontal
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    	
    	 Pass
        {
        	Name "LineBlock Vertical"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag_Vertical
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}