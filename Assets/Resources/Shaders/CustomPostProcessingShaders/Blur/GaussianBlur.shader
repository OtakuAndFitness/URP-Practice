Shader "Custom/PostProcessing/Blur/GaussianBlur"
{
    Properties
    {
//        _MainTex("Main Tex", 2D) = "white"{}
//        _Offset("Offset", vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        ZTest Always
        Cull Off
        Zwrite Off
        
        Pass
        {
        	Name "Gaussian Blur"
//            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
	        #pragma vertex vert
            #pragma fragment frag
            // #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "../CustomPostProcessing.hlsl"

            // struct Attributes
            // {
            //     float4 positionOS : POSITION;
            //     float2 uv : TEXCOORD0;
            //     UNITY_VERTEX_INPUT_INSTANCE_ID
            // };

            struct GaussianVaryings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                float4 uv01 : TEXCOORD1;
                float4 uv23 : TEXCOORD2;
                float4 uv45 : TEXCOORD3;
                // UNITY_VERTEX_INPUT_INSTANCE_ID
                // UNITY_VERTEX_OUTPUT_STEREO
            };
            
            float4 _GaussianBlurSize;

            GaussianVaryings vert(uint vertexID : SV_VertexID)
            {
                GaussianVaryings OUT = (GaussianVaryings)0;
                
                // UNITY_SETUP_INSTANCE_ID(IN);
                // UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            	ScreenSpaceData dataSS = GetScreenSpaceData(vertexID);
                
                OUT.positionHCS = dataSS.positionHCS;
                OUT.uv = dataSS.uv;
                _GaussianBlurSize *= _SourceTexture_TexelSize.xyxy;
                OUT.uv01 = OUT.uv.xyxy + _GaussianBlurSize.xyxy * float4(1, 1, -1, -1);
	    	    OUT.uv23 = OUT.uv.xyxy + _GaussianBlurSize.xyxy * float4(1, 1, -1, -1) * 2.0;
	    	    OUT.uv45 = OUT.uv.xyxy + _GaussianBlurSize.xyxy * float4(1, 1, -1, -1) * 6.0;
                
                return OUT;
            }

            half4 frag(GaussianVaryings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                half4 color = half4(0,0,0,0);
                color = 0.4 * GetSource(IN.uv);
	    	    color += 0.15 * GetSource(IN.uv01.xy);
	    	    color += 0.15 * GetSource(IN.uv01.zw);
	    	    color += 0.10 * GetSource(IN.uv23.xy);
	    	    color += 0.10 * GetSource(IN.uv23.zw);
	    	    color += 0.05 * GetSource(IN.uv45.xy);
	    	    color += 0.05 * GetSource(IN.uv45.zw);
                // color = 0.4 * LOAD_TEXTURE2D_X(_MainTex, UnityStereoTransformScreenSpaceTex(IN.uv));
	    	    // color += 0.15 * LOAD_TEXTURE2D(_MainTex, saturate(IN.uv01.xy));
	    	    // color += 0.15 * LOAD_TEXTURE2D(_MainTex, saturate(IN.uv01.zw));
	    	    // color += 0.10 * LOAD_TEXTURE2D(_MainTex, saturate(IN.uv23.xy));
	    	    // color += 0.10 * LOAD_TEXTURE2D(_MainTex, saturate(IN.uv23.zw));
	    	    // color += 0.05 * LOAD_TEXTURE2D(_MainTex, saturate(IN.uv45.xy));
	    	    // color += 0.05 * LOAD_TEXTURE2D(_MainTex, saturate(IN.uv45.zw));
                return color;
            }

            ENDHLSL
        }
    }
}