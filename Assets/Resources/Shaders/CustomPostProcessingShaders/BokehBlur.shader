Shader "Custom/PostProcessing/BokehBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
        _Offset("Offset", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
    	Cull Off ZWrite Off ZTest Always

        Pass
        {
            Tags {"LightMode" = "UniversalForward"}

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
                // float4 _BaseMap_ST;
            	half4 _GoldenRot;
				half4 _Offset;
                // half4 _BaseColor;
            CBUFFER_END

            half4 BokehBlur(Varyings IN)
			{
				half2x2 rot = half2x2(_GoldenRot);
				half4 accumulator = 0.0;
				half4 divisor = 0.0;

				half r = 1.0;
				half2 angle = half2(0.0, _Offset.y);

				for (int j = 0; j < _Offset.x; j++)
				{
					r += 1.0 / r;
					angle = mul(rot, angle);
					half4 bokeh = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, float2(IN.uv + _Offset.zw * (r - 1.0) * angle));
					accumulator += bokeh * bokeh;
					divisor += bokeh;
				}
				return accumulator / divisor;
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
                return BokehBlur(IN);
            }
            ENDHLSL
        }
    }
}