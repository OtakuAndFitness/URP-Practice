Shader "Custom/PostProcessing/GrainyBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
    	_Params("Params", Vector) = (1,1,1,1)
    }

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

            #include "../CustomPPHeader.hlsl"

            CBUFFER_START(UnityPerMaterial)
                // float4 _MainTex_ST;
                // half4 _BaseColor;
                half2 _Params;	
            CBUFFER_END

            float Rand(float2 n)
	        {
		        return sin(dot(n, half2(1233.224, 1743.335)));
	        }

            half4 GrainyBlur(Varyings i)
			{
				half2 randomOffset = float2(0.0, 0.0);
				half4 finalColor = half4(0.0, 0.0, 0.0, 0.0);
				float random = Rand(i.uv);
				
				for (int k = 0; k < int(_Params.y); k ++)
				{
					random = frac(43758.5453 * random + 0.61432);;
					randomOffset.x = (random - 0.5) * 2.0;
					random = frac(43758.5453 * random + 0.61432);
					randomOffset.y = (random - 0.5) * 2.0;
					
					finalColor += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, half2(i.uv + randomOffset * _Params.x));
				}
				return finalColor / _Params.y;
			}
            

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return GrainyBlur(IN);
            }
            ENDHLSL
        }
    }
}