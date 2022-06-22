Shader "Custom/PostProcessing/GrainyBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

    	Cull Off ZWrite Off ZTest Always

        Pass
        {
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                // float4 _MainTex_ST;
                // half4 _BaseColor;
                uniform half2 _Params;	
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

            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                
                return OUT;
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