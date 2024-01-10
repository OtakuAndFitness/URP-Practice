Shader "Custom/PostProcessing/Glitch/DigitalStripe"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//    	_NoiseTex("Noise Tex", 2D) = "white"{}
//        _StripColorAdjustColor("_StripColorAdjustColor", vector) = (1,1,1,1)
//        _Indensity("_Indensity", float) = 1
//        _StripColorAdjustIndensity("_StripColorAdjustIndensity", float) = 1
    }
    
    HLSLINCLUDE
        #include "../CustomPostProcessing.hlsl"

		#pragma shader_feature NEED_TRASH_FRAME
    
		TEXTURE2D(_DigitalStripeNoiseTex);
		SAMPLER(sampler_DigitalStripeNoiseTex);

        // CBUFFER_START(UnityPerMaterial)
            half4 _StripColorAdjustColor;
		    float _DigitalStripeIndensity;
            float _StripColorAdjustIndensity;
        // CBUFFER_END

        half4 Frag(Varyings i): SV_Target
		{
			// 基础数据准备
			 half4 stripNoise = SAMPLE_TEXTURE2D(_DigitalStripeNoiseTex, sampler_DigitalStripeNoiseTex, i.uv);
			 half threshold = 1.001 - _DigitalStripeIndensity * 1.001;
			// uv偏移
			half uvShift = step(threshold, pow(abs(stripNoise.x), 3));
			float2 uv = frac(i.uv + stripNoise.yz * uvShift);
			half4 source = GetSource(uv);

	#ifndef NEED_TRASH_FRAME
			return source;
	#endif 	

			// 基于废弃帧插值
			half stripIndensity = step(threshold, pow(abs(stripNoise.w), 3)) * _StripColorAdjustIndensity;
			half3 color = lerp(source, _StripColorAdjustColor, stripIndensity).rgb;
			return float4(color, source.a);
		}
    ENDHLSL

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Digital Stripe"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment Frag
            // #pragma multi_compile_instancing

            
            ENDHLSL
        }

    }
}