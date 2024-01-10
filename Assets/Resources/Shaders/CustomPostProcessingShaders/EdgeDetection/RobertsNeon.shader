Shader "Custom/PostProcessing/EdgeDetection/RobertsNeon"
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
			float4 _RobertsNeonParams;
			half4 _RobertsNeonBackgroundColor;
		// CBUFFER_END
    
		#define _EdgeWidth _RobertsNeonParams.x
		#define _EdgeNeonFade _RobertsNeonParams.y
		#define _Brigtness _RobertsNeonParams.z
		#define _BackgroundFade _RobertsNeonParams.w
		
		
		float3 sobel(float stepx, float stepy, float2 center)
		{
			// get samples around pixel
			float3 topLeft = GetSource(center + float2(-stepx, stepy)).rgb;
			float3 bottomLeft = GetSource(center + float2(-stepx, -stepy)).rgb;
			float3 topRight = GetSource(center + float2(stepx, stepy)).rgb;
			float3 bottomRight = GetSource(center + float2(stepx, -stepy)).rgb;
			
			// Roberts Operator
			//X = -1   0      Y = 0  -1
			//     0   1          1   0
			
			// Gx = sum(kernelX[i][j]*image[i][j])
			float3 Gx = -1.0 * topLeft + 1.0 * bottomRight;
			
			// Gy = sum(kernelY[i][j]*image[i][j]);
			float3 Gy = -1.0 * topRight + 1.0 * bottomLeft;
			
			
			float3 sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
			return sobelGradient;
		}
		
		
		half4 frag(Varyings i): SV_Target
		{
			
			
			half4 sceneColor = GetSource(i.uv);
			
			float3 sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.uv);
			
			half3 backgroundColor = lerp(_RobertsNeonBackgroundColor.rgb, sceneColor.rgb, _BackgroundFade);
			
			//Edge Opacity
			float3 edgeColor = lerp(backgroundColor.rgb, sobelGradient.rgb, _EdgeNeonFade);
			
			return float4(edgeColor * _Brigtness, 1);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Roberts Neon"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}