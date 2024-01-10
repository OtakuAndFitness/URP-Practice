Shader "Custom/PostProcessing/Pixelise/Leaf"
{
    Properties
    {
//        _MainTex("MainTex", 2D) = "white" {}
//        _Params("Params", Vector) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float4 _LeafParams;
        // CBUFFER_END

        #define _PixelSize _LeafParams.x
		#define _PixelRatio _LeafParams.y
		#define _PixelScaleX _LeafParams.z
		#define _PixelScaleY _LeafParams.w


		float2 TrianglePixelizeUV(float2 uv)
		{
			float2 pixelScale = _PixelSize * float2(_PixelScaleX, _PixelScaleY / _PixelRatio);

			//乘以缩放，向下取整，再除以缩放，得到分段UV
			float2 coord = floor(uv * pixelScale) / pixelScale;

			uv -= coord;
			uv *= pixelScale;

			//进行像素偏移处理
			coord += 
				float2(step(1.0 - uv.y, uv.x) / (pixelScale.x),	// Leaf X
				step(uv.x, uv.y) / (pixelScale.y)//Leaf Y
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
        	Name "Leaf"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing
            

            
            ENDHLSL
        }
    }
}