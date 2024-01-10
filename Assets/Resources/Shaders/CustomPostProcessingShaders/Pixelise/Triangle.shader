Shader "Custom/PostProcessing/Pixelise/Triangle"
{
    Properties
    {
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float4 _TriangleParams;
        // CBUFFER_END

        #define _PixelSize _TriangleParams.x
		#define _PixelRatio _TriangleParams.y
		#define _PixelScaleX _TriangleParams.z
		#define _PixelScaleY _TriangleParams.w	

		float2 TrianglePixelizeUV(float2 uv)
		{

			float2 pixelScale = _PixelSize * float2(_PixelScaleX, _PixelScaleY / _PixelRatio);

			//乘以缩放，向下取整，再除以缩放，得到分段UV
			float2 coord = floor(uv * pixelScale) / pixelScale;

			uv -= coord;
			uv *= pixelScale;

			//进行三角形像素偏移处理
			coord += 
				float2(step(1.0 - uv.y, uv.x) / (2.0 * pixelScale.x),//X
				step(uv.x, uv.y) / (2.0 * pixelScale.y)//Y
				);

			return  coord;
		}


		float4 frag(Varyings i) : SV_Target
		{
			float2 uv = TrianglePixelizeUV(i.uv);

			return GetSource(uv);		
		}


    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Triangle"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing
            

            
            ENDHLSL
        }
    }
}