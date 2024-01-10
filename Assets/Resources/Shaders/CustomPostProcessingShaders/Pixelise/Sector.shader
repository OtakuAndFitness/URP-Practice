Shader "Custom/PostProcessing/Pixelise/Sector"
{
    Properties
    {
//        _MainTex("MainTex", 2D) = "white" {}
//        _BackgroundColor("BackgroundColor", Color) = (1,1,1,1)
//        _Params("Params", Vector) = (1,1,1,1)
//        _Params2("Params2", Vector) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float4 _SectorParams;
			float2 _SectorParams2;
			half4 _SectorBackground;
        // CBUFFER_END

        #define _PixelIntervalX _SectorParams2.x
		#define _PixelIntervalY _SectorParams2.y


		float4 SectorPixelize(float2 uv)
		{
			float pixelScale = 1.0 / _SectorParams.x;

			float ratio = _ScreenParams.y / _ScreenParams.x;
			uv.x = uv.x / ratio;

			//x和y坐标分别除以缩放系数，在用floor向下取整，再乘以缩放系数，得到分段UV
			float2 coord = half2(_PixelIntervalX *  floor(uv.x / (pixelScale * _PixelIntervalX)), (_PixelIntervalY)* floor(uv.y / (pixelScale * _PixelIntervalY)));

			//设定扇形坐标
			float2 circleCenter = coord * pixelScale;

			//计算当前uv值隔圆心的距离，并乘以缩放系数
			float dist = length(uv - circleCenter) * _SectorParams.x;
			//圆心坐标乘以缩放系数
			circleCenter.x *= ratio;

			//采样
			float4 screenColor = GetSource(circleCenter);

			//对于距离大于半径的像素，替换为背景色
			if (dist > _SectorParams.z)  screenColor = _SectorBackground;

			return screenColor;
		}

		
		
		float4 frag(Varyings i): SV_Target
		{
			return SectorPixelize(i.uv);
		}

    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Sector"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing
            

            
            ENDHLSL
        }
    }
}