Shader "Custom/PostProcessing/Blur/IrisBlur"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//		_Params("Params", Vector) = (1,1,1,1)
//    	_Gradient("Gradient", Vector) = (1,1,1,1)
//    	_GoldenRot("GoldenRot", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Iris Blur"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            #include "../CustomPostProcessing.hlsl"

            // CBUFFER_START(UnityPerMaterial)
				float3 _IrisGradient;
				float2 _IrisParameters;
            // CBUFFER_END

            float IrisMask(float2 uv)
	        {
		        float2 center = uv * 2.0 - 1.0 + _IrisGradient.xy; // [0,1] -> [-1,1] 
		        return dot(center, center) * _IrisGradient.z;
	        }

            half4 IrisBlur(Varyings i)
			{
            	float a = 2.3389f;
                float2x2 rot = float2x2(cos(a), -sin(a), sin(a), cos(a));
				half4 accumulator = 0.0;
				half4 divisor = 0.0;
				
				half r = 1.0;
				half2 angle = half2(0.0, _IrisParameters.y * saturate(IrisMask(i.uv)));
				
				for (int j = 0; j < _IrisParameters.x; j ++)
				{
					r += 1.0 / r;
					angle = mul(rot, angle);
					half4 bokeh = GetSource(i.uv + _SourceTexture_TexelSize.xy * (r - 1.0) * angle);
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
                return IrisBlur(IN);
            }
            ENDHLSL
        }
    }
}