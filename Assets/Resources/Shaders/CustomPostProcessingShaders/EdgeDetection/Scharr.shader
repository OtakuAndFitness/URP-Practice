Shader "Custom/PostProcessing/EdgeDetection/Scharr"
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
		    float2 _ScharrParams;
			half4 _ScharrEdgeColor;
			half4 _ScharrBackgroundColor;
		// CBUFFER_END

		#define _EdgeWidth _ScharrParams.x
		#define _BackgroundFade _ScharrParams.y


		float intensity(in float4 color)
		{
			return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
		}

		float scharr(float stepx, float stepy, float2 center)
		{
			// get samples around pixel
			float topLeft = intensity(GetSource(center + float2(-stepx, stepy)));
			float midLeft = intensity(GetSource( center + float2(-stepx, 0)));
			float bottomLeft = intensity(GetSource(center + float2(-stepx, -stepy)));
			float midTop = intensity(GetSource(center + float2(0, stepy)));
			float midBottom = intensity(GetSource(center + float2(0, -stepy)));
			float topRight = intensity(GetSource(center + float2(stepx, stepy)));
			float midRight = intensity(GetSource(center + float2(stepx, 0)));
			float bottomRight = intensity(GetSource(center + float2(stepx, -stepy)));

			// scharr masks ( http://en.wikipedia.org/wiki/Sobel_operator#Alternative_operators)
			//        3 0 -3        3 10   3
			//    X = 10 0 -10  Y = 0  0   0
			//        3 0 -3        -3 -10 -3

			// Gx = sum(kernelX[i][j]*image[i][j]);
			float Gx = 3.0* topLeft + 10.0 * midLeft + 3.0 * bottomLeft -3.0* topRight - 10.0 * midRight - 3.0* bottomRight;
			// Gy = sum(kernelY[i][j]*image[i][j]);
			float Gy = 3.0 * topLeft + 10.0 * midTop + 3.0 * topRight -3.0* bottomLeft - 10.0 * midBottom -3.0* bottomRight;

			float scharrGradient = sqrt((Gx * Gx) + (Gy * Gy));
			return scharrGradient;
		}



		half4 frag(Varyings i) : SV_Target
		{

			half4 sceneColor = GetSource(i.uv);

			float scharrGradient = scharr(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y , i.uv);

			//return sceneColor * scharrGradient;
			//BackgroundFading
			sceneColor = lerp(sceneColor, _ScharrBackgroundColor, _BackgroundFade);

			//Edge Opacity
			float3 edgeColor = lerp(sceneColor.rgb, _ScharrEdgeColor.rgb, scharrGradient);

			return float4(edgeColor, 1);

		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Scharr"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}