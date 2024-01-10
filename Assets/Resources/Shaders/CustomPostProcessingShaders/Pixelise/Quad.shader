Shader "Custom/PostProcessing/Pixelise/Quad"
{
    Properties
    {
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float4 _QuadParams;
        // CBUFFER_END

        #define _PixelSize _QuadParams.x
		#define _PixelRatio _QuadParams.y
		#define _PixelScaleX _QuadParams.z
		#define _PixelScaleY _QuadParams.w	

		float2 RectPixelizeUV( half2 uv)
		{
			float pixelScale = 1.0 / _PixelSize;
			// Divide by the scaling factor, round up, and multiply by the scaling factor to get the segmented UV
			float2 coord = half2(pixelScale * _PixelScaleX * floor(uv.x / (pixelScale *_PixelScaleX)), (pixelScale * _PixelRatio *_PixelScaleY) * floor(uv.y / (pixelScale *_PixelRatio * _PixelScaleY)));

			return  coord;
		}

	

		float4 frag(Varyings i) : SV_Target
		{

			float2 uv = RectPixelizeUV(i.uv);

			float4 color = GetSource(uv);

			return color;

		}


    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Quad"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            

            
            ENDHLSL
        }
    }
}