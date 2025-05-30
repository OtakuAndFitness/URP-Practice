Shader "Custom/PostProcessing/EdgeDetection/ScharrNeon"
{
    Properties
    {
//    	_MainTex("Main Tex", 2D) = "white"{}
//        _Params("_Params", Vector) = (1,1,1,1)
//    	_BackgroundColor("_BackgroundColor", Color) = (1,1,1,1)
    }
    
    HLSLINCLUDE

		#include "../CustomPostProcessing.hlsl"

		// CBUFFER_START(UnityPerMaterial)
		    float4 _ScharrNeonParams;
			half4 _ScharrNeonBackgroundColor;
		// CBUFFER_END

		#define _EdgeWidth _ScharrNeonParams.x
		#define _EdgeNeonFade _ScharrNeonParams.y
		#define _Brigtness _ScharrNeonParams.z
		#define _BackgroundFade _ScharrNeonParams.w
		
		
		float3 scharr(float stepx, float stepy, float2 center)
		{
			// get samples around pixel
			float3 topLeft = GetSource(center + float2(-stepx, stepy)).rgb;
			float3 midLeft = GetSource(float2(-stepx, 0)).rgb;
			float3 bottomLeft = GetSource(center + float2(-stepx, -stepy)).rgb;
			float3 midTop = GetSource(center + float2(0, stepy)).rgb;
			float3 midBottom = GetSource(center + float2(0, -stepy)).rgb;
			float3 topRight = GetSource(center + float2(stepx, stepy)).rgb;
			float3 midRight = GetSource(center + float2(stepx, 0)).rgb;
			float3 bottomRight = GetSource(center + float2(stepx, -stepy)).rgb;
			
			
			// scharr masks ( http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators)
			//        3 0 -3        3 10   3
			//    X = 10 0 -10  Y = 0  0   0
			//        3 0 -3        -3 -10 -3
			
			// Gx = sum(kernelX[i][j]*image[i][j]);
			float3 Gx = 3.0 * topLeft + 10.0 * midLeft + 3.0 * bottomLeft - 3.0 * topRight - 10.0 * midRight - 3.0 * bottomRight;
			// Gy = sum(kernelY[i][j]*image[i][j]);
			float3 Gy = 3.0 * topLeft + 10.0 * midTop + 3.0 * topRight - 3.0 * bottomLeft - 10.0 * midBottom - 3.0 * bottomRight;
			
			float3 scharrGradient = sqrt((Gx * Gx) + (Gy * Gy)).rgb;
			return scharrGradient;
		}
		
		
		
		half4 frag(Varyings i): SV_Target
		{
			half4 sceneColor = GetSource(i.uv);
			
			float3 scharrGradient = scharr(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.uv);
			
			half3 backgroundColor = lerp(_ScharrNeonBackgroundColor.rgb, sceneColor.rgb, _BackgroundFade);
			
			float3 edgeColor = lerp(backgroundColor.rgb, scharrGradient.rgb, _EdgeNeonFade);
			
			return float4(edgeColor * _Brigtness, 1);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Scharr Neon"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}