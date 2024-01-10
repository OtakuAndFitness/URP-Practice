Shader "Custom/PostProcessing/Blur/KawaseBlur"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _Offset("Offset", Float) = 1
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}
        
        ZTest Always
        Cull Off
        Zwrite Off

        Pass
        {
        	Name "Kawase Blur"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex KawaseBlurPassVertex
            #pragma fragment frag
            // #pragma multi_compile_instancing

            #include "../CustomPostProcessing.hlsl"

            // CBUFFER_START(UnityPerMaterial)
                // float4 _MainTex_TexelSize;
                float _KawaseBlurSize;
                // float4 _BaseMap_ST;
                // half4 _BaseColor;
            // CBUFFER_END

            struct KawaseVaryings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                
            };

	        KawaseVaryings KawaseBlurPassVertex(uint vertexID : SV_VertexID)
	        {
	            KawaseVaryings output;

	        	ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
	        	output.positionHCS = dataSS.positionHCS;
	        	output.uv = dataSS.uv;
	        	output.uv1 = output.uv.xyxy + _KawaseBlurSize * float4(-0.5f, -0.5f, 0.5f, 0.5f) * _SourceTexture_TexelSize.xyxy;

	        	return output;
	        }

            half4 frag(KawaseVaryings IN) : SV_Target
            {
                // UNITY_SETUP_INSTANCE_ID(IN);
                // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
                
		        // half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                half4 color = 0;
		        color += GetSource(IN.uv1.xy) * 0.25; 
		        color += GetSource(IN.uv1.zy) * 0.25; 
		        color += GetSource(IN.uv1.xw) * 0.25; 
		        color += GetSource(IN.uv1.zw) * 0.25; 
		        return color;
            }
            ENDHLSL
        }
    }
}