Shader "Custom/PostProcessing/Pixelise/Diamond"
{
    Properties
    {
//        _MainTex("MainTex", 2D) = "white" {}
//        _PixelSize("PixelSize", Float) = 1
        
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

        // CBUFFER_START(UnityPerMaterial)
            float _DiamondPixelSize;
        // CBUFFER_END

        float2 DiamondPixelizeUV(float2 uv)
		{
			half2 pixelSize = 10 / _DiamondPixelSize;
			
			half2 coord = uv * pixelSize;
			
			//计算当前Diamond的朝向
			int direction = int(dot(frac(coord), half2(1, 1)) >= 1.0) + 2 * int(dot(frac(coord), half2(1, -1)) >= 0.0);
			
			//进行向下取整
			coord = floor(coord);
			
			//处理Diamond的四个方向
			if (direction == 0) coord += half2(0, 0.5);
			if(direction == 1) coord += half2(0.5, 1);
			if(direction == 2) coord += half2(0.5, 0);
			if(direction == 3) coord += half2(1, 0.5);
			
			//最终缩放uv
			coord /= pixelSize;
			
			return coord;
		}
		
		
		
		float4 frag(Varyings i): SV_Target
		{
			float2 uv = DiamondPixelizeUV(i.uv);
			
			return GetSource(uv);
		}

    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Diamond"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing
            

            
            ENDHLSL
        }
    }
}