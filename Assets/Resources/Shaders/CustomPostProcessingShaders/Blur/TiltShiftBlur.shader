Shader "Custom/PostProcessing/TiltShiftBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
//        _Offset("Offset", Float) = 1
		_Params("Params", Vector) = (1,1,1,1)
    	_Gradient("Gradient", Vector) = (1,1,1,1)
    	_GoldenRot("GoldenRot", Vector) = (1,1,1,1)

    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZTest Always
        Cull Off
        Zwrite Off

        Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vertDefault
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "../CustomPPHeader.hlsl"

            CBUFFER_START(UnityPerMaterial)
                // float4 _MainTex_TexelSize;
                // float _Offset;
                // float4 _BaseMap_ST;
                // half4 _BaseColor;
                half3 _Gradient;
	            half4 _GoldenRot;
	            // uniform half4 _Distortion;
	            half4 _Params;
            CBUFFER_END

            float TiltShiftMask(float2 uv)
	        {
		        float centerY = uv.y * 2.0 - 1.0 + _Gradient.x; // [0,1] -> [-1,1]
		        return pow(abs(centerY * _Gradient.y), _Gradient.z);
	        }

            half4 TiltShiftBlur(Varyings IN)
			{
				half2x2 rot = half2x2(_GoldenRot);
				half4 accumulator = 0.0;
				half4 divisor = 0.0;
				
				half r = 1.0;
				half2 angle = half2(0.0, _Params.y * saturate(TiltShiftMask(IN.uv)));
				
				for (int j = 0; j < _Params.x; j ++)
				{
					r += 1.0 / r;
					angle = mul(rot, angle);
					half4 bokeh = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(IN.uv + _Params.zw * (r - 1.0) * angle));
					accumulator += bokeh * bokeh;
					divisor += bokeh;
				}
				return accumulator / divisor;
			}
            
            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return TiltShiftBlur(IN);
            }
            ENDHLSL
        }
    }
}