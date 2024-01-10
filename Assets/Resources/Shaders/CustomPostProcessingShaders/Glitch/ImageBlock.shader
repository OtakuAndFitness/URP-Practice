Shader "Custom/PostProcessing/Glitch/ImageBlock"
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

    // CBUFFER_START(UnityPerMaterial)
        float3 _ImageBlockParams;
		float4 _ImageBlockParams2;
        float3 _ImageBlockParams3;
    // CBUFFER_END

    #define _TimeX _ImageBlockParams.x
	#define _Offset _ImageBlockParams.y
	#define _Fade _ImageBlockParams.z

	#define _BlockLayer1_U _ImageBlockParams2.w
	#define _BlockLayer1_V _ImageBlockParams2.x
	#define _BlockLayer2_U _ImageBlockParams2.y
	#define _BlockLayer2_V _ImageBlockParams2.z

	#define _RGBSplit_Indensity _ImageBlockParams3.x
	#define _BlockLayer1_Indensity _ImageBlockParams3.y
	#define _BlockLayer2_Indensity _ImageBlockParams3.z

    float randomNoise(float2 seed)
	{
		return frac(sin(dot(seed * floor(_TimeX * 30.0), float2(127.1, 311.7))) * 43758.5453123);
	}
	
	float randomNoise(float seed)
	{
		return randomNoise(float2(seed, 1.0));
	}
	
	float4 frag(Varyings i): SV_Target
	{
		float2 uv = i.uv.xy;
		
		//求解第一层blockLayer
		float2 blockLayer1 = floor(uv * float2(_BlockLayer1_U, _BlockLayer1_V));
		float2 blockLayer2 = floor(uv * float2(_BlockLayer2_U, _BlockLayer2_V));

		//return float4(blockLayer1, blockLayer2);
		
		float lineNoise1 = pow(randomNoise(blockLayer1), _BlockLayer1_Indensity);
		float lineNoise2 = pow(randomNoise(blockLayer2), _BlockLayer2_Indensity);
		float RGBSplitNoise = pow(randomNoise(5.1379), 7.1) * _RGBSplit_Indensity;
		float lineNoise = lineNoise1 * lineNoise2 * _Offset  - RGBSplitNoise;
		
		float4 colorR = GetSource(uv);
		float4 colorG = GetSource(uv + float2(lineNoise * 0.05 * randomNoise(7.0), 0));
		float4 colorB = GetSource(uv - float2(lineNoise * 0.05 * randomNoise(23.0), 0));
		
		float4 result = float4(float3(colorR.x, colorG.y, colorB.z), colorR.a + colorG.a + colorB.a);
		result = lerp(colorR, result, _Fade);
		
		return result;
	}
    
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
    	
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Image Block"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing
            
            ENDHLSL
        }
    }
}