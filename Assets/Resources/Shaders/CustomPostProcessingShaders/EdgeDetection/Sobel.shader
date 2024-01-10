Shader "Custom/PostProcessing/EdgeDetection/Sobel"
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
		    float2 _SobelParams;
			half4 _SobelEdgeColor;
			half4 _SobelBackgroundColor;
		// CBUFFER_END

		#define _EdgeWidth _SobelParams.x
		#define _BackgroundFade _SobelParams.y
		

		float intensity(in float4 color)
		{
			return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
		}
		
		float sobel(float stepx, float stepy, float2 center)
		{
			// get samples around pixel
			float topLeft = intensity(GetSource(center + float2(-stepx, stepy)));
			float midLeft = intensity(GetSource(center + float2(-stepx, 0)));
			float bottomLeft = intensity(GetSource(center + float2(-stepx, -stepy)));
			float midTop = intensity(GetSource(center + float2(0, stepy)));
			float midBottom = intensity(GetSource(center + float2(0, -stepy)));
			float topRight = intensity(GetSource(center + float2(stepx, stepy)));
			float midRight = intensity(GetSource(center + float2(stepx, 0)));
			float bottomRight = intensity(GetSource(center + float2(stepx, -stepy)));
			
			// Sobel masks (see http://en.wikipedia.org/wiki/Sobel_operator)
			//        1 0 -1     -1 -2 -1
			//    X = 2 0 -2  Y = 0  0  0
			//        1 0 -1      1  2  1

			// Gx = sum(kernelX[i][j]*image[i][j])
			float Gx = topLeft + 2.0 * midLeft + bottomLeft - topRight - 2.0 * midRight - bottomRight;
			// Gy = sum(kernelY[i][j]*image[i][j]);
			float Gy = -topLeft - 2.0 * midTop - topRight + bottomLeft + 2.0 * midBottom + bottomRight;
			float sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
			return sobelGradient;
		}
		
		
		
		half4 frag(Varyings i): SV_Target
		{
			
			half4 sceneColor = GetSource(i.uv);
			
			float sobelGradient= sobel(_EdgeWidth /_ScreenParams.x, _EdgeWidth /_ScreenParams.y , i.uv);

			half4 backgroundColor = lerp(sceneColor, _SobelBackgroundColor, _BackgroundFade);

			float3 edgeColor = lerp(backgroundColor.rgb, _SobelEdgeColor.rgb, sobelGradient);

			return float4(edgeColor, 1);

		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Sobel"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}