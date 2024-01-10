Shader "Custom/PostProcessing/Blur/BokehBlur"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _Offset("Offset", Vector) = (1,1,1,1)
//    	_GoldenRot("GoldenRot", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
        	Name "Bokeh Blur"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            #include "../CustomPostProcessing.hlsl"

            // CBUFFER_START(UnityPerMaterial)
                // float4 _BaseMap_ST;
            float _BokehBlurSize;
			float _BokehIteration;
                // half4 _BaseColor;
            // CBUFFER_END

            half4 BokehBlur(Varyings IN)
			{
                half4 color = GetSource(IN);
				float a = 2.3389f;
                float2x2 rot = float2x2(cos(a), -sin(a), sin(a), cos(a));
                float2 angle = float2(_BokehBlurSize, 0.0f);
                float r = 0.0f;
                float2 uv = 0.0f;

                for (int i = 1; i <= _BokehIteration; i++) {
                    r = sqrt(i);
                    angle = mul(rot, angle);
                    uv = IN.uv + _SourceTexture_TexelSize.xy * angle * r;
                    color += GetSource(uv);
                }

                color /= _BokehIteration;
				return color;
			}

            half4 frag(Varyings IN) : SV_Target
            {
                // UNITY_SETUP_INSTANCE_ID(IN);
                // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return BokehBlur(IN);
            }
            ENDHLSL
        }
    }
}