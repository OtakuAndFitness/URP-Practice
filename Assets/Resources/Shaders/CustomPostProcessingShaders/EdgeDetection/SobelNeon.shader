Shader "Custom/PostProcessing/EdgeDetection/SobelNeon"
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
		    float4 _SobelNeonParams;
			half4 _SobelNeonBackgroundColor;
		// CBUFFER_END

		#define _EdgeWidth _SobelNeonParams.x
		#define _EdgeNeonFade _SobelNeonParams.y
		#define _Brightness _SobelNeonParams.z
		#define _BackgroundFade _SobelNeonParams.w


		float intensity(in float4 color)
		{
			return sqrt((color.x * color.x) + (color.y * color.y) + (color.z * color.z));
		}

		float3 sobel(float stepx, float stepy, float2 center)
		{
			// get samples around pixel
			float3 topLeft = GetSource(center + float2(-stepx, stepy)).rgb;
			float3 midLeft = GetSource(center + float2(-stepx, 0)).rgb;
			float3 bottomLeft = GetSource(center + float2(-stepx, -stepy)).rgb;
			float3 midTop = GetSource(center + float2(0, stepy)).rgb;
			float3 midBottom = GetSource(center + float2(0, -stepy)).rgb;
			float3 topRight = GetSource(center + float2(stepx, stepy)).rgb;
			float3 midRight = GetSource(center + float2(stepx, 0)).rgb;
			float3 bottomRight = GetSource(float2(stepx, -stepy)).rgb;

			// Sobel masks (see http://en.wikipedia.org/wiki/Sobel_operator)
			//        1 0 -1     -1 -2 -1
			//    X = 2 0 -2  Y = 0  0  0
			//        1 0 -1      1  2  1

			// Gx = sum(kernelX[i][j]*image[i][j])
			float3 Gx = topLeft + 2.0 * midLeft + bottomLeft - topRight - 2.0 * midRight - bottomRight;
			// Gy = sum(kernelY[i][j]*image[i][j]);
			float3 Gy = -topLeft - 2.0 * midTop - topRight + bottomLeft + 2.0 * midBottom + bottomRight;
			float3 sobelGradient = sqrt((Gx * Gx) + (Gy * Gy));
			return sobelGradient;
		}



		half4 frag(Varyings i) : SV_Target
		{
			half4 sceneColor = GetSource(i.uv);

			float3 sobelGradient = sobel(_EdgeWidth / _ScreenParams.x, _EdgeWidth / _ScreenParams.y , i.uv);

			half3 backgroundColor = lerp(_SobelNeonBackgroundColor.rgb, sceneColor.rgb, _BackgroundFade);

			float3 edgeColor = lerp(backgroundColor.rgb, sobelGradient.rgb, _EdgeNeonFade);

			return float4(edgeColor * _Brightness, 1);

		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "SobelNeon"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }
    }
}