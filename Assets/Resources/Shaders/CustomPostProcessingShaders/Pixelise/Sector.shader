Shader "Custom/PostProcessing/Pixel/Sector"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _BackgroundColor("BackgroundColor", Color) = (1,1,1,1)
        _Params("Params", Vector) = (1,1,1,1)
        _Params2("Params2", Vector) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _Params;
			float2 _Params2;
			half4 _BackgroundColor;
        CBUFFER_END

        #define _PixelIntervalX _Params2.x
		#define _PixelIntervalY _Params2.y


		float4 SectorPixelize(float2 uv)
		{
			float pixelScale = 1.0 / _Params.x;

			float ratio = _ScreenParams.y / _ScreenParams.x;
			uv.x = uv.x / ratio;

			//x和y坐标分别除以缩放系数，在用floor向下取整，再乘以缩放系数，得到分段UV
			float2 coord = half2(_PixelIntervalX *  floor(uv.x / (pixelScale * _PixelIntervalX)), (_PixelIntervalY)* floor(uv.y / (pixelScale * _PixelIntervalY)));

			//设定扇形坐标
			float2 circleCenter = coord * pixelScale;

			//计算当前uv值隔圆心的距离，并乘以缩放系数
			float dist = length(uv - circleCenter) * _Params.x;
			//圆心坐标乘以缩放系数
			circleCenter.x *= ratio;

			//采样
			float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, circleCenter);

			//对于距离大于半径的像素，替换为背景色
			if (dist > _Params.z)  screenColor = _BackgroundColor;

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
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing
            

            
            ENDHLSL
        }
    }
}