Shader "Custom/PostProcessing/Blur/DirectionalBlur"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//    	_Params("Params", Vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
		
        ZTest Always
        Cull Off
        Zwrite Off
        
        Pass
        {
        	Name "Directional Blur"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex Vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            #include "../CustomPostProcessing.hlsl"

            // CBUFFER_START(UnityPerMaterial)
                // float4 _MainTex_TexelSize;
                // float _Offset;
                // float4 _BaseMap_ST;
                // half4 _BaseColor;
                float3 _DirectionalBlurSize;
            // CBUFFER_END

            half4 DirectionalBlur(Varyings i)
	        {
		        half4 color = half4(0.0, 0.0, 0.0, 0.0);

		        for (int k = -_DirectionalBlurSize.x; k < _DirectionalBlurSize.x; k++)
		        {
			        color += GetSource(i.uv - _DirectionalBlurSize.yz * _SourceTexture_TexelSize.xy * k);
		        }
		        half4 finalColor = color / (_DirectionalBlurSize.x * 2.0);

		        return finalColor;
	        }

            half4 frag(Varyings IN) : SV_Target
            {
                // UNITY_SETUP_INSTANCE_ID(IN);
                // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return DirectionalBlur(IN);
            }
            ENDHLSL
        }
    }
}