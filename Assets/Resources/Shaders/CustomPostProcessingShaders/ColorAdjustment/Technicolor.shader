Shader "Custom/PostProcessing/ColorAdjustment/Technicolor"
{
    Properties
    {
        _Exposure("Exposure", Float) = 1
    	_Indensity("Indensity", Float) = 1
    	_ColorBalance("ColorBalance", Color) = (1,1,1,1)
        _MainTex("MainTex", 2D) = "white" {}
    }
    
    HLSLINCLUDE
        #include "../CustomPPHeader.hlsl"
        CBUFFER_START(UnityPerMaterial)
			float _Exposure;
            float _Indensity;
			half4 _ColorBalance;
        CBUFFER_END

        // reference : https://github.com/crosire/reshade-shaders/blob/master/Shaders/Technicolor.fx
		half4 frag(Varyings i): SV_Target
		{
			const half3 cyanfilter = float3(0.0, 1.30, 1.0);
			const half3 magentafilter = float3(1.0, 0.0, 1.05);
			const half3 yellowfilter = float3(1.6, 1.6, 0.05);
			const half2 redorangefilter = float2(1.05, 0.620); // RG_
			const half2 greenfilter = float2(0.30, 1.0);       // RG_
			const half2 magentafilter2 = magentafilter.rb;     // R_B


			half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);

			half3 balance = 1.0 / (_ColorBalance.rgb * _Exposure);

			half negative_mul_r = dot(redorangefilter, color.rg * balance.rr);
			half negative_mul_g = dot(greenfilter, color.rg * balance.gg);
			half negative_mul_b = dot(magentafilter2, color.rb * balance.bb);

			half3 output_r = negative_mul_r.rrr + cyanfilter;
			half3 output_g = negative_mul_g .rrr + magentafilter;
			half3 output_b = negative_mul_b.rrr + yellowfilter;

			half3 result = output_r  * output_g * output_b;
			return half4(lerp(color.rgb, result.rgb, _Indensity), 1.0);

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