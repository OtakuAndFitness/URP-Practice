Shader "Custom/PostProcessing/EdgeDetection/Roberts"
{
    Properties
    {
//    	_MainTex("Main Tex", 2D) = "white"{}
//        _Params("_Params", Vector) = (1,1,1,1)
//    	_EdgeColor("_EdgeColor", Color) = (1,1,1,1)
//    	_BackgroundColor("_BackgroundColor", Color) = (1,1,1,1)
    }
    
    HLSLINCLUDE

		#include "../CustomPostProcessing.hlsl"

		// CBUFFER_START(UnityPerMaterial)
		    float2 _RobertsParams;
			half4 _RobertsEdgeColor;
			half4 _RobertsBackgroundColor;
		// CBUFFER_END

		#define _EdgeWidth _RobertsParams.x
		#define _BackgroundFade _RobertsParams.y

		
		float intensity(in float4 color)
		{
			return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
		}
		
		float sobel(float stepx, float stepy, float2 center)
		{
			// get samples around pixel
			float topLeft = intensity(GetSource(center + float2(-stepx, stepy)));
			float bottomLeft = intensity(GetSource(center + float2(-stepx, -stepy)));
			float topRight = intensity(GetSource(center + float2(stepx, stepy)));
			float bottomRight = intensity(GetSource(center + float2(stepx, -stepy)));
			
			// Roberts Operator
			//X = -1   0      Y = 0  -1
			//     0   1          1   0
			
			// Gx = sum(kernelX[i][j]*image[i][j])
			float Gx = -1.0 * topLeft + 1.0 * bottomRight;
			
			// Gy = sum(kernelY[i][j]*image[i][j]);
			float Gy = -1.0 * topRight + 1.0 * bottomLeft;
			
			
			float sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
			return sobelGradient;
		}
		
		
		
		half4 frag(Varyings i): SV_Target
		{
			
			half4 sceneColor = GetSource(i.uv);
			
			float sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y, i.uv);
			
			half4 backgroundColor = lerp(sceneColor, _RobertsBackgroundColor, _BackgroundFade);
			
			float3 edgeColor = lerp(backgroundColor.rgb, _RobertsEdgeColor.rgb, sobelGradient);
			
			return float4(edgeColor, 1);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Roberts"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}