Shader "Custom/PostProcessing/Pixelise/Led"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white" {}
        _Params("Params", Vector) = (1,1,1,1)
    	_BackgroundColor("BackgroundColor", Color) = (1,1,1,1)
        
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"

        CBUFFER_START(UnityPerMaterial)
            float4 _Params;
			half4 _BackgroundColor;
        CBUFFER_END

        #define _PixelSize _Params.x
		#define _PixelRatio _Params.y
		#define _LedRadius _Params.z

		float2 RectPixelizeUV(half2 uv)
		{
			float pixelScale = 1.0 / _PixelSize;
			//除以缩放系数，在向上取整，再乘以缩放系数，得到分段UV
			float2 coord = half2(pixelScale * floor(uv.x / (pixelScale)), (pixelScale * _PixelRatio ) * floor(uv.y / (pixelScale *_PixelRatio)));

			return  coord;
		}



		float4 frag(Varyings i) : SV_Target
		{

			// 实现矩形像素效果
			float2 uv = RectPixelizeUV(i.uv);
			float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

			// 计算矩形像素坐标
			half2 coord = i.uv * half2(_PixelSize, _PixelSize / _PixelRatio);

			// 横纵坐标强度渐变
			half ledX = abs(sin(coord.x * 3.1415)) * 1.5;
			half ledY = abs(sin(coord.y * 3.1415)) * 1.5;
			// 求解LedValue
			half ledValue = ledX * ledY;
			// led半径校正
			half radius = step(ledValue, _LedRadius);

			//最终颜色 = 基础led颜色 + 渐变led颜色 + 背景颜色
			color = ((1 - radius) * color) + ((color * ledValue) * radius) + radius * (1- ledValue)* _BackgroundColor;


			return color;

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