Shader "Custom/PostProcessing/Blur/DirectionalBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
    	_Params("Params", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

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
                half3 _Params;
            CBUFFER_END

            half4 DirectionalBlur(Varyings i)
	        {
		        half4 color = half4(0.0, 0.0, 0.0, 0.0);

		        for (int k = -_Params.x; k < _Params.x; k++)
		        {
			        color += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv - _Params.yz * k);
		        }
		        half4 finalColor = color / (_Params.x * 2.0);

		        return finalColor;
	        }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return DirectionalBlur(IN);
            }
            ENDHLSL
        }
    }
}