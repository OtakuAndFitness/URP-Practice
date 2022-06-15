Shader "Custom/PostProcessing/GaussianBlur"
{
    Properties
    {
        _MainTex("Main Tex", 2D) = "white"{}
        _Offset("Offset", vector) = (1,1,1,1)
    }

    SubShader
    {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalRenderPipeline"}

        ZTest Always
        Cull Off
        Zwrite Off
        
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
                float4 uv01 : TEXCOORD1;
                float4 uv23 : TEXCOORD2;
                float4 uv45 : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;
                half4 _Offset;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = IN.uv;
                _Offset *= _MainTex_TexelSize.xyxy;
                OUT.uv01 = IN.uv.xyxy + _Offset.xyxy * float4(1, 1, -1, -1);
	    	    OUT.uv23 = IN.uv.xyxy + _Offset.xyxy * float4(1, 1, -1, -1) * 2.0;
	    	    OUT.uv45 = IN.uv.xyxy + _Offset.xyxy * float4(1, 1, -1, -1) * 3.0;
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                half4 color = half4(0,0,0,0);
                color = 0.4 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv);
	    	    color += 0.15 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv01.xy);
	    	    color += 0.15 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv01.zw);
	    	    color += 0.10 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv23.xy);
	    	    color += 0.10 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv23.zw);
	    	    color += 0.05 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv45.xy);
	    	    color += 0.05 * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex, IN.uv45.zw);

                return color;
            }
            ENDHLSL
        }
    }
}